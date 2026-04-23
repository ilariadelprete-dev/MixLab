using System;
using System.Collections.Generic;
// ============================================================
//  MixLab — Cocktail.cs
//  Rappresenta una ricetta cocktail con ingredienti e metadati.
// ============================================================
 
// ── Livello di difficoltà ─────────────────────────────────────
public enum Difficulty { Easy, Medium, Hard }
 
// ── Un singolo ingrediente nella ricetta ─────────────────────
[Serializable]
public class Ingredient
{
    public string bottleId;   // deve corrispondere a Bottle.id
    public string label;      // nome leggibile, es. "Gin"
    public string quantity;   // dose leggibile, es. "3 cl"
    public bool   isOptional; // true = guarnizione o tocco finale
 
    public Ingredient(string bottleId, string label, string quantity, bool isOptional = false)
    {
        this.bottleId   = bottleId;
        this.label      = label;
        this.quantity   = quantity;
        this.isOptional = isOptional;
    }
}
 
// ── Ricetta cocktail completa ─────────────────────────────────
[Serializable]
public class Cocktail
{
    public string            id;              // es. "negroni"
    public string            displayName;     // es. "Negroni"
    public string            emoji;           // es. "🍸"
    public string            tagline;         // sottotitolo breve
    public Difficulty        difficulty;
    public int               prepTimeMinutes;
    public bool              hasMocktail;     // disponibile versione analcolica
    public List<Ingredient>  ingredients;     // lista ingredienti (obbligatori + opzionali)
    public List<string>      steps;           // passi del tutorial
 
    // ── Costruttore ───────────────────────────────────────────
    public Cocktail(string id, string displayName, string emoji, string tagline,
                    Difficulty difficulty, int prepTimeMinutes, bool hasMocktail)
    {
        this.id              = id;
        this.displayName     = displayName;
        this.emoji           = emoji;
        this.tagline         = tagline;
        this.difficulty      = difficulty;
        this.prepTimeMinutes = prepTimeMinutes;
        this.hasMocktail     = hasMocktail;
        this.ingredients     = new List<Ingredient>();
        this.steps           = new List<string>();
    }
 
    // ── Metodi di supporto ────────────────────────────────────
 
    // Restituisce solo gli ingredienti NON opzionali
    public List<Ingredient> RequiredIngredients()
    {
        return ingredients.FindAll(i => !i.isOptional);
    }
 
    // Controlla se questo cocktail è fattibile con le bottiglie date.
    // Itera ogni ingrediente obbligatorio e verifica che esista
    // una bottiglia corrispondente con fillLevel > 0.
    public bool IsMakeable(List<Bottle> inventory)
    {
        foreach (Ingredient ing in RequiredIngredients())
        {
            // Cerca una bottiglia nell'inventario con l'id corretto e non vuota
            Bottle found = inventory.Find(b => b.id == ing.bottleId && b.IsAvailable);
            if (found == null)
                return false; // ingrediente mancante → cocktail non fattibile
        }
        return true;
    }
 
    public override string ToString() => $"{emoji} {displayName} ({difficulty})";
}