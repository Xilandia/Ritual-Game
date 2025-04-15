using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaContainer : MonoBehaviour
{
    public int baseSoftCap = 20;
    public int softCap; // Adjusted soft cap based on aspects.

    [SerializeField] private Tile tile;
    [SerializeField] private ManaVisualizer manaVisualizer;
    
    private List<ManaParticle> manaParticles = new List<ManaParticle>();
    private List<ManaParticle> incomingParticles = new List<ManaParticle>();

    void Start()
    {
        softCap = baseSoftCap; // TODO: Initialize soft cap based on aspects.
        ManaManager.Instance.AddManaContainer(this);
    }

    public int AddMana(ManaParticle newParticle)
    {
        incomingParticles.Add(newParticle);
        return incomingParticles.Count;
    }

    public bool RemoveMana(ManaParticle particle)
    {
        return manaParticles.Remove(particle);
    }

    public Dictionary<ManaType, int> GetAggregatedMana() // Format might not work with what I have in mind, will update later
    {
        Dictionary<ManaType, int> aggregated = new Dictionary<ManaType, int>();
        foreach (var mp in manaParticles)
        {
            if (aggregated.ContainsKey(mp.type))
                aggregated[mp.type] += mp.quantity;
            else
                aggregated[mp.type] = mp.quantity;
        }
        return aggregated;
    }

    public void UpdateManaFlow()
    {
        for (int i = 0; i < manaParticles.Count; i++)
        {
            ManaParticle mp = ManaFlowCalculator.Instance.CalculateManaFlow(tile, this, manaParticles[i]);
            manaParticles[i] = mp;
        }
    }

    public void IncrementManaFlow()
    {
        /*foreach (ManaParticle mp in manaParticles)
        {
            manaVisualizer.PrepareVisualization(TransferManaParticle(mp, GridManager.Instance.GetTile(mp.particleX, mp.particleY)));
        }*/
        for (int i = manaParticles.Count - 1; i >= 0; i--)
        {
            ManaParticle mp = manaParticles[i];
            manaVisualizer.PrepareVisualization(TransferManaParticle(mp, GridManager.Instance.GetTile(mp.particleX, mp.particleY)));
        }
    }

    public void HandleEvents()
    {
        // Handle overflow based on probability - soft cap
        // Handle other events

        foreach (ManaParticle mp in incomingParticles)
        {
            manaParticles.Add(mp);
        }

        manaVisualizer.EndTransition(manaParticles);
        incomingParticles.Clear();
    }

    public ManaParticle TransferManaParticle(ManaParticle particle, Tile target)
    {
        if (RemoveMana(particle))
        {
            particle.nextOrb = target.AddMana(particle);
        }

        return particle;
    }
}
