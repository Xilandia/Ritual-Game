using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    [SerializeField] private List<ManaContainer> ManaContainers = new List<ManaContainer>();
    [SerializeField] private List<ManaVisualizer> ManaVisualizers = new List<ManaVisualizer>();
    [SerializeField] private List<ManaFeature> ManaFeatures = new List<ManaFeature>();
    [SerializeField] private int manaGenerationRate = 6; // How much mana is generated per Tile per tick
    [SerializeField] private int manaGeneratedCount; // Count of mana particles generated for debugging purposes

    private bool isManaTransition = false;
    private float timeSinceStart = 0f;

    private List<int> manaTypes = new List<int>
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
    };

    private List<int> usedManaTypes = new List<int>();

    public static ManaManager Instance { get; private set; }

    public void Init()
    {
        Instance = this;
        // Populate mana
        PopulateManaFonts();
    }

    public void AddManaContainer(ManaContainer manaContainer, Tile tile)
    {
        ManaContainers.Add(manaContainer);

        if (tile.behavior != TileBehavior.Border)
        {
            for (int i = 0; i < 4; i++)
            {
                int randomIndex = Random.Range(0, manaTypes.Count);
                ManaParticle mp = new ManaParticle
                {
                    type = (ManaType)manaTypes[randomIndex],
                    quantity = manaGenerationRate,
                    particleAge = 0,
                    particleX = tile.coordX,
                    particleY = tile.coordY,
                    inertia = 1,
                    velocity = new Vector2(0, 0),
                    prevOrb = -1,
                    nextOrb = 0
                };

                manaContainer.AddMana(mp);
                usedManaTypes.Add(manaTypes[randomIndex]);
                manaTypes.RemoveAt(randomIndex);
            }

            manaTypes.AddRange(usedManaTypes);
            usedManaTypes.Clear();
        }
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

    public void AddManaFeature(ManaFeature manaFeature)
    {
        ManaFeatures.Add(manaFeature);
    }

    public void RemoveManaFeature(ManaFeature manaFeature)
    {
        ManaFeatures.Remove(manaFeature);
    }

    void PopulateManaFonts()
    {
        for (int i = 0; i < 12; i++)
        {
            int x = Random.Range(1, GridManager.Instance.width + 1);
            int y = Random.Range(1, GridManager.Instance.height + 1);
            Tile tile = GridManager.Instance.GetTile(x, y);

            if (tile != null && tile.behavior != TileBehavior.Border)
            {
                tile.gameObject.AddComponent<ManaFont>();
                ManaFont mf = tile.gameObject.GetComponent<ManaFont>();
                mf.Init(tile, 1, 80);

                if (tile.AddManaFeature(mf))
                {
                    Debug.Log($"Mana Font has been added to ({x}, {y})");
                    AddManaFeature(mf);
                }
                else
                {
                    Destroy(mf);
                    i--;
                }
            }
            else
            {
                i--;
            }
        }
    }

    public void PerformManaPhase()
    {
        //DebugManaGeneration();

        foreach (ManaFeature feature in ManaFeatures)
        {
            feature.ExecutePhase();
        }

        foreach (ManaContainer mc in ManaContainers)
        {
            mc.CheckManaCap();
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

            if (timeSinceStart >= 1.05f)
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
        //GridManager.Instance.height
        for (int x = 0; x <= GridManager.Instance.width; x++)
        {
            ManaParticle mp = new ManaParticle
            {
                type = (ManaType) (manaGeneratedCount % 11),
                quantity = manaGenerationRate,
                particleAge = 0,
                particleX = x,
                particleY = 16,
                inertia = 1,
                velocity = new Vector2(0, 0),
                prevOrb = -1,
                nextOrb = 0
            };
            GridManager.Instance.GetTile(x, 16).AddMana(mp);
            manaGeneratedCount++;
        }

        for (int i = 0; i < 100; i++)
        {
            int x = Random.Range(0, GridManager.Instance.width);
            ManaParticle mp = new ManaParticle
            {
                type = (ManaType)(manaGeneratedCount % 11),
                quantity = manaGenerationRate,
                particleAge = 0,
                particleX = x,
                particleY = 16,
                inertia = 1,
                velocity = new Vector2(0, 0),
                prevOrb = -1,
                nextOrb = 0
            };
            GridManager.Instance.GetTile(x, 16).AddMana(mp);
            manaGeneratedCount++;
        }
    }
}