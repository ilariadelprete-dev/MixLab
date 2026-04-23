using System;
using System.Collections.Generic;
using UnityEngine;
// ============================================================
//  MixLab — InventoryManager.cs
//  Gestisce l'inventario delle bottiglie: aggiunta, rimozione,
//  salvataggio su disco (PlayerPrefs + JSON) e notifiche.
//
//  USO: Aggiungi questo script a un GameObject vuoto chiamato
//       "InventoryManager" nella tua scena principale.
//       È un Singleton: accessibile da qualunque script con
//       InventoryManager.Instance
// ============================================================

public class InventoryManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────
    public static InventoryManager Instance { get; private set; }

    // ── Evento: si attiva ogni volta che l'inventario cambia ──
    // Altri script possono "ascoltare" questo evento per
    // aggiornare l'UI automaticamente:
    //   InventoryManager.Instance.OnInventoryChanged += MioMetodo;
    public event Action OnInventoryChanged;

    // ── Dati privati ──────────────────────────────────────────
    private List<Bottle> _inventory = new List<Bottle>();
    private const string SAVE_KEY   = "mixlab_inventory"; // chiave per PlayerPrefs

    // ── Proprietà pubblica (sola lettura) ─────────────────────
    public IReadOnlyList<Bottle> Bottles => _inventory.AsReadOnly();
    public int Count => _inventory.Count;

    // =========================================================
    //  LIFECYCLE UNITY
    // =========================================================

    private void Awake()
    {
        // Implementazione Singleton: un solo InventoryManager
        // sopravvive al cambio scena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadInventory(); // carica dati salvati all'avvio
    }

    // =========================================================
    //  API PUBBLICA
    // =========================================================

    // ── Aggiunge una bottiglia all'inventario ─────────────────
    // Restituisce false se la bottiglia è già presente.
    public bool AddBottle(Bottle bottle)
    {
        if (bottle == null)
        {
            Debug.LogWarning("InventoryManager: tentativo di aggiungere una bottiglia null.");
            return false;
        }

        // Evita duplicati controllando l'id
        if (_inventory.Exists(b => b.id == bottle.id))
        {
            Debug.Log($"InventoryManager: '{bottle.displayName}' è già nell'inventario.");
            return false;
        }

        _inventory.Add(bottle);
        SaveInventory();
        OnInventoryChanged?.Invoke(); // notifica tutti gli ascoltatori
        Debug.Log($"InventoryManager: aggiunto '{bottle.displayName}'.");
        return true;
    }

    // ── Rimuove una bottiglia per id ──────────────────────────
    public bool RemoveBottle(string bottleId)
    {
        Bottle toRemove = _inventory.Find(b => b.id == bottleId);
        if (toRemove == null)
        {
            Debug.LogWarning($"InventoryManager: bottiglia '{bottleId}' non trovata.");
            return false;
        }

        _inventory.Remove(toRemove);
        SaveInventory();
        OnInventoryChanged?.Invoke();
        Debug.Log($"InventoryManager: rimosso '{toRemove.displayName}'.");
        return true;
    }

    // ── Aggiorna il livello di riempimento di una bottiglia ───
    public void SetFillLevel(string bottleId, float newLevel)
    {
        Bottle bottle = _inventory.Find(b => b.id == bottleId);
        if (bottle == null) return;

        bottle.fillLevel = Mathf.Clamp01(newLevel);
        SaveInventory();
        OnInventoryChanged?.Invoke();
    }

    // ── Controlla se una bottiglia è nell'inventario ──────────
    public bool HasBottle(string bottleId)
    {
        return _inventory.Exists(b => b.id == bottleId && b.IsAvailable);
    }

    // ── Restituisce le bottiglie con livello basso ─────────────
    public List<Bottle> GetLowBottles()
    {
        return _inventory.FindAll(b => b.IsLow);
    }

    // ── Restituisce le bottiglie filtrate per categoria ────────
    public List<Bottle> GetByCategory(string category)
    {
        return _inventory.FindAll(b =>
            b.category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    // =========================================================
    //  PERSISTENZA (salvataggio / caricamento)
    // =========================================================

    // Salva l'inventario come JSON in PlayerPrefs
    // (PlayerPrefs è il sistema di salvataggio semplice di Unity)
    private void SaveInventory()
    {
        // Unity non serializza List<T> direttamente in JSON,
        // quindi usiamo un wrapper
        BottleListWrapper wrapper = new BottleListWrapper { bottles = _inventory };
        string json = JsonUtility.ToJson(wrapper, prettyPrint: true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    // Carica l'inventario da PlayerPrefs (se esiste)
    private void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("InventoryManager: nessun salvataggio trovato. Inventario vuoto.");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        try
        {
            BottleListWrapper wrapper = JsonUtility.FromJson<BottleListWrapper>(json);
            _inventory = wrapper.bottles ?? new List<Bottle>();
            Debug.Log($"InventoryManager: caricato {_inventory.Count} bottiglie.");
        }
        catch (Exception e)
        {
            Debug.LogError($"InventoryManager: errore nel caricamento JSON. {e.Message}");
            _inventory = new List<Bottle>();
        }
    }

    // Cancella tutto l'inventario (utile per reset / debug)
    public void ClearInventory()
    {
        _inventory.Clear();
        PlayerPrefs.DeleteKey(SAVE_KEY);
        OnInventoryChanged?.Invoke();
        Debug.Log("InventoryManager: inventario azzerato.");
    }

    // ── Classe wrapper necessaria per JsonUtility ─────────────
    [Serializable]
    private class BottleListWrapper
    {
        public List<Bottle> bottles;
    }
}
