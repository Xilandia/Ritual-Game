using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPopulator : MonoBehaviour
{
    [SerializeField] private ItemReader reader;

    public IItem PopulateItem()
    {
        Katana katana = new Katana("Katana");
        
        List<int> stats = reader.GetStats();
        int[,] damageGrid = reader.GetDamageGrid();

        InputItemStats(katana, stats);
        InputDamageGrid(katana, damageGrid);

        return katana;
    }

    void InputItemStats(Katana katana, List<int> stats)
    {
        katana.attackWidth = stats[0];
        katana.attackHeight = stats[1];
        katana.holderCoordX = stats[2];
        katana.holderCoordY = stats[3];
        katana.baseDamage = stats[4];
        katana.maxDurability = stats[5];
        katana.durability = katana.maxDurability;
    }

    void InputDamageGrid(Katana katana, int[,] damageGrid)
    {
        katana.damageMap = damageGrid;
        katana.hitMap = new bool[katana.attackHeight, katana.attackWidth];

        for (int i = 0; i < katana.attackHeight; i++)
        {
            for (int j = 0; j < katana.attackWidth; j++)
            {
                if (damageGrid[i, j] > 0)
                {
                    katana.hitMap[i, j] = true;
                }
            }
        }
    }
}
