using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private float manaVolume; // separate for aspect calculation
    private bool manaAdded;

    void Start()
    {
        softCap = baseSoftCap; // TODO: Initialize soft cap based on aspects.
        ManaManager.Instance.AddManaContainer(this, tile);
    }

    public int AddMana(ManaParticle newParticle)
    {
        manaAdded = true;

        for (int i = 0; i < incomingParticles.Count; i++)
        {
            if (incomingParticles[i].type == newParticle.type)
            {
                ManaParticle mergedParticle = incomingParticles[i];
                mergedParticle.quantity += newParticle.quantity;
                incomingParticles[i] = mergedParticle;
                return mergedParticle.nextOrb;
            }
        }

        int nextOrb = -1;

        for (int i = 0; i < manaParticles.Count; i++)
        {
            if (manaParticles[i].type == newParticle.type)
            {
                nextOrb = i;
                break;
            }
        }

        if (nextOrb == -1)
        {
            nextOrb = incomingParticles.Count + 1;
        }

        newParticle.nextOrb = nextOrb;

        if (newParticle.prevOrb == -1)
        {
            newParticle.prevOrb = newParticle.nextOrb;
        }

        incomingParticles.Add(newParticle);

        return nextOrb;
    }

    public Dictionary<ManaType, float> GetAggregatedMana() // Format might not work with what I have in mind, will update later
    {
        Dictionary<ManaType, float> aggregated = new Dictionary<ManaType, float>();
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
        if (!(manaAdded && manaVolume > softCap))
            return;

        float pBase = (float)(manaVolume - softCap) / manaCount;
        Dictionary<int, int> ejectedParticles = new Dictionary<int, int>();
        var agg = GetAggregatedMana();

        for (int i = 0; i < manaParticles.Count; i++)
        {
            ManaParticle mp = manaParticles[i];
            
            float p = pBase;
            float weightModifier = 0f;
            
            if (ManaTypeRelationDB.Instance.Dict.TryGetValue(mp.type, out var relation))
            {
                foreach (var pos in relation.positiveTypes)
                    if (agg.TryGetValue(pos.type, out float qPos))
                        weightModifier -= pos.weight / 5 * qPos;
                
                foreach (var neg in relation.negativeTypes)
                    if (agg.TryGetValue(neg.type, out float qNeg))
                        weightModifier += neg.weight / 5 * qNeg;
            }

            p *= (1 + weightModifier / 100);
            p = Mathf.Clamp(p, 0.03f, 1f);
            
            int leaving = 0;

            for (int u = 0; u < mp.quantity; u++)
                if (Random.value < p)
                    leaving++;

            if (leaving > 0)
                ejectedParticles.Add(i, leaving);
        }

        foreach (KeyValuePair<int, int> kvp in ejectedParticles)
        {
            ManaParticle mp = manaParticles[kvp.Key];
            float remainder = 0;

            mp.quantity -= kvp.Value;

            if (mp.quantity < 0)
            {
                remainder = mp.quantity;
                mp.quantity = 0;
            }

            manaCount -= kvp.Value;
            manaVolume -= kvp.Value + remainder;
            manaParticles[kvp.Key] = mp;

            ManaParticle ejected = mp;
            ejected.quantity = kvp.Value;
            TransferManaParticle(ejected);
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

        if (incomingParticles.Count == 0)
        {
            manaAdded = false;
        }

        foreach (ManaParticle mp in incomingParticles)
        {
            bool alreadyAdded = false;

            for (int i = 0; i < manaParticles.Count; i++)
            {
                if (manaParticles[i].type == mp.type)
                {
                    ManaParticle mergedParticle = manaParticles[i];
                    mergedParticle.quantity += mp.quantity;
                    manaParticles[i] = mergedParticle;
                    alreadyAdded = true;
                    break;
                }
            }

            if (!alreadyAdded)
            {
                manaParticles.Add(mp);
            }
        }

        manaCount = 0;
        manaVolume = 0;

        foreach (ManaParticle mp in manaParticles)
        {
            manaCount += Mathf.CeilToInt(mp.quantity);
            manaVolume += mp.quantity;
        }

        manaVisualizer.EndTransition(manaParticles);
        incomingParticles.Clear();
    }

    public void TransferManaParticle(ManaParticle particle)
    {
        Tile target = OddsMaker.Instance.DecideTarget(particle, tile);

        particle.particleX = target.coordX;
        particle.particleY = target.coordY;
        particle.prevOrb = particle.nextOrb;
        particle.nextOrb = target.AddMana(particle);

        outgoingParticles.Add(particle);
    }

    public int ExtractMana(ManaType type, int amount, Tile tile)
    {
        for (int i = 0; i < manaParticles.Count; i++)
        {
            if (manaParticles[i].type == type)
            {
                int available = Mathf.FloorToInt(manaParticles[i].quantity);
                int taken = Mathf.Min(available, amount);

                ManaParticle mp = manaParticles[i];
                mp.quantity -= taken;
                manaParticles[i] = mp;
                manaCount -= taken;
                manaVolume -= taken;
                //visualize extraction of mana
                return taken;
            }
        }
        return 0;
    }

    public int GetManaGap()
    {
        return Mathf.Max(0, softCap - manaCount);
    }
}
