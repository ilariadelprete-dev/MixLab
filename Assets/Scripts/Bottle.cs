using System;
using UnityEngine;
// ============================================================
//  MixLab — Bottle.cs
//  Rappresenta una singola bottiglia nell'inventario.
//  È un semplice "contenitore di dati" (nessuna logica Unity).
// ============================================================

[Serializable]   // permette a Unity di mostrarlo nell'Inspector e salvarlo in JSON
public class Bottle
{
    // ── Campi principali ──────────────────────────────────────
    public string id;          // identificatore unico, es. "gin_london_dry"
    public string displayName; // nome leggibile, es. "Hendrick's Gin"
    public string category;    // "Distillato", "Aperitivo", "Liquore", "Vino"
    public string emoji;       // emoji per l'icona nell'UI, es. "🌿"
    public float  fillLevel;   // livello di riempimento 0.0 – 1.0  (0 = vuoto, 1 = pieno)
 
    // ── Costruttore ───────────────────────────────────────────
    // Ti permette di creare una bottiglia con una sola riga:
    //   new Bottle("gin_london_dry", "Gin London Dry", "Distillato", "🌿", 0.75f)
    public Bottle(string id, string displayName, string category, string emoji, float fillLevel = 1f)
    {
        this.id          = id;
        this.displayName = displayName;
        this.category    = category;
        this.emoji       = emoji;
        this.fillLevel   = Mathf.Clamp01(fillLevel); // garantisce che sia sempre tra 0 e 1
    }
 
    // ── Proprietà di supporto ─────────────────────────────────
    // Restituisce true se la bottiglia ha ancora del liquore
    public bool IsAvailable => fillLevel > 0f;
 
    // Restituisce true se il livello è sotto il 20% (utile per gli avvisi)
    public bool IsLow => fillLevel > 0f && fillLevel < 0.2f;
 
    public override string ToString() => $"{displayName} ({fillLevel * 100f:F0}%)";
}