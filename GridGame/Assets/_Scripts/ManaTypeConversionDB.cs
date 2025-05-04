using System.Collections.Generic;
using UnityEngine;

public class ManaTypeConversionDB : MonoBehaviour
{
    public static ManaTypeConversionDB Instance { get; private set; }

    [Tooltip("List all your ManaTypeConversionSO assets here.")]
    [SerializeField] private ManaTypeConversionSO[] relations;

    // Runtime lookup
    private Dictionary<ManaType, ManaTypeConversionSO> dict;
    public IReadOnlyDictionary<ManaType, ManaTypeConversionSO> Dict => dict;

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
        dict = new Dictionary<ManaType, ManaTypeConversionSO>();
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
                Debug.LogWarning($"[ManaTypeConversionSO] Duplicate entry for {so.manaType}, skipping.");
                continue;
            }
            dict.Add(so.manaType, so);
        }
    }
}