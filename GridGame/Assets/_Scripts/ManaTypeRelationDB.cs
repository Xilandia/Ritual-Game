using System.Collections.Generic;
using UnityEngine;

public class ManaTypeRelationDB : MonoBehaviour
{
    public static ManaTypeRelationDB Instance { get; private set; }

    [Tooltip("List all your ManaTypeRelationSO assets here.")]
    [SerializeField] private ManaTypeRelationSO[] relations;

    // Runtime lookup
    private Dictionary<ManaType, ManaTypeRelationSO> dict;
    public IReadOnlyDictionary<ManaType, ManaTypeRelationSO> Dict => dict;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildDictionary();
    }

    private void BuildDictionary()
    {
        dict = new Dictionary<ManaType, ManaTypeRelationSO>();
        if (relations == null)
        {
            Debug.LogWarning("[ManaTypeRelationDB] No relations assigned!");
            return;
        }

        foreach (var so in relations)
        {
            if (so == null) continue;
            if (dict.ContainsKey(so.manaType))
            {
                Debug.LogWarning($"[ManaTypeRelationDB] Duplicate entry for {so.manaType}, skipping.");
                continue;
            }
            dict.Add(so.manaType, so);
        }
    }
}