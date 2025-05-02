using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRitual : MonoBehaviour
{
    [SerializeField] private ICharacter caster;
    [SerializeField] private RitualCostOrder costOrder;
    [SerializeField] private Dictionary<ManaType, int> manaCosts = new Dictionary<ManaType, int>();
    [SerializeField] private Dictionary<ManaType, int> manaContained = new Dictionary<ManaType, int>();
    [SerializeField] private Dictionary<ManaType, int> manaMissing = new Dictionary<ManaType, int>();

    [SerializeField] private int CasterWillRange;

    private bool active = false;

    public void RegisterRitual(ICharacter caster)
    {
        this.caster = caster;
        ManaManager.Instance.AddActiveRitual(this);
    }

    public void PlugInOrder(Dictionary<ManaType, int> manaCostsDictionary)
    {
        if (active)
        {
            Debug.Log("Ritual already active, double ritual behavior unimplemented.");
        }
        else
        {
            active = true;
            CasterWillRange = 0;

            foreach (var kvp in manaCostsDictionary)
            {
                ManaType type = kvp.Key;
                int cost = kvp.Value;

                manaCosts[type] = cost;
                manaContained[type] = 0;
                manaMissing[type] = cost;
            }
        }
    }

    public void ChargeRitual()
    {
        if (active)
        {
            // Process tick
            CasterWillRange++;
            // Grab and fill mana
            GrabMana();
            // Convert and fill mana
            FillMana(ManaConverter.Instance.ConvertMana(manaMissing, caster.GetConversionCapacity(),
                caster.GetCurrentTile()));
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
                active = false;
                costOrder.RitualCharged();
            }
        }
    }

    private void GrabMana()
    {
        Dictionary<ManaType, int> grabbedMana = new Dictionary<ManaType, int>();

        Tile origin = caster.GetCurrentTile();
        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<(Tile tile, int dist)> frontier = new Queue<(Tile, int)>();

        frontier.Enqueue((origin, 0));
        visited.Add(origin);

        while (frontier.Count > 0)
        {
            var (tile, dist) = frontier.Dequeue();

            if (dist > CasterWillRange) continue;

            ManaContainer container = tile.GetManaContainer();
            foreach (var kvp in manaMissing)
            {
                if (kvp.Value <= 0) continue;

                ManaType type = kvp.Key;
                int needed = kvp.Value;

                int taken = container.ExtractMana(type, needed, origin);
                if (taken > 0)
                {
                    if (!grabbedMana.ContainsKey(type))
                        grabbedMana[type] = 0;
                    grabbedMana[type] += taken;
                    Debug.Log($"I took {taken} of type {type} from {container}");
                }
            }

            // Enqueue neighbors
            foreach (Tile neighbor in tile.GetNeighborTiles())
            {
                if (!visited.Contains(neighbor))
                {
                    frontier.Enqueue((neighbor, dist + 1));
                    visited.Add(neighbor);
                }
            }
        }

        FillMana(grabbedMana);
        // Moves mana of relevant types from further away (2x CasterWillRange) closer to the caster - maybe? Need to decide if and how to implement this
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
