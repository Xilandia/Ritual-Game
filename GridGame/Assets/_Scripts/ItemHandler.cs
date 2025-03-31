using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    [SerializeField] private ItemReader reader;
    [SerializeField] private ItemWriter writer;
    [SerializeField] private ItemPopulator populator;

    [SerializeField] private string itemDataFolder;
    [SerializeField] private string itemDataExtension;
    [SerializeField] private List<string> itemDataFileNames;

    public static ItemHandler Instance;

    void Start()
    {
        Instance = this;
    }

    public void Init()
    {
        DebugCreateItems(); // Generate one of each item for debugging purposes
    }

    public IItem CreateItem()
    {
        string dataFileName = itemDataFileNames[Random.Range(0, itemDataFileNames.Count)];

        return CreateItem(dataFileName);
    }

    public IItem CreateItem(string dataFileName)
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, itemDataFolder, $"{dataFileName}{itemDataExtension}");

        reader.PrepareReader(filePath);

        if (reader.ReadItemFile())
        {
            return populator.PopulateItem(dataFileName);
        }

        return null;
    }

    private void DebugCreateItems()
    {
        string filePath;
        
        foreach (string dataFileName in itemDataFileNames)
        {
            filePath = System.IO.Path.Combine(Application.dataPath, itemDataFolder, $"{dataFileName}{itemDataExtension}");
            reader.PrepareReader(filePath);
            Debug.Log(dataFileName);
            if (reader.ReadItemFile())
            {
                IItem item = populator.PopulateItem(dataFileName);
                item.DebugItem();
            }
        }
    }
}
