using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaVisualizer : MonoBehaviour
{
    [SerializeField] private ManaContainer manaContainer;
    [SerializeField] private List<GameObject> manaOrbs;
    [SerializeField] private List<Vector3> orbPositions;
    [SerializeField] private List<MeshRenderer> orbMeshRenderers;
    [SerializeField] private List<Material> manaMaterials;

    public List<Vector3> orbStartPositions = new List<Vector3>();

    private List<ManaParticle> particles = new List<ManaParticle>();
    private List<Vector3> particleDestinations = new List<Vector3>();

    void Start()
    {
        ManaManager.Instance.AddManaVisualizer(this);

        foreach (GameObject orb in manaOrbs)
        {
            orbStartPositions.Add(orb.transform.position);
            orb.SetActive(false);
        }
    }

    public void PrepareVisualization(ManaParticle particle)
    {
        if (particle.prevOrb == -1 || particle.prevOrb >= manaOrbs.Count || particle.nextOrb >= manaOrbs.Count)
        {
            return;
        }

        particles.Add(particle);

        if (particles.Count < 9)
        {
            ManaVisualizer dmv = GridManager.Instance.GetTile(particle.particleX, particle.particleY).GetManaVisualizer();

            particleDestinations.Add(dmv.orbStartPositions[(particle.nextOrb == -1? particle.prevOrb : particle.nextOrb)]);
        }
    }

    public void ProgressParticleTransition(float timeSinceStart)
    {
        for (int i = 0; i < particleDestinations.Count; i++)
        {
            Vector3 startPos = orbStartPositions[i];
            Vector3 dest = particleDestinations[i];
            Vector3 newPos = Vector3.Lerp(startPos, dest, timeSinceStart);
            manaOrbs[i].transform.position = newPos;
        }
    }

    public void EndTransition(List<ManaParticle> manaParticles)
    {
        particles.Clear();
        particleDestinations.Clear();

        for (int i = 0; i < 9; i++)
        {
            if (manaParticles.Count > i)
            {
                manaOrbs[i].SetActive(true);
                manaOrbs[i].transform.position = orbStartPositions[i];
                manaOrbs[i].transform.localScale = new Vector3(0.15f, 1.5f, 0.15f) * manaParticles[i].quantity / 5;
                orbMeshRenderers[i].material = manaMaterials[(int) manaParticles[i].type];
            }
            else
            {
                manaOrbs[i].SetActive(false);
            }
        }
    }
}
