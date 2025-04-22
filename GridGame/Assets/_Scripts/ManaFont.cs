using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaFont : MonoBehaviour, ManaFeature
{
    private Tile tile;
    private ManaType mainType;
    private int mainThreshold;

    public void Init(Tile iTile, int type, int threshold)
    {
        tile = iTile;
        mainType = (ManaType) type;
        mainThreshold = threshold;
    }

    public void ExecutePhase()
    {
        int randomRoll = Random.Range(0, 100);
        int quantity = Random.Range(1, 7);
        ManaType type;

        if (randomRoll < mainThreshold)
        {
            type = mainType;
        }
        else if (randomRoll < (100 - mainThreshold / 2))
        {
            type = ManaType.Neutral;
        }
        else
        {
            type = (ManaType) Random.Range(0, 11);
        }

        ManaParticle mp = new ManaParticle
        {
            type = type,
            quantity = quantity > 4 ? 4 : quantity,
            particleAge = 0,
            particleX = tile.coordX,
            particleY = tile.coordY,
            inertia = 1,
            velocity = new Vector2(0, 0),
            prevOrb = -1,
            nextOrb = 0
        };

        tile.AddMana(mp);
    }
}
