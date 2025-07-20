using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualCostOrder : MonoBehaviour
{
    [SerializeField] private ActiveRitual ritual;
    [SerializeField] private Player caster;

    public void StartChargingRitual(Dictionary<ManaType, int> manaCost)
    {
        ritual.PlugInOrder(manaCost);
    }

    public void RitualCharged()
    {
        // Perform the ritual
        Debug.Log("Ritual Charged");
    }
}
