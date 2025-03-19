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


    public IItem CreateItem()
    {
        string dataFileName = itemDataFileNames[Random.Range(0, itemDataFileNames.Count)];
        string filePath = System.IO.Path.Combine(Application.dataPath, itemDataFolder, $"{dataFileName}{itemDataExtension}");

        reader.PrepareReader(filePath);

        if (reader.ReadItemFile())
        {
            return populator.PopulateItem();
        }

        return null;
    }

    public void Init()
    {
        IItem item = CreateItem();
        item.DebugItem();
    }
}
