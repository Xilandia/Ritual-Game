using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRitual : MonoBehaviour
{
    private ICharacter caster;
    private RitualCostOrder costOrder;

    private Dictionary<ManaType, int> manaCosts = new Dictionary<ManaType, int>();
    private Dictionary<ManaType, int> manaContained = new Dictionary<ManaType, int>();
    private Dictionary<ManaType, int> manaMissing = new Dictionary<ManaType, int>();

    private int CasterWillRange;

    public void Init(ICharacter caster, RitualCostOrder costOrder, Dictionary<ManaType, int> manaCostsDictionary)
    {
        this.caster = caster;
        this.costOrder = costOrder;
        PlugInOrder(manaCostsDictionary);
        ManaManager.Instance.AddActiveRitual(this);
    }

    private void PlugInOrder(Dictionary<ManaType, int> manaCostsDictionary)
    {
        foreach (var kvp in manaCostsDictionary)
        {
            ManaType type = kvp.Key;
            int cost = kvp.Value;

            manaCosts[type] = cost;
            manaContained[type] = 0;
            manaMissing[type] = cost;
        }
    }

    public void ChargeRitual()
    {
        // Process tick
        CasterWillRange++;
        // Grab mana
        GrabMana();
        // Convert mana
        Dictionary<ManaType, int> manaConverted = ManaConverter.Instance.ConvertMana(manaMissing, caster.GetConversionCapacity(), caster.GetCurrentTile());
        // Fill mana
        FillMana(manaConverted);
        // Check if ritual is charged
        bool isCharged = true;

        foreach (var kvp in manaMissing)
        {
            if (kvp.Value > 0)
            {
                isCharged = false;
                break;
            }
        }

        if (isCharged)
        {
            // Ritual is charged, perform ritual
            //costOrder.RitualCharged();
        }
    }

    private void GrabMana()
    {
        // based on range, grab mana from tiles around the caster, as much as possible
        // this method does not handle conversion, so it only looks for mana of the exact types
        Tile currentTile = caster.GetCurrentTile();
        Dictionary<ManaType, int> manaGrabbed = new Dictionary<ManaType, int>();
        // Looks at the tiles around the caster with a radius of CasterWillRange
        // Grabs matching mana from the closest tiles first
        // Calls a TBD method to visualize the mana grab
        FillMana(manaGrabbed);
        // Moves mana of relevant types from further away (2x CasterWillRange) closer to the caster
    }

    private void FillMana(Dictionary<ManaType, int> manaConverted)
    {
        foreach (var kvp in manaConverted)
        {
            ManaType type = kvp.Key;
            int amount = kvp.Value;
            if (manaContained.ContainsKey(type))
            {
                manaContained[type] += amount;
                manaMissing[type] -= amount;
            }
        }
    }
}
