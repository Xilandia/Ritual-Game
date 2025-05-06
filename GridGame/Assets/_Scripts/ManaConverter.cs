using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaConverter : MonoBehaviour
{
    private int ConversionRange = 2;

    public static ManaConverter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Dictionary<ManaType, int> ConvertMana(Dictionary<ManaType, int> manaRequired, int conversionCapacity, Tile casterTile)
    {
        Dictionary<ManaType, int> manaConverted = new Dictionary<ManaType, int>();
        int capacityUsed = 0;

        var nearbyTiles = GridManager.Instance.GetTilesInRadius(ConversionRange, casterTile);
        var casterContainer = casterTile.GetManaContainer();

        // Step 1: Collect all valid recipe executions (tile, recipe, targetType)
        // Step 2: Sort candidates by descending score
        var candidates = GetCandidates(manaRequired, nearbyTiles);

        // Step 3: Execute top recipes greedily
        var (accumulated, leftoverConverted) = ExecuteBestConversions(
            candidates, manaRequired, nearbyTiles, ref capacityUsed, conversionCapacity
        );

        // Step 5: Commit integer mana and deposit leftovers
        foreach (var kvp in accumulated)
        {
            ManaType type = kvp.Key;
            int whole = Mathf.FloorToInt(kvp.Value);
            if (whole > 0) manaConverted[type] = whole;
        }

        foreach (var kvp in leftoverConverted)
        {
            ManaParticle mp = new ManaParticle
            {
                type = kvp.Key,
                quantity = Mathf.CeilToInt(kvp.Value),
                particleX = casterTile.coordX,
                particleY = casterTile.coordY
            };
            casterContainer.AddMana(mp);
        }

        return manaConverted;
    }

    private List<(Tile tile, ManaContainer container, ManaType targetType, ManaTypeConversionEdge edge, float score)>
        GetCandidates(Dictionary<ManaType, int> manaRequired, List<Tile> nearbyTiles)
    {
        var candidates = new List<(Tile tile, ManaContainer container, ManaType targetType, ManaTypeConversionEdge edge, float score)>();

        foreach (var targetKvp in manaRequired)
        {
            ManaType targetType = targetKvp.Key;
            int missing = targetKvp.Value;
            if (missing <= 0) continue;

            if (!ManaTypeConversionDB.Instance.Dict.TryGetValue(targetType, out var conversionSO))
                continue;

            foreach (var edge in conversionSO.conversionEdges)
            {
                foreach (var tile in nearbyTiles)
                {
                    var container = tile.GetManaContainer();
                    if (container == null) continue;

                    // Check if tile can satisfy all recipe inputs
                    bool valid = true;
                    for (int i = 0; i < edge.startTypes.Length; i++)
                    {
                        if (container.GetQuantityOf(edge.startTypes[i]) < edge.quantity[i])
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        float score = edge.efficiency * missing;
                        candidates.Add((tile, container, targetType, edge, score));
                    }
                }
            }
        }

        // Step 2: Sort candidates by descending score
        candidates.Sort((a, b) => b.score.CompareTo(a.score));

        return candidates;
    }

    private (Dictionary<ManaType, float> accumulated, Dictionary<ManaType, float> leftoverConverted)
        ExecuteBestConversions(
            List<(Tile tile, ManaContainer container, ManaType targetType, ManaTypeConversionEdge edge, float score)> candidates,
            Dictionary<ManaType, int> manaRequired,
            List<Tile> nearbyTiles,
            ref int capacityUsed,
            int conversionCapacity)
    {
        Dictionary<ManaType, float> accumulated = new();
        Dictionary<ManaType, float> leftoverConverted = new();

        // Execute scored recipes greedily
        foreach (var (tile, container, targetType, edge, _) in candidates)
        {
            int totalInput = 0;
            for (int i = 0; i < edge.startTypes.Length; i++)
                totalInput += edge.quantity[i];

            while (conversionCapacity - capacityUsed >= totalInput && manaRequired[targetType] > 0)
            {
                bool canExecute = true;
                for (int i = 0; i < edge.startTypes.Length; i++)
                {
                    if (container.GetQuantityOf(edge.startTypes[i]) < edge.quantity[i])
                    {
                        canExecute = false;
                        break;
                    }
                }
                if (!canExecute) break;

                for (int i = 0; i < edge.startTypes.Length; i++)
                {
                    container.ExtractMana(edge.startTypes[i], edge.quantity[i], tile);
                }

                capacityUsed += totalInput;

                float produced = totalInput * edge.efficiency;
                float deliverable = Mathf.Min(produced, manaRequired[targetType]);

                if (!accumulated.ContainsKey(targetType)) accumulated[targetType] = 0f;
                accumulated[targetType] += deliverable;

                float leftover = produced - deliverable;
                if (leftover > 0f)
                {
                    if (!leftoverConverted.ContainsKey(targetType)) leftoverConverted[targetType] = 0f;
                    leftoverConverted[targetType] += leftover;
                }

                float neutral = totalInput * (1f - edge.efficiency);
                if (neutral > 0f)
                {
                    container.AddMana(new ManaParticle
                    {
                        type = ManaType.Neutral,
                        quantity = Mathf.CeilToInt(neutral),
                        particleX = tile.coordX,
                        particleY = tile.coordY
                    });
                }
            }
        }

        // Fallback to neutral conversion
        foreach (var targetType in manaRequired.Keys)
        {
            while (manaRequired[targetType] > 0 && conversionCapacity - capacityUsed >= 5)
            {
                bool fallbackUsed = false;
                foreach (var tile in nearbyTiles)
                {
                    var container = tile.GetManaContainer();
                    if (container == null) continue;

                    if (container.GetQuantityOf(ManaType.Neutral) >= 5)
                    {
                        container.ExtractMana(ManaType.Neutral, 5, tile);
                        capacityUsed += 5;

                        if (!accumulated.ContainsKey(targetType)) accumulated[targetType] = 0f;
                        accumulated[targetType] += 1f;

                        fallbackUsed = true;
                        break;
                    }
                }
                if (!fallbackUsed) break;
            }
        }

        return (accumulated, leftoverConverted);
    }

}
