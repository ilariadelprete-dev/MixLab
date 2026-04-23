

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

    private void Start()
    {
        // Ascolta i cambiamenti all'inventario per aggiornare l'UI
        InventoryManager.Instance.OnInventoryChanged += RefreshHome;
        InventoryManager.Instance.OnInventoryChanged += RefreshInventory;

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
        panelHome.SetActive(false);
        panelInventory.SetActive(false);
        panelDetail.SetActive(false);
        panelAdd.SetActive(false);

        target.SetActive(true);
        _currentPanel = target;
    }

    // =========================================================
    //  REFRESH — aggiornano i contenuti di ogni schermata
    // =========================================================

    // ── HOME ──────────────────────────────────────────────────
    private void RefreshHome()
    {
        // Aggiorna le statistiche nell'hero banner
        if (txtBottleCount)   txtBottleCount.text   = InventoryManager.Instance.Count.ToString();
        if (txtCocktailCount) txtCocktailCount.text  = CocktailDatabase.Instance.MakeableCount.ToString();
        if (txtMocktailCount) txtMocktailCount.text  = CocktailDatabase.Instance.MocktailCount.ToString();

        // Ripopola la lista cocktail
        if (cocktailListParent == null || cocktailCardPrefab == null) return;

        ClearChildren(cocktailListParent);

        List<Cocktail> makeable = CocktailDatabase.Instance.GetMakeableCocktails();

        if (makeable.Count == 0)
        {
            // Nessun cocktail disponibile — mostra messaggio vuoto
            // (puoi aggiungere un TextMeshPro "vuoto" nell'Inspector)
            Debug.Log("UIManager: nessun cocktail realizzabile con l'inventario corrente.");
            return;
        }

        foreach (Cocktail cocktail in makeable)
        {
            GameObject card = Instantiate(cocktailCardPrefab, cocktailListParent);

            // Cerca i componenti nella card (dipende dal tuo prefab)
            TextMeshProUGUI nameText  = card.transform.Find("TxtName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI metaText  = card.transform.Find("TxtMeta")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI emojiText = card.transform.Find("TxtEmoji")?.GetComponent<TextMeshProUGUI>();

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
}