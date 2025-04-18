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
    private List<ManaParticle> outgoingParticles = new List<ManaParticle>();
    private int manaCount;
    private int manaVolume; // separate for aspect calculation
    private bool manaAdded;

    void Start()
    {
        softCap = baseSoftCap; // TODO: Initialize soft cap based on aspects.
        ManaManager.Instance.AddManaContainer(this, tile);
    }

    public int AddMana(ManaParticle newParticle)
    {
        manaCount += newParticle.quantity;
        manaVolume += newParticle.quantity;
        manaAdded = true;

        for (int i = 0; i < incomingParticles.Count; i++)
        {
            if (incomingParticles[i].type == newParticle.type)
            {
                ManaParticle mergedParticle = incomingParticles[i];
                mergedParticle.quantity += newParticle.quantity;
                incomingParticles[i] = mergedParticle;
                return i;
            }
        }

        newParticle.nextOrb = incomingParticles.Count;
        incomingParticles.Add(newParticle);
        return incomingParticles.Count - 1;
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

    public void CheckManaCap()
    {
        /*for (int i = 0; i < manaParticles.Count; i++)
        {
            ManaParticle mp = ManaFlowCalculator.Instance.CalculateManaFlow(tile, this, manaParticles[i]);
            manaParticles[i] = mp;
        }*/

        if (manaAdded && manaVolume > softCap)
        {
            float pBase = (float) (manaVolume - softCap) / manaCount;

            for (int i = 0; i < manaParticles.Count; i++)
            {
                ManaParticle mp = manaParticles[i];

                int leaving = 0;

                for (int u = 0; u < mp.quantity; u++)
                {
                    if (Random.value < pBase) // Add mana type relations here (to pBase)
                        leaving++;
                }

                if (leaving > 0)
                {
                    mp.quantity -= leaving;
                    manaParticles[i] = mp;
                    
                    ManaParticle ejected = mp;
                    ejected.quantity = leaving;

                    TransferManaParticle(ejected);
                }
            }
        }
    }

    public void IncrementManaFlow()
    {
        foreach (ManaParticle mp in outgoingParticles)
        {
            manaVisualizer.PrepareVisualization(mp);
        }

        outgoingParticles.Clear();
    }

    public void HandleEvents()
    {
        // Handle other events
        for (int i = manaParticles.Count - 1; i >= 0; i--)
        {
            if (manaParticles[i].quantity == 0)
            {
                manaParticles.RemoveAt(i);
            }
        }

        foreach (ManaParticle mp in incomingParticles)
        {
            manaParticles.Add(mp);
        }

        manaVisualizer.EndTransition(manaParticles);
        incomingParticles.Clear();
        manaAdded = false;
    }

    public void TransferManaParticle(ManaParticle particle)
    {
        manaCount -= particle.quantity;
        manaVolume -= particle.quantity;
        Tile target = OddsMaker.Instance.DecideTarget(particle, tile);

        particle.particleX = target.coordX;
        particle.particleY = target.coordY;
        particle.nextOrb = target.AddMana(particle);

        outgoingParticles.Add(particle);
    }
}
