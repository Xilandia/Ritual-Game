using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPopulator : MonoBehaviour
{
    [SerializeField] private ItemReader reader;

    public IItem PopulateItem(string itemName)
    {
        Weapon weapon = new Weapon(itemName);
        
        List<int> stats = reader.GetStats();
        int[,] damageGrid = reader.GetDamageGrid();

        InputItemStats(weapon, stats);
        InputDamageGrid(weapon, damageGrid);

        return weapon;
    }

    void InputItemStats(Weapon weapon, List<int> stats)
    {
        weapon.attackWidth = stats[0];
        weapon.attackHeight = stats[1];
        weapon.holderCoordX = stats[2];
        weapon.holderCoordY = stats[3];
        weapon.baseDamage = stats[4];
        weapon.maxDurability = stats[5];
        weapon.durability = weapon.maxDurability;
    }

    void InputDamageGrid(Weapon weapon, int[,] damageGrid)
    {
        weapon.damageMap = damageGrid;
        weapon.hitMap = new bool[weapon.attackWidth, weapon.attackHeight];

        for (int i = 0; i < weapon.attackWidth; i++)
        {
            for (int j = 0; j < weapon.attackHeight; j++)
            {
                if (damageGrid[i, j] > 0)
                {
                    weapon.hitMap[i, j] = true;
                }
            }
        }
    }
}
