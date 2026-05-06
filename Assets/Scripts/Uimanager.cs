

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;   // TextMeshPro — assicurati di averlo importato dal Package Manager

// ============================================================
//  MixLab — UIManager.cs
//  Gestisce la navigazione tra le schermate dell'app.
//  Collega i dati (InventoryManager, CocktailDatabase) all'UI.
//
//  USO:
//  1. Crea un Canvas in Unity (UI > Canvas).
//  2. Crea 4 pannelli figli: PanelHome, PanelInventory,
//     PanelDetail, PanelAdd.
//  3. Aggiungi questo script a un GameObject "UIManager".
//  4. Trascina i pannelli nei campi dell'Inspector.
// ============================================================

public class UIManager : MonoBehaviour
{
    private const string ResourcesImagesFolder = "IngredientsImages";

    [Header("--- Home (opzioni lista) ---")]
    [Tooltip("Se true: mostra solo i cocktail realizzabili con l'inventario. Se false: mostra tutto il catalogo.")]
    [SerializeField] private bool showOnlyMakeableCocktails = false;

    // =========================================================
    //  RIFERIMENTI AI PANNELLI (assegna nell'Inspector)
    // =========================================================

    [Header("--- Pannelli principali ---")]
    [SerializeField] private GameObject panelHome;
    [SerializeField] private GameObject panelInventory;
    [SerializeField] private GameObject panelDetail;
    [SerializeField] private GameObject panelAdd;

    // ── Home Screen ───────────────────────────────────────────
    [Header("--- Home ---")]
    [SerializeField] private TextMeshProUGUI txtBottleCount;    // "8"
    [SerializeField] private TextMeshProUGUI txtCocktailCount;  // "24"
    [SerializeField] private TextMeshProUGUI txtMocktailCount;  // "6"
    [SerializeField] private Transform      cocktailListParent; // contenitore scroll list
    [SerializeField] private GameObject     cocktailCardPrefab; // prefab card cocktail

    [Header("--- Home (inventario preview) ---")]
    [Tooltip("Contenitore (es. griglia/orizzontale) per mostrare alcune bottiglie nella Home.")]
    [SerializeField] private Transform homeBottlePreviewParent;
    [Tooltip("Prefab card bottiglia per la preview Home. Se vuoto usa `bottleCardPrefab`.")]
    [SerializeField] private GameObject homeBottleCardPrefab;
    [SerializeField] private int homeBottlePreviewMax = 8;
    [SerializeField] private bool debugHomeInventoryPreview = false;

    // ── Inventory Screen ──────────────────────────────────────
    [Header("--- Inventario ---")]
    [SerializeField] private Transform  bottleGridParent;   // griglia bottiglie
    [SerializeField] private GameObject bottleCardPrefab;   // prefab card bottiglia

    // ── Detail Screen ─────────────────────────────────────────
    [Header("--- Dettaglio Ricetta ---")]
    [SerializeField] private TextMeshProUGUI txtDetailName;
    [SerializeField] private TextMeshProUGUI txtDetailEmoji;
    [SerializeField] private TextMeshProUGUI txtDetailTagline;
    [SerializeField] private TextMeshProUGUI txtDetailDifficulty;
    [SerializeField] private TextMeshProUGUI txtDetailTime;
    [SerializeField] private Transform       ingredientListParent;
    [SerializeField] private GameObject      ingredientRowPrefab;
    [SerializeField] private Transform       stepListParent;
    [SerializeField] private GameObject      stepRowPrefab;
    [SerializeField] private Button          btnTutorial;

    // ── Add Screen ────────────────────────────────────────────
    [Header("--- Aggiungi Bottiglia ---")]
    [SerializeField] private TMP_InputField  searchInput;
    [SerializeField] private Transform       spiritsListParent;
    [SerializeField] private GameObject      spiritRowPrefab;

    [Header("--- Immagini ingredienti (Resources) ---")]
    [Tooltip("Fallback se non troviamo lo sprite in Resources/IngredientsImages/<id>.")]
    [SerializeField] private Sprite fallbackIngredientSprite;

    // =========================================================
    //  STATO INTERNO
    // =========================================================

    private GameObject _currentPanel;        // pannello attivo
    private Cocktail   _selectedCocktail;    // cocktail aperto nel dettaglio
    private bool       _tutorialOpen = false;

    // Catalogo spirits per la schermata Add (dati statici)
    private readonly List<Bottle> _spiritCatalog = new List<Bottle>
    {
        new Bottle("gin_london_dry",   "Gin London Dry",       "Distillato", "🌿"),
        new Bottle("gin_hendricks",    "Hendrick's Gin",        "Distillato", "🌿"),
        new Bottle("gin_monkey47",     "Monkey 47",             "Distillato", "🌿"),
        new Bottle("whisky_scotch",    "Whisky Scotch",         "Distillato", "🥃"),
        new Bottle("whisky_bourbon",   "Bulleit Bourbon",       "Distillato", "🥃"),
        new Bottle("rum_bianco",       "Rum Bianco",            "Distillato", "🫚"),
        new Bottle("rum_diplomatico",  "Diplomatico Rum",       "Distillato", "🫚"),
        new Bottle("vodka_belvedere",  "Belvedere Vodka",       "Distillato", "🫧"),
        new Bottle("aperol",           "Aperol",                "Aperitivo",  "🍊"),
        new Bottle("campari",          "Campari",               "Aperitivo",  "🌹"),
        new Bottle("limoncello",       "Limoncello",            "Liquore",    "🍋"),
        new Bottle("cointreau",        "Cointreau",             "Liquore",    "🍊"),
        new Bottle("vermouth_rosso",   "Vermouth Rosso Carpano","Vino",       "🍷"),
        new Bottle("prosecco",         "Prosecco DOC",          "Vino",       "🍾"),
    };

    // =========================================================
    //  LIFECYCLE
    // =========================================================

    private void Awake()
    {
        // Garantisce la presenza dei manager necessari anche se non sono stati messi in scena
        EnsureManager<InventoryManager>("InventoryManager");
        EnsureManager<CocktailDatabase>("CocktailDatabase");
    }

    private void Start()
    {
        // Ascolta i cambiamenti all'inventario per aggiornare l'UI
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += RefreshHome;
            InventoryManager.Instance.OnInventoryChanged += RefreshInventory;
        }

        ShowHome(); // apre sempre l'app sulla Home
    }

    private void OnDestroy()
    {
        // Rimuove i listener quando l'oggetto viene distrutto
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshHome;
            InventoryManager.Instance.OnInventoryChanged -= RefreshInventory;
        }
    }

    // =========================================================
    //  NAVIGAZIONE — chiamate dai pulsanti dell'UI
    // =========================================================

    public void ShowHome()
    {
        SwitchPanel(panelHome);
        RefreshHome();
    }

    public void ShowInventory()
    {
        SwitchPanel(panelInventory);
        RefreshInventory();
    }

    public void ShowDetail(Cocktail cocktail)
    {
        _selectedCocktail = cocktail;
        SwitchPanel(panelDetail);
        RefreshDetail();
    }

    public void ShowAdd()
    {
        SwitchPanel(panelAdd);
        RefreshAdd("");
    }

    public void GoBack()
    {
        // Torna alla Home (puoi implementare uno stack di navigazione
        // più sofisticato in una versione futura)
        ShowHome();
    }

    // Attiva il pannello richiesto, nasconde tutti gli altri
    private void SwitchPanel(GameObject target)
    {
        if (panelHome != null) panelHome.SetActive(false);
        if (panelInventory != null) panelInventory.SetActive(false);
        if (panelDetail != null) panelDetail.SetActive(false);
        if (panelAdd != null) panelAdd.SetActive(false);

        if (target == null)
        {
            Debug.LogError("UIManager.SwitchPanel: target panel è null. Assegna i pannelli nell'Inspector.");
            return;
        }

        target.SetActive(true);
        _currentPanel = target;
    }

    // =========================================================
    //  REFRESH — aggiornano i contenuti di ogni schermata
    // =========================================================

    // ── HOME ──────────────────────────────────────────────────
    private void RefreshHome()
    {
        if (CocktailDatabase.Instance == null)
        {
            Debug.LogError("UIManager: CocktailDatabase mancante in scena.");
            return;
        }

        // Aggiorna le statistiche nell'hero banner
        int bottleCount = InventoryManager.Instance != null ? InventoryManager.Instance.Count : 0;
        if (txtBottleCount) txtBottleCount.text = bottleCount.ToString();

        // Se non abbiamo inventory, i "makeable" saranno 0: evitiamo di far sembrare l'app rotta
        if (txtCocktailCount)
            txtCocktailCount.text = InventoryManager.Instance != null
                ? CocktailDatabase.Instance.MakeableCount.ToString()
                : CocktailDatabase.Instance.AllCocktails.Count.ToString();

        if (txtMocktailCount)
            txtMocktailCount.text = InventoryManager.Instance != null
                ? CocktailDatabase.Instance.MocktailCount.ToString()
                : "0";

        RefreshHomeBottlePreview();

        // Ripopola la lista cocktail
        if (cocktailListParent == null || cocktailCardPrefab == null) return;

        ClearChildren(cocktailListParent);

        List<Cocktail> list = showOnlyMakeableCocktails && InventoryManager.Instance != null
            ? CocktailDatabase.Instance.GetMakeableCocktails()
            : new List<Cocktail>(CocktailDatabase.Instance.AllCocktails);

        if (list.Count == 0)
        {
            // Nessun cocktail disponibile — mostra messaggio vuoto
            // (puoi aggiungere un TextMeshPro "vuoto" nell'Inspector)
            Debug.Log("UIManager: nessun cocktail da mostrare.");
            return;
        }

        foreach (Cocktail cocktail in list)
        {
            GameObject card = Instantiate(cocktailCardPrefab, cocktailListParent);

            // Cerca i componenti nella card (supporta prefab diversi: es. `Ricetta.prefab`)
            TextMeshProUGUI nameText =
                card.transform.Find("TxtName")?.GetComponent<TextMeshProUGUI>() ??
                card.transform.Find("Text (TMP) (3)")?.GetComponent<TextMeshProUGUI>() ??
                card.transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();

            TextMeshProUGUI metaText =
                card.transform.Find("TxtMeta")?.GetComponent<TextMeshProUGUI>() ??
                card.transform.Find("Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>();

            TextMeshProUGUI emojiText =
                card.transform.Find("TxtEmoji")?.GetComponent<TextMeshProUGUI>() ??
                card.transform.Find("TxtEmoji/Text (TMP)")?.GetComponent<TextMeshProUGUI>();

            if (nameText)  nameText.text  = cocktail.displayName;
            if (emojiText) emojiText.text = cocktail.emoji;
            if (metaText)
            {
                // Costruisce la stringa "Gin · Campari · Vermouth"
                List<string> labels = new List<string>();
                foreach (var ing in cocktail.RequiredIngredients()) labels.Add(ing.label);
                metaText.text = string.Join(" · ", labels);
            }

            // Assegna il click alla card: apre il dettaglio di questo cocktail
            Cocktail captured = cocktail; // closure capture
            Button btn = card.GetComponent<Button>();
            if (btn) btn.onClick.AddListener(() => ShowDetail(captured));
        }
    }

    private void RefreshHomeBottlePreview()
    {
        if (homeBottlePreviewParent == null)
        {
            if (debugHomeInventoryPreview)
                Debug.Log("UIManager(HomePreview): homeBottlePreviewParent NON assegnato → skip.");
            return;
        }

        ClearChildren(homeBottlePreviewParent);

        if (InventoryManager.Instance == null)
        {
            if (debugHomeInventoryPreview)
                Debug.Log("UIManager(HomePreview): InventoryManager.Instance null → skip.");
            return;
        }

        GameObject prefab = homeBottleCardPrefab != null ? homeBottleCardPrefab : bottleCardPrefab;
        if (prefab == null)
        {
            if (debugHomeInventoryPreview)
                Debug.Log("UIManager(HomePreview): nessun prefab bottiglia assegnato (homeBottleCardPrefab/bottleCardPrefab) → skip.");
            return;
        }

        if (debugHomeInventoryPreview)
            Debug.Log($"UIManager(HomePreview): bottiglie in inventario = {InventoryManager.Instance.Bottles.Count} (max {homeBottlePreviewMax}).");

        int shown = 0;
        foreach (Bottle bottle in InventoryManager.Instance.Bottles)
        {
            if (shown >= Mathf.Max(0, homeBottlePreviewMax))
                break;

            GameObject card = Instantiate(prefab, homeBottlePreviewParent);

            // Supporta prefab `Alcolico` + altri
            TextMeshProUGUI nameText =
                card.transform.Find("TxtName")?.GetComponent<TextMeshProUGUI>() ??
                card.transform.Find("Nome/Text (TMP)")?.GetComponent<TextMeshProUGUI>() ??
                card.transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();

            TextMeshProUGUI typeText =
                card.transform.Find("TxtType")?.GetComponent<TextMeshProUGUI>() ??
                card.transform.Find("Nome/Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>() ??
                card.transform.Find("Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>();

            TextMeshProUGUI emojiText = card.transform.Find("TxtEmoji")?.GetComponent<TextMeshProUGUI>();
            Slider fillBar = card.transform.Find("FillBar")?.GetComponent<Slider>();

            if (nameText) nameText.text = bottle.displayName;
            if (typeText) typeText.text = bottle.category;
            if (emojiText) emojiText.text = bottle.emoji;
            if (fillBar) fillBar.value = bottle.fillLevel;

            // Immagine (nel prefab `Alcolico` si chiama "Immagine")
            TryAssignIngredientImage(card.transform, bottle.id);

            // In Home non serve un pulsante "add": se esiste lo disabilitiamo
            Button addBtn =
                card.transform.Find("BtnAdd")?.GetComponent<Button>() ??
                card.GetComponent<Button>();
            if (addBtn != null && addBtn.gameObject.name == "BtnAdd")
                addBtn.gameObject.SetActive(false);

            BottleCardController controller = card.GetComponent<BottleCardController>();
            if (controller) controller.Initialize(bottle.id, bottle.displayName);

            shown++;
        }
    }

    private static void EnsureManager<T>(string goName) where T : MonoBehaviour
    {
        if (typeof(T) == typeof(InventoryManager))
        {
            if (InventoryManager.Instance != null) return;
        }
        else if (typeof(T) == typeof(CocktailDatabase))
        {
            if (CocktailDatabase.Instance != null) return;
        }
        else
        {
            // fallback: se esiste già in scena, non crearne un altro
            if (FindAnyObjectByType<T>() != null) return;
        }

        GameObject go = new GameObject(goName);
        go.AddComponent<T>();
    }

    // ── INVENTARIO ────────────────────────────────────────────
    private void RefreshInventory()
    {
        if (bottleGridParent == null || bottleCardPrefab == null) return;
        ClearChildren(bottleGridParent);

        foreach (Bottle bottle in InventoryManager.Instance.Bottles)
        {
            GameObject card = Instantiate(bottleCardPrefab, bottleGridParent);

            TextMeshProUGUI nameText  = card.transform.Find("TxtName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI typeText  = card.transform.Find("TxtType")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI emojiText = card.transform.Find("TxtEmoji")?.GetComponent<TextMeshProUGUI>();
            Slider          fillBar   = card.transform.Find("FillBar")?.GetComponent<Slider>();

            if (nameText)  nameText.text   = bottle.displayName;
            if (typeText)  typeText.text   = bottle.category;
            if (emojiText) emojiText.text  = bottle.emoji;
            if (fillBar)   fillBar.value   = bottle.fillLevel;

            // Logica doppio tap per rimuovere
            BottleCardController controller = card.GetComponent<BottleCardController>();
            if (controller) controller.Initialize(bottle.id, bottle.displayName);
        }
    }

    // ── DETTAGLIO ─────────────────────────────────────────────
    private void RefreshDetail()
    {
        if (_selectedCocktail == null) return;

        if (txtDetailName)       txtDetailName.text       = _selectedCocktail.displayName;
        if (txtDetailEmoji)      txtDetailEmoji.text      = _selectedCocktail.emoji;
        if (txtDetailTagline)    txtDetailTagline.text    = _selectedCocktail.tagline;
        if (txtDetailTime)       txtDetailTime.text       = $"⏱ {_selectedCocktail.prepTimeMinutes} min";
        if (txtDetailDifficulty) txtDetailDifficulty.text = DifficultyLabel(_selectedCocktail.difficulty);

        // Ingredienti
        if (ingredientListParent && ingredientRowPrefab)
        {
            ClearChildren(ingredientListParent);
            foreach (Ingredient ing in _selectedCocktail.ingredients)
            {
                GameObject row = Instantiate(ingredientRowPrefab, ingredientListParent);
                TextMeshProUGUI ingName = row.transform.Find("TxtName")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI ingQty  = row.transform.Find("TxtQty")?.GetComponent<TextMeshProUGUI>();
                if (ingName) { ingName.text = ing.label; if (ing.isOptional) ingName.color = new Color(1,1,1,0.4f); }
                if (ingQty)  { ingQty.text  = ing.quantity; if (ing.isOptional) ingQty.color = new Color(1,1,1,0.4f); }
            }
        }

        // Passi del tutorial (nascosti di default)
        _tutorialOpen = false;
        if (stepListParent) stepListParent.gameObject.SetActive(false);
        if (btnTutorial)
        {
            TextMeshProUGUI btnTxt = btnTutorial.GetComponentInChildren<TextMeshProUGUI>();
            if (btnTxt) btnTxt.text = "▶ Inizia Tutorial";
            btnTutorial.onClick.RemoveAllListeners();
            btnTutorial.onClick.AddListener(ToggleTutorial);
        }
    }

    public void ToggleTutorial()
    {
        if (_selectedCocktail == null) return;
        _tutorialOpen = !_tutorialOpen;

        if (stepListParent) stepListParent.gameObject.SetActive(_tutorialOpen);

        if (_tutorialOpen && stepListParent && stepRowPrefab)
        {
            ClearChildren(stepListParent);
            int stepNum = 1;
            foreach (string step in _selectedCocktail.steps)
            {
                GameObject row = Instantiate(stepRowPrefab, stepListParent);
                TextMeshProUGUI numText  = row.transform.Find("TxtNum")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI stepText = row.transform.Find("TxtStep")?.GetComponent<TextMeshProUGUI>();
                if (numText)  numText.text  = stepNum.ToString("D2");
                if (stepText) stepText.text = step;
                stepNum++;
            }
        }

        TextMeshProUGUI btnLabel = btnTutorial?.GetComponentInChildren<TextMeshProUGUI>();
        if (btnLabel) btnLabel.text = _tutorialOpen ? "■ Chiudi Tutorial" : "▶ Inizia Tutorial";
    }

    // ── ADD ───────────────────────────────────────────────────
    private void RefreshAdd(string filter)
    {
        if (spiritsListParent == null || spiritRowPrefab == null) return;
        ClearChildren(spiritsListParent);

        string f = filter.ToLower();
        foreach (Bottle spirit in _spiritCatalog)
        {
            // Filtra per testo se l'utente sta cercando
            if (!string.IsNullOrEmpty(f) &&
                !spirit.displayName.ToLower().Contains(f) &&
                !spirit.category.ToLower().Contains(f)) continue;

            // Non mostrare bottiglie già nell'inventario
            bool alreadyOwned = InventoryManager.Instance.HasBottle(spirit.id);

            GameObject row = Instantiate(spiritRowPrefab, spiritsListParent);
            TextMeshProUGUI nameText  = row.transform.Find("TxtName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI typeText  = row.transform.Find("TxtType")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI emojiText = row.transform.Find("TxtEmoji")?.GetComponent<TextMeshProUGUI>();
            Button          addBtn    = row.transform.Find("BtnAdd")?.GetComponent<Button>();

            if (nameText)  nameText.text  = spirit.displayName;
            if (typeText)  typeText.text  = spirit.category;
            if (emojiText) emojiText.text = spirit.emoji;

            // Immagine (nel prefab `Alcolico` si chiama "Immagine")
            TryAssignIngredientImage(row.transform, spirit.id);

            if (addBtn)
            {
                addBtn.interactable = !alreadyOwned;
                Bottle captured = spirit;
                addBtn.onClick.AddListener(() => {
                    InventoryManager.Instance.AddBottle(new Bottle(
                        captured.id, captured.displayName, captured.category, captured.emoji, 1f));
                    RefreshAdd(searchInput ? searchInput.text : "");
                });
            }
        }
    }

    // Collegalo al campo di ricerca (OnValueChanged)
    public void OnSearchChanged(string value) => RefreshAdd(value);

    // =========================================================
    //  UTILITY
    // =========================================================

    // Rimuove tutti i figli di un Transform (svuota una lista/griglia)
    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    private string DifficultyLabel(Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy:   return "Facile";
            case Difficulty.Medium: return "Medio";
            case Difficulty.Hard:   return "Difficile";
            default:                return "";
        }
    }

    private void TryAssignIngredientImage(Transform rowTransform, string ingredientId)
    {
        if (rowTransform == null || string.IsNullOrWhiteSpace(ingredientId))
            return;

        Image img =
            rowTransform.Find("ImgIngredient")?.GetComponent<Image>() ??
            rowTransform.Find("Immagine")?.GetComponent<Image>() ??
            rowTransform.Find("Image")?.GetComponent<Image>();

        if (img == null)
            return;

        string resourcePath = $"{ResourcesImagesFolder}/{ingredientId}";
        Sprite s = Resources.Load<Sprite>(resourcePath) ?? fallbackIngredientSprite;
        if (s == null)
            return;

        img.sprite = s;
        img.preserveAspect = true;
        img.enabled = true;
    }
}