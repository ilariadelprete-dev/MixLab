using System.Collections.Generic;
using UnityEngine;
// ============================================================
//  MixLab — CocktailDatabase.cs
//  Catalogo statico di tutti i cocktail supportati.
//  Include anche la logica per calcolare quali cocktail
//  sono realizzabili con l'inventario corrente.
//
//  USO: Aggiungi questo script allo stesso GameObject di
//       InventoryManager, oppure a uno separato.
//       Accessibile con CocktailDatabase.Instance
// ============================================================

public class CocktailDatabase : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────
    public static CocktailDatabase Instance { get; private set; }

    // ── Catalogo completo ─────────────────────────────────────
    private List<Cocktail> _allCocktails = new List<Cocktail>();

    public IReadOnlyList<Cocktail> AllCocktails => _allCocktails.AsReadOnly();

    // =========================================================
    //  LIFECYCLE
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildDatabase(); // popola il catalogo
    }

    // =========================================================
    //  COSTRUZIONE DEL CATALOGO
    //  Aggiungi qui nuovi cocktail seguendo lo stesso pattern.
    // =========================================================

    private void BuildDatabase()
    {
        _allCocktails.Clear();

        // ── Negroni ───────────────────────────────────────────
        var negroni = new Cocktail(
            id:              "negroni",
            displayName:     "Negroni",
            emoji:           "🍸",
            tagline:         "L'aperitivo italiano per eccellenza",
            difficulty:      Difficulty.Medium,
            prepTimeMinutes: 3,
            hasMocktail:     true
        );
        negroni.ingredients.Add(new Ingredient("gin_london_dry",     "Gin",           "3 cl"));
        negroni.ingredients.Add(new Ingredient("campari",            "Campari",        "3 cl"));
        negroni.ingredients.Add(new Ingredient("vermouth_rosso",     "Vermouth rosso", "3 cl"));
        negroni.ingredients.Add(new Ingredient("arancia",            "Scorza arancia", "q.b.", isOptional: true));
        negroni.steps.AddRange(new[] {
            "Riempi un mixing glass di ghiaccio fino all'orlo.",
            "Versa 3 cl di Gin, 3 cl di Campari e 3 cl di Vermouth rosso.",
            "Mescola delicatamente per 30 secondi con un bar spoon.",
            "Filtra in un bicchiere Old Fashioned con un grosso cubo di ghiaccio.",
            "Guarnisci con una scorza d'arancia sprizzata sul bordo."
        });
        _allCocktails.Add(negroni);

        // ── Aperol Spritz ─────────────────────────────────────
        var spritz = new Cocktail(
            id:              "aperol_spritz",
            displayName:     "Aperol Spritz",
            emoji:           "🍊",
            tagline:         "L'aperitivo veneto conquistatore del mondo",
            difficulty:      Difficulty.Easy,
            prepTimeMinutes: 2,
            hasMocktail:     false
        );
        spritz.ingredients.Add(new Ingredient("aperol",             "Aperol",         "6 cl"));
        spritz.ingredients.Add(new Ingredient("prosecco",           "Prosecco DOC",   "9 cl"));
        spritz.ingredients.Add(new Ingredient("soda",               "Soda water",     "3 cl"));
        spritz.ingredients.Add(new Ingredient("arancia",            "Fetta d'arancia","1 pz", isOptional: true));
        spritz.steps.AddRange(new[] {
            "Riempi un calice grande di ghiaccio.",
            "Versa prima il Prosecco, poi l'Aperol, infine la soda.",
            "Mescola brevemente con il cucchiaino.",
            "Guarnisci con una fetta d'arancia."
        });
        _allCocktails.Add(spritz);

        // ── Whisky Sour ───────────────────────────────────────
        var whiskySour = new Cocktail(
            id:              "whisky_sour",
            displayName:     "Whisky Sour",
            emoji:           "🥃",
            tagline:         "Equilibrio perfetto tra dolce e acido",
            difficulty:      Difficulty.Medium,
            prepTimeMinutes: 4,
            hasMocktail:     false
        );
        whiskySour.ingredients.Add(new Ingredient("whisky_scotch",  "Whisky",         "5 cl"));
        whiskySour.ingredients.Add(new Ingredient("succo_limone",   "Succo di limone","3 cl"));
        whiskySour.ingredients.Add(new Ingredient("sciroppo",       "Sciroppo di zucchero","2 cl"));
        whiskySour.ingredients.Add(new Ingredient("albume",         "Albume d'uovo",  "1 pz", isOptional: true));
        whiskySour.steps.AddRange(new[] {
            "Metti tutti gli ingredienti nello shaker senza ghiaccio.",
            "Agita vigorosamente per 10 secondi (dry shake).",
            "Aggiungi il ghiaccio e agita di nuovo per 15 secondi.",
            "Filtra in un bicchiere rocks con ghiaccio fresco.",
            "Guarnisci con una fetta di limone o una ciliegina."
        });
        _allCocktails.Add(whiskySour);

        // ── Mojito ────────────────────────────────────────────
        var mojito = new Cocktail(
            id:              "mojito",
            displayName:     "Mojito",
            emoji:           "🍹",
            tagline:         "Il cocktail più fresco dell'estate",
            difficulty:      Difficulty.Easy,
            prepTimeMinutes: 3,
            hasMocktail:     true
        );
        mojito.ingredients.Add(new Ingredient("rum_bianco",         "Rum bianco",     "5 cl"));
        mojito.ingredients.Add(new Ingredient("succo_lime",         "Succo di lime",  "3 cl"));
        mojito.ingredients.Add(new Ingredient("sciroppo",           "Sciroppo di zucchero","2 cl"));
        mojito.ingredients.Add(new Ingredient("menta",              "Foglie di menta","10 pz"));
        mojito.ingredients.Add(new Ingredient("soda",               "Soda water",     "q.b."));
        mojito.steps.AddRange(new[] {
            "Metti le foglie di menta e lo sciroppo in un bicchiere highball.",
            "Pestare delicatamente con il muddler per rilasciare gli oli.",
            "Aggiungi il succo di lime e il rum.",
            "Riempi di ghiaccio tritato e completa con soda.",
            "Mescola brevemente e guarnisci con un rametto di menta."
        });
        _allCocktails.Add(mojito);

        // ── Limoncello Spritz ─────────────────────────────────
        var limonSpritz = new Cocktail(
            id:              "limoncello_spritz",
            displayName:     "Limoncello Spritz",
            emoji:           "🍋",
            tagline:         "La freschezza del Sud Italia in un bicchiere",
            difficulty:      Difficulty.Easy,
            prepTimeMinutes: 2,
            hasMocktail:     false
        );
        limonSpritz.ingredients.Add(new Ingredient("limoncello",    "Limoncello",     "4 cl"));
        limonSpritz.ingredients.Add(new Ingredient("prosecco",      "Prosecco",       "8 cl"));
        limonSpritz.ingredients.Add(new Ingredient("soda",          "Soda water",     "2 cl"));
        limonSpritz.ingredients.Add(new Ingredient("arancia",       "Fetta di limone","1 pz", isOptional: true));
        limonSpritz.steps.AddRange(new[] {
            "Riempi un calice di ghiaccio.",
            "Versa il limoncello, poi il Prosecco e la soda.",
            "Mescola delicatamente.",
            "Guarnisci con una fetta di limone."
        });
        _allCocktails.Add(limonSpritz);

        Debug.Log($"CocktailDatabase: caricati {_allCocktails.Count} cocktail.");
    }

    // =========================================================
    //  LOGICA DI SUGGERIMENTO
    // =========================================================

    // Restituisce tutti i cocktail realizzabili con l'inventario attuale
    public List<Cocktail> GetMakeableCocktails()
    {
        List<Bottle> inventory = new List<Bottle>(InventoryManager.Instance.Bottles);
        return _allCocktails.FindAll(c => c.IsMakeable(inventory));
    }

    // Restituisce tutti i cocktail realizzabili filtrati per difficoltà
    public List<Cocktail> GetMakeableByDifficulty(Difficulty difficulty)
    {
        return GetMakeableCocktails().FindAll(c => c.difficulty == difficulty);
    }

    // Restituisce solo i cocktail con versione analcolica disponibile
    public List<Cocktail> GetMakeableMocktails()
    {
        return GetMakeableCocktails().FindAll(c => c.hasMocktail);
    }

    // Cerca un cocktail per id (es. "negroni")
    public Cocktail GetById(string id)
    {
        return _allCocktails.Find(c => c.id == id);
    }

    // Conta quanti cocktail sono realizzabili ora
    public int MakeableCount => GetMakeableCocktails().Count;

    // Conta quanti analcolici sono realizzabili ora
    public int MocktailCount => GetMakeableMocktails().Count;
}