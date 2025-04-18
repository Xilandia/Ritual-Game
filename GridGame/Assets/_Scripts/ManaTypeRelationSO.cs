using UnityEngine;

[System.Serializable]
public struct ManaTypeWeight
{
    public ManaType type;
    public float weight;
}

[CreateAssetMenu(menuName = "Mana/ManaTypeRelation")]
public class ManaTypeRelationSO : ScriptableObject
{
    public ManaType manaType;               // The source type
    public ManaTypeWeight[] positiveTypes;  // Types that attract this mana
    public ManaTypeWeight[] negativeTypes;  // Types that repel this mana
}