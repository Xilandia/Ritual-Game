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
            Debug.Log($"Grab range: {CasterWillRange}, now grabbing.");
            // Grab and fill mana
            FillMana(GrabMana());
            // Convert and fill mana
            Debug.Log($"Converting mana.");
            FillMana(ManaConverter.Instance.ConvertMana(manaMissing, caster.GetConversionCapacity() * 10,
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
                else
                {
                    Debug.Log($"{kvp.Key} obtained.");
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

    private Dictionary<ManaType, int> GrabMana()
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

                if (!grabbedMana.ContainsKey(type))
                    grabbedMana[type] = 0;

                int taken = container.ExtractMana(type, needed - grabbedMana[type], origin);
                if (taken > 0)
                {
                    grabbedMana[type] += taken;
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

        return grabbedMana;
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

                Debug.Log($"Charging: {type} by {amount}, missing {manaMissing[type]} out of {manaCosts[type]} needed ({manaContained[type]} contained)");
            }
        }
    }
}
