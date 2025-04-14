using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    [SerializeField] private List<ManaContainer> ManaContainers = new List<ManaContainer>();

    public static ManaManager Instance { get; private set; }

    public void Init()
    {
        Instance = this;
        // Populate mana
    }

    public void AddManaContainer(ManaContainer manaContainer)
    {
        ManaContainers.Add(manaContainer);
    }

    public void RemoveManaContainer(ManaContainer manaContainer)
    {
        ManaContainers.Remove(manaContainer);
    }

    public void PerformManaPhase()
    {
        foreach (ManaContainer mc in ManaContainers)
        {
            mc.UpdateManaFlow();
        }

        foreach (ManaContainer mc in ManaContainers)
        {
            mc.IncrementManaFlow();
        }

        foreach (ManaContainer mc in ManaContainers)
        {
            mc.HandleEvents();
        }
    }
}
