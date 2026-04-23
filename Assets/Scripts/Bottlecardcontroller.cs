using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ============================================================
//  MixLab — BottleCardController.cs
//  Gestisce il comportamento della singola card bottiglia
//  nell'inventario: doppio tap per rimuovere.
//
//  USO: Aggiungi questo script al prefab "BottleCard".
// ============================================================

public class BottleCardController : MonoBehaviour
{
    // ── Riferimenti interni al prefab ─────────────────────────
    [SerializeField] private Image            cardBackground;
    [SerializeField] private TextMeshProUGUI  txtType;   // mostra "Rimuovi?" in modalità confirm

    // ── Colori ────────────────────────────────────────────────
    [SerializeField] private Color normalBorderColor  = new Color(1f, 1f, 1f, 0.07f);
    [SerializeField] private Color confirmBorderColor = new Color(0.878f, 0.333f, 0.333f, 0.4f); // rosso

    // ── Stato ─────────────────────────────────────────────────
    private string _bottleId;
    private string _originalTypeText;
    private bool   _confirmMode = false;
    private float  _confirmTimer = 0f;
    private const float CONFIRM_TIMEOUT = 2f; // secondi prima di tornare allo stato normale

    // =========================================================
    //  INIZIALIZZAZIONE
    //  Chiamata da UIManager dopo l'istanziazione
    // =========================================================

    public void Initialize(string bottleId, string displayName)
    {
        _bottleId = bottleId;
        if (txtType) _originalTypeText = txtType.text;

        // Collega il click del pulsante
        Button btn = GetComponent<Button>();
        if (btn) btn.onClick.AddListener(OnTap);
    }

    // =========================================================
    //  LOGICA DOPPIO TAP
    // =========================================================

    private void OnTap()
    {
        if (_confirmMode)
        {
            // Secondo tap → rimuove la bottiglia
            InventoryManager.Instance.RemoveBottle(_bottleId);
            // L'UI si aggiorna automaticamente via OnInventoryChanged
            // (questo GameObject verrà distrutto da UIManager.RefreshInventory)
        }
        else
        {
            // Primo tap → entra in modalità conferma
            EnterConfirmMode();
        }
    }

    private void EnterConfirmMode()
    {
        _confirmMode  = true;
        _confirmTimer = CONFIRM_TIMEOUT;

        // Cambia aspetto visivo della card
        if (cardBackground) cardBackground.color = new Color(0.878f, 0.333f, 0.333f, 0.08f);
        if (txtType)        { txtType.text = "Rimuovi?"; txtType.color = new Color(0.878f, 0.333f, 0.333f, 1f); }
    }

    private void ExitConfirmMode()
    {
        _confirmMode = false;

        // Ripristina aspetto normale
        if (cardBackground) cardBackground.color = Color.clear;
        if (txtType)        { txtType.text = _originalTypeText; txtType.color = new Color(1f, 1f, 1f, 0.38f); }
    }

    // =========================================================
    //  UPDATE — gestisce il timeout della modalità conferma
    // =========================================================

    private void Update()
    {
        if (!_confirmMode) return;

        _confirmTimer -= Time.deltaTime;
        if (_confirmTimer <= 0f)
            ExitConfirmMode();
    }
}