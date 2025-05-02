using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OddsMaker : MonoBehaviour
{
    public static OddsMaker Instance { get; private set; }

    private List<ManaContainer> neighborContainers = new List<ManaContainer>();
    private List<Tile> neighborTiles = new List<Tile>();
    private Tile currentTile;

    private void Awake()
    {
        Instance = this;
    }

    public Tile DecideTarget(ManaParticle mp, Tile tile)
    {
        // 1) Get the relation for this particle's type
        if (!ManaTypeRelationDB.Instance.Dict.TryGetValue(mp.type, out var relation))
        {
            // Fallback: uniform random
            var fallback = tile.GetNeighborTiles();
            return fallback[Random.Range(0, fallback.Count)];
        }

        if (currentTile == null || tile != currentTile) // if first run or moving to new tile, get neighbors
        {
            currentTile = tile;
            neighborContainers = tile.GetNeighborManaContainers();
            neighborTiles = tile.GetNeighborTiles();
        }

        // 3) Prepare weights
        float[] dirWeights = new float[8];
        for (int i = 0; i < 8; i++)
        {
            // Baseline direction bias: cardinal (even indices) more likely than diagonal
            dirWeights[i] = (i % 2 == 0) ? 1.4f : 1.0f;

            // Aggregate the neighboring tile's mana
            var agg = neighborContainers[i].GetAggregatedMana();

            // Add attraction for positive types
            foreach (var p in relation.positiveTypes)
                if (agg.TryGetValue(p.type, out int q))
                    dirWeights[i] += p.weight * q;

            // Subtract repulsion for negative types
            foreach (var n in relation.negativeTypes)
                if (agg.TryGetValue(n.type, out int q2))
                    dirWeights[i] -= n.weight * q2;

            dirWeights[i] += neighborContainers[i].GetManaGap() * 2;

            // Clamp so we never go negative
            dirWeights[i] = Mathf.Max(0, dirWeights[i]);
        }

        // 4) Randomly pick one direction by weighted sampling
        float sum = 0;
        foreach (var w in dirWeights) sum += w;
        float r = Random.value * sum;

        int chosen = 0;
        for (int i = 0; i < 8; i++)
        {
            r -= dirWeights[i];
            if (r <= 0)
            {
                chosen = i;
                break;
            }
        }

        // 5) Return the tile in that direction
        return neighborTiles[chosen];
    }

}
