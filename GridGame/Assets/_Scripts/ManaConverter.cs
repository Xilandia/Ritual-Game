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

    public Dictionary<ManaType, int> ConvertMana(Dictionary<ManaType, int> manaRequired, int conversionCapacity, Tile currentTile)
    {
        Dictionary<ManaType, int> manaConverted = new Dictionary<ManaType, int>();
        int conversionCapacityUsed = 0;

        // Converts mana in surrounding tiles (radius of ConversionRange) to mana needed by the ritual, then grabs it immediately
        // Different conversions have different efficiencies, so we need to check the mana type and the conversion efficiencies (retrieve from dictionary)
        // At implementation, we will ignore character efficiency boosts
        // Inputs for conversion: 1 mana of the correct type and 1 conversion capacity
        // Outputs of conversion: 1 times the conversion efficiency of the converted mana type, the remainder stays on the tile as neutral mana (both can and will be decimal values)
        // The conversion efficiency is a float value between 0 and 1, representing the percentage of mana that can be converted
        // The converter will optimize to fill as much mana required as possible, so it will use the conversion capacity as well as it can
        // Seeing as manaConverted is a whole number (rituals use up whole units of mana only)
        // The converter will dump the remainder of the converted mana onto the tile the character is on (neutral mana remains behind).
        // The converter calls a TBD method to visualize mana movement

        return manaConverted;
    }
}
