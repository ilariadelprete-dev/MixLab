using UnityEngine;
// ============================================================
//  MixLab — AppBootstrapper.cs
//  Punto di avvio dell'app: popola l'inventario con dati
//  di esempio se non c'è nessun salvataggio precedente.
//
//  USO: Aggiungi questo script allo stesso GameObject di
//       InventoryManager. Verrà eseguito DOPO InventoryManager
//       grazie all'ordine di esecuzione di Unity (Start vs Awake).
// ============================================================

public class AppBootstrapper : MonoBehaviour
{
    [Header("Carica dati di esempio al primo avvio?")]
    [SerializeField] private bool loadSampleData = true;

    private void Start()
    {
        // Start() viene chiamato dopo Awake() di tutti gli altri script.
        // InventoryManager ha già caricato i dati salvati in Awake().
        // Se l'inventario è vuoto E l'opzione è attiva, carichiamo i dati demo.

        if (loadSampleData && InventoryManager.Instance.Count == 0)
        {
            LoadSampleInventory();
        }
    }

    private void LoadSampleInventory()
    {
        Debug.Log("AppBootstrapper: caricamento inventario di esempio...");

        InventoryManager.Instance.AddBottle(new Bottle("gin_london_dry",  "Gin London Dry",  "Distillato", "🌿", 0.40f));
        InventoryManager.Instance.AddBottle(new Bottle("whisky_scotch",   "Whisky Scotch",   "Distillato", "🥃", 0.90f));
        InventoryManager.Instance.AddBottle(new Bottle("rum_bianco",      "Rum Bianco",      "Distillato", "🫚", 0.20f));
        InventoryManager.Instance.AddBottle(new Bottle("aperol",          "Aperol",          "Aperitivo",  "🍊", 0.55f));
        InventoryManager.Instance.AddBottle(new Bottle("campari",         "Campari",         "Aperitivo",  "🌹", 0.60f));
        InventoryManager.Instance.AddBottle(new Bottle("limoncello",      "Limoncello",      "Liquore",    "🍋", 0.75f));
        InventoryManager.Instance.AddBottle(new Bottle("vermouth_rosso",  "Vermouth Rosso",  "Vino",       "🍷", 0.50f));
        InventoryManager.Instance.AddBottle(new Bottle("prosecco",        "Prosecco DOC",    "Vino",       "🍾", 0.80f));

        Debug.Log($"AppBootstrapper: aggiunte {InventoryManager.Instance.Count} bottiglie di esempio.");
    }

    // Metodo pubblico per resettare l'app (utile durante lo sviluppo)
    // Puoi chiamarlo da un pulsante "Reset" nell'Inspector con SendMessage
    public void ResetApp()
    {
        InventoryManager.Instance.ClearInventory();
        Debug.Log("AppBootstrapper: app resettata.");
    }
}