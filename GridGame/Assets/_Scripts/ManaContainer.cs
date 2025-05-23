﻿using System;
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
    private int manaVolume; // separate for aspect calculation
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
        if (!(manaAdded && manaVolume > softCap))
            return;

        float pBase = (float)(manaVolume - softCap) / manaCount;
        Dictionary<int, int> ejectedParticles = new Dictionary<int, int>();

        for (int i = 0; i < manaParticles.Count; i++)
        {
            ManaParticle mp = manaParticles[i];
            
            float p = pBase;
            float weightModifier = 0f;
            
            if (ManaTypeRelationDB.Instance.Dict.TryGetValue(mp.type, out var relation))
            {
                var agg = GetAggregatedMana();

                foreach (var pos in relation.positiveTypes)
                    if (agg.TryGetValue(pos.type, out int qPos))
                        weightModifier -= pos.weight / 5 * qPos;
                
                foreach (var neg in relation.negativeTypes)
                    if (agg.TryGetValue(neg.type, out int qNeg))
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

            mp.quantity -= kvp.Value;
            manaCount -= kvp.Value;
            manaVolume -= kvp.Value;
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
            manaCount += mp.quantity;
            manaVolume += mp.quantity;
        }

        manaVisualizer.EndTransition(manaParticles);
        incomingParticles.Clear();
    }

    public void TransferManaParticle(ManaParticle particle)
    {
        manaCount -= particle.quantity;
        manaVolume -= particle.quantity;
        Tile target = OddsMaker.Instance.DecideTarget(particle, tile);

        particle.particleX = target.coordX;
        particle.particleY = target.coordY;
        particle.prevOrb = particle.nextOrb;
        particle.nextOrb = target.AddMana(particle);

        outgoingParticles.Add(particle);
    }
}
