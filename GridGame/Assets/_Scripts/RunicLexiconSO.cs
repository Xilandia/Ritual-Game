using System.Collections.Generic;
using UnityEngine;

public enum RuneSlot
{
    ManaType,
    Shape,
    Modifier,
    Effect
}

[System.Serializable]
public struct ManaRequirement
{
    public ManaType type;

    [Min(0)]
    public int amount;

    public override string ToString()
    {
        return $"{type}: {amount}";
    }
}

[System.Serializable]
public struct RunicLexiconEntry
{
    public string internalRuneName; // Hidden name of the rune
    public string internalDescription; // Hidden description of the rune
    // public Sprite icon; // To be implemented later
    public int runeId; // Unique ID for the rune
    public RuneSlot runeSlot; // The slot of the rune (ManaType, Shape, Modifier, Effect)
    public List<ManaRequirement> requiredMana; // Dictionary of required mana types and their quantities
    public List<int> blockedRunes; // List of rune IDs that are blocked by this rune
    public List<int> requiredRunes; // List of rune IDs that are required to use this rune
    // Some "apply" function? 
}

[CreateAssetMenu(menuName = "Runes/New Rune")]
public class RunicLexiconEntrySO : ScriptableObject
{
    public RunicLexiconEntry entry; // The rune entry data
}
