using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingManaParticlePool : MonoBehaviour
{
    public static MovingManaParticlePool Instance { get; private set; }

    [Tooltip("Prefab for moving mana particles")]
    [SerializeField] private GameObject movingParticlePrefab;

    [Tooltip("How many to pre‑allocate at start")]
    [SerializeField] private int initialPoolSize = 500;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Pre‑warm
        for (int i = 0; i < initialPoolSize; i++)
        {
            var go = Instantiate(movingParticlePrefab, transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    /// <summary>
    /// Gets a particle GameObject (activated). Grows pool if needed.
    /// </summary>
    public GameObject Get()
    {
        GameObject go;
        if (pool.Count > 0)
        {
            go = pool.Dequeue();
        }
        else
        {
            go = Instantiate(movingParticlePrefab, transform);
        }
        go.SetActive(true);
        return go;
    }

    /// <summary>
    /// Returns a particle to the pool (deactivated).
    /// </summary>
    public void Release(GameObject go)
    {
        go.SetActive(false);
        pool.Enqueue(go);
    }
}
