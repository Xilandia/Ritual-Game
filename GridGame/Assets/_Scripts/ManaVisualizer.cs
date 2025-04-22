using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaVisualizer : MonoBehaviour
{
    [SerializeField] private ManaContainer manaContainer;
    [SerializeField] private GameObject movingParticlePrefab;
    [SerializeField] private List<GameObject> manaOrbs;
    [SerializeField] private List<MeshRenderer> orbMeshRenderers;
    [SerializeField] private List<Material> manaMaterials;

    public List<Vector3> orbStartPositions = new List<Vector3>();
    
    private List<GameObject> movingParticles = new List<GameObject>();
    private List<Vector3> particleOrigins = new List<Vector3>();
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

        if (particle.prevOrb < 9)
        {
            ManaVisualizer dmv = GridManager.Instance.GetTile(particle.particleX, particle.particleY).GetManaVisualizer();

            GameObject movingParticle = Instantiate(movingParticlePrefab, orbStartPositions[particle.prevOrb], Quaternion.identity);
            movingParticle.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f) * particle.quantity / 5;
            movingParticle.gameObject.GetComponent<MeshRenderer>().material = manaMaterials[(int)particle.type];
            movingParticles.Add(movingParticle);
            particleOrigins.Add(orbStartPositions[particle.prevOrb]);
            particleDestinations.Add(dmv.orbStartPositions[(particle.nextOrb == -1 ? particle.prevOrb : particle.nextOrb)]);
        }
    }

    public void ProgressParticleTransition(float timeSinceStart)
    {
        for (int i = 0; i < movingParticles.Count; i++)
        {
            Vector3 startPos = orbStartPositions[i];
            Vector3 dest = particleDestinations[i];
            Vector3 newPos = Vector3.Lerp(startPos, dest, timeSinceStart);
            movingParticles[i].transform.position = newPos;
        }
    }

    public void EndTransition(List<ManaParticle> manaParticles)
    {
        foreach (GameObject movingParticle in movingParticles)
        {
            Destroy(movingParticle);
        }

        movingParticles.Clear();
        particleOrigins.Clear();
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
