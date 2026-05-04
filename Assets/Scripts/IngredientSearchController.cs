using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class IngredientSearchController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private TMP_InputField searchInput;

    [Header("Results UI")]
    [SerializeField] private Transform resultsParent;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private bool hideAlreadyOwned = true;
    [SerializeField] private int maxResults = 60;
    [SerializeField] private float manualRowSpacing = 12f;

    [Header("Data Source (StreamingAssets)")]
    [SerializeField] private string ingredientsFileName = "ingredients.json";

    private readonly List<IngredientEntry> _all = new List<IngredientEntry>();
    private string _lastQuery = "";

    private void OnEnable()
    {
        if (searchInput != null)
            searchInput.onValueChanged.AddListener(OnSearchChanged);
    }

    private void OnDisable()
    {
        if (searchInput != null)
            searchInput.onValueChanged.RemoveListener(OnSearchChanged);
    }

    private void Start()
    {
        StartCoroutine(LoadIngredientsThenRefresh());
    }

    private IEnumerator LoadIngredientsThenRefresh()
    {
        yield return LoadIngredientsFromStreamingAssets();
        Refresh(string.IsNullOrEmpty(_lastQuery) ? (searchInput ? searchInput.text : "") : _lastQuery);
    }

    private IEnumerator LoadIngredientsFromStreamingAssets()
    {
        _all.Clear();

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, ingredientsFileName);
        string url = path;

        // Android requires UnityWebRequest even for local StreamingAssets.
        if (!url.Contains("://"))
            url = "file://" + url;

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"IngredientSearchController: errore caricamento '{ingredientsFileName}'. {req.error}");
                yield break;
            }

            string json = req.downloadHandler.text;
            IngredientsRoot root;
            try
            {
                root = JsonUtility.FromJson<IngredientsRoot>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"IngredientSearchController: JSON non valido. {e.Message}");
                yield break;
            }

            if (root?.ingredients == null)
                yield break;

            _all.AddRange(root.ingredients.Where(i => i != null && !string.IsNullOrWhiteSpace(i.id)));
        }
    }

    public void OnSearchChanged(string query)
    {
        _lastQuery = query ?? "";
        Refresh(_lastQuery);
    }

    private void Refresh(string query)
    {
        if (resultsParent == null || rowPrefab == null)
            return;

        ClearChildren(resultsParent);
        RectTransform contentRT = resultsParent as RectTransform;
        RectTransform prefabRT = rowPrefab.GetComponent<RectTransform>();
        float rowHeight = prefabRT != null ? prefabRT.rect.height : 110f;

        string q = (query ?? "").Trim().ToLowerInvariant();

        IEnumerable<IngredientEntry> filtered = _all;
        if (!string.IsNullOrEmpty(q))
        {
            filtered = filtered.Where(i =>
                (!string.IsNullOrEmpty(i.name) && i.name.ToLowerInvariant().Contains(q)) ||
                (!string.IsNullOrEmpty(i.type) && i.type.ToLowerInvariant().Contains(q)) ||
                (!string.IsNullOrEmpty(i.subtype) && i.subtype.ToLowerInvariant().Contains(q)) ||
                (!string.IsNullOrEmpty(i.id) && i.id.ToLowerInvariant().Contains(q)));
        }

        filtered = filtered
            .OrderBy(i => i.type ?? "")
            .ThenBy(i => i.subtype ?? "")
            .ThenBy(i => i.name ?? "");

        int shown = 0;
        int laidOutIndex = 0;
        foreach (IngredientEntry ing in filtered)
        {
            if (shown >= maxResults) break;

            bool alreadyOwned = InventoryManager.Instance != null && InventoryManager.Instance.HasBottle(ing.id);
            if (hideAlreadyOwned && alreadyOwned) continue;

            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(resultsParent, worldPositionStays: false);

            // Supporto prefab "Ingrediente" (root = Button, figli = "Text (TMP)", "Text (TMP) (1)", ...)
            TextMeshProUGUI nameText =
                row.transform.Find("TxtName")?.GetComponent<TextMeshProUGUI>() ??
                row.transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();

            TextMeshProUGUI typeText =
                row.transform.Find("TxtType")?.GetComponent<TextMeshProUGUI>() ??
                row.transform.Find("Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>();

            TextMeshProUGUI emojiText =
                row.transform.Find("TxtEmoji")?.GetComponent<TextMeshProUGUI>(); // nel prefab Ingrediente non c'è

            // Se non esiste un bottone figlio, usa il Button sul root del prefab
            Button addBtn =
                row.transform.Find("BtnAdd")?.GetComponent<Button>() ??
                row.GetComponent<Button>();

            if (nameText) nameText.text = string.IsNullOrEmpty(ing.name) ? ing.id : ing.name;
            if (typeText) typeText.text = CategoryLabel(ing);
            if (emojiText) emojiText.text = EmojiFor(ing);

            if (addBtn)
            {
                addBtn.interactable = !alreadyOwned;
                IngredientEntry captured = ing;
                addBtn.onClick.RemoveAllListeners();
                addBtn.onClick.AddListener(() =>
                {
                    if (InventoryManager.Instance == null) return;
                    InventoryManager.Instance.AddBottle(new Bottle(
                        captured.id,
                        string.IsNullOrEmpty(captured.name) ? captured.id : captured.name,
                        CategoryLabel(captured),
                        EmojiFor(captured),
                        1f
                    ));
                    Refresh(searchInput ? searchInput.text : "");
                });
            }

            // Layout manuale: impila una riga sotto l'altra (senza Layout Group)
            RectTransform rowRT = row.GetComponent<RectTransform>();
            if (rowRT != null)
            {
                rowRT.anchorMin = new Vector2(0f, 1f);
                rowRT.anchorMax = new Vector2(1f, 1f);
                rowRT.pivot = new Vector2(0.5f, 1f);
                rowRT.anchoredPosition = new Vector2(0f, -laidOutIndex * (rowHeight + manualRowSpacing));
                // Mantieni l'altezza del prefab; estendi in larghezza
                rowRT.sizeDelta = new Vector2(0f, rowHeight);
            }

            shown++;
            laidOutIndex++;
        }

        // Aggiorna l'altezza del Content per lo scroll
        if (contentRT != null)
        {
            float totalH = Mathf.Max(0f, laidOutIndex * (rowHeight + manualRowSpacing) - manualRowSpacing);
            contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, totalH);
        }
    }

    private static string CategoryLabel(IngredientEntry ing)
    {
        if (ing == null) return "Altro";
        string t = (ing.type ?? "").ToLowerInvariant();
        return t switch
        {
            "spirit" => "Distillato",
            "bitter" => "Aperitivo/Amaro",
            "liqueur" => "Liquore",
            "wine" => "Vino",
            "juice" => "Succo",
            "mixer" => "Mixer",
            "sweetener" => "Dolcificante",
            "herb" => "Erbe/Spice",
            "other" => "Altro",
            _ => string.IsNullOrEmpty(ing.type) ? "Altro" : ing.type
        };
    }

    private static string EmojiFor(IngredientEntry ing)
    {
        if (ing == null) return "➕";
        string t = (ing.type ?? "").ToLowerInvariant();
        string st = (ing.subtype ?? "").ToLowerInvariant();

        if (t == "spirit")
        {
            if (st.Contains("gin")) return "🌿";
            if (st.Contains("vodka")) return "🫧";
            if (st.Contains("rum")) return "🫚";
            if (st.Contains("tequila") || st.Contains("mezcal")) return "🌵";
            if (st.Contains("whiskey") || st.Contains("whisky") || st.Contains("bourbon") || st.Contains("rye")) return "🥃";
            if (st.Contains("brandy") || st.Contains("cognac")) return "🍇";
            return "🍶";
        }

        if (t == "bitter") return "🌹";
        if (t == "liqueur") return "🍊";
        if (t == "wine") return "🍷";
        if (t == "juice") return "🧃";
        if (t == "mixer") return "🥤";
        if (t == "sweetener") return "🍯";
        if (t == "herb") return "🌱";
        return "➕";
    }

    private static void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    [Serializable]
    private class IngredientsRoot
    {
        public List<IngredientEntry> ingredients;
    }

    [Serializable]
    private class IngredientEntry
    {
        public string id;
        public string name;
        public string type;
        public string subtype;
    }
}

