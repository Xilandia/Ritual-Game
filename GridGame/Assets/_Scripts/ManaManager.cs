using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    [SerializeField] private List<ManaContainer> ManaContainers = new List<ManaContainer>();
    [SerializeField] private List<ManaVisualizer> ManaVisualizers = new List<ManaVisualizer>();
    [SerializeField] private int manaGenerationRate = 2; // How much mana is generated per Tile per tick
    [SerializeField] private int manaGeneratedCount = 0; // How many tiles around the tile can generate mana

    private bool isManaTransition = false;
    private float timeSinceStart = 0f;

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

    public void AddManaVisualizer(ManaVisualizer manaVisualizer)
    {
        ManaVisualizers.Add(manaVisualizer);
    }

    public void RemoveManaVisualizer(ManaVisualizer manaVisualizer)
    {
        ManaVisualizers.Remove(manaVisualizer);
    }

    public void PerformManaPhase()
    {
        DebugManaGeneration();

        foreach (ManaContainer mc in ManaContainers)
        {
            mc.UpdateManaFlow();
        }

        foreach (ManaContainer mc in ManaContainers)
        {
            mc.IncrementManaFlow();
        }

        isManaTransition = true;
    }

    void Update()
    {
        if (isManaTransition)
        {
            timeSinceStart += Time.deltaTime;

            if (timeSinceStart >= 1f)
            {
                isManaTransition = false;
                timeSinceStart = 0f;

                foreach (ManaContainer mc in ManaContainers)
                {
                    mc.HandleEvents();
                }

                InitiativeManager.Instance.NextPhase();
            }
            else
            {
                foreach (ManaVisualizer mv in ManaVisualizers)
                {
                    mv.ProgressParticleTransition(timeSinceStart);
                }
            }
        }
    }

    void DebugManaGeneration()
    {
        for (int x = 0; x <= GridManager.Instance.width; x++)
        {
            ManaParticle mp = new ManaParticle
            {
                type = (ManaType) (manaGeneratedCount % 11),
                quantity = manaGenerationRate,
                particleX = x,
                particleY = GridManager.Instance.height,
                velocity = new Vector2(0, 0),
                prevOrb = -1,
                nextOrb = 0
            };
            GridManager.Instance.GetTile(x, GridManager.Instance.height).AddMana(mp);
            manaGeneratedCount++;
        }
    }
}