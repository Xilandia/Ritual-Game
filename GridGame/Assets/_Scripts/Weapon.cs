using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Weapon : IItem
{
    public int attackWidth;
    public int attackHeight;
    public int holderCoordX;
    public int holderCoordY;
    public int baseDamage;
    public int durability;
    public int maxDurability;
    public bool[,] hitMap;
    public int[,] damageMap;

    public string itemName { get; private set; }

    public Weapon(string IitemName)
    {
        itemName = IitemName;
    }

    public bool[,] GetHitMap()
    {
        return hitMap;
    }

    public int[,] GetDamageMap()
    {
        return damageMap;
    }

    public void DebugItem()
    {
        StringBuilder debugMessage = new StringBuilder();

        debugMessage.AppendLine($"Item Name: {itemName}");
        debugMessage.AppendLine($"Attack Width: {attackWidth}");
        debugMessage.AppendLine($"Attack Height: {attackHeight}");
        debugMessage.AppendLine($"Holder X: {holderCoordX}");
        debugMessage.AppendLine($"Holder Y: {holderCoordY}");
        debugMessage.AppendLine($"Base Damage: {baseDamage}");
        debugMessage.AppendLine($"Durability: {durability} / {maxDurability}");

        debugMessage.AppendLine("Hit Map:");
        if (hitMap != null)
        {
            for (int i = 0; i < hitMap.GetLength(0); i++)
            {
                for (int j = 0; j < hitMap.GetLength(1); j++)
                {
                    debugMessage.Append(hitMap[i, j] ? "1 " : "0 ");
                }
                debugMessage.AppendLine();
            }
        }
        else
        {
            debugMessage.AppendLine("Hit Map is null.");
        }

        debugMessage.AppendLine("Damage Map:");
        if (damageMap != null)
        {
            for (int i = 0; i < damageMap.GetLength(0); i++)
            {
                for (int j = 0; j < damageMap.GetLength(1); j++)
                {
                    debugMessage.Append($"{damageMap[i, j]} ");
                }
                debugMessage.AppendLine();
            }
        }
        else
        {
            debugMessage.AppendLine("Damage Map is null.");
        }

        Debug.Log(debugMessage.ToString());
    }
}
