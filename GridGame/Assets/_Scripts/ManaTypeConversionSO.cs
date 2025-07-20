using UnityEngine;

[System.Serializable]
public struct ManaTypeConversionEdge
{
    public ManaType[] startTypes;
    public int[] quantity;
    public float efficiency;
}

[CreateAssetMenu(menuName = "Mana/ManaTypeConversion")]
public class ManaTypeConversionSO : ScriptableObject
{
    public ManaType manaType;               // The source type
    public ManaTypeConversionEdge[] conversionEdges; // Types that can be converted to this mana type
}