using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicLexiconDB : MonoBehaviour
{
    public static RunicLexiconDB Instance { get; private set; }

    [Tooltip("List all your ManaTypeConversionSO assets here.")]
    [SerializeField] private RunicLexiconEntrySO[] runes;

    // Runtime lookup
    private Dictionary<int, RunicLexiconEntrySO> dict;
    public IReadOnlyDictionary<int, RunicLexiconEntrySO> Dict => dict;

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
        dict = new Dictionary<int, RunicLexiconEntrySO>();

        if (runes == null)
        {
            Debug.LogWarning("[RunicLexiconDB] No runes assigned!");
            return;
        }

        foreach (var so in runes)
        {
            if (so == null) continue;
            if (dict.ContainsKey(so.entry.runeId)) // Currently no relevant KV to check
            {
                Debug.LogWarning($"[RunicLexiconDB] Duplicate entry for {so.entry.runeSlot}, skipping.");
                continue;
            }
            dict.Add(so.entry.runeId, so);
        }
    }

    public RunicLexiconEntrySO GetRuneById(int id)
    {
        if (dict.TryGetValue(id, out var rune))
        {
            return rune;
        }

        Debug.LogWarning($"[RunicLexiconDB] Rune with ID {id} not found.");
        return null;
    }
}
