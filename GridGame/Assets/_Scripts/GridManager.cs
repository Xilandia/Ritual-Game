using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private int width, height;

    [SerializeField] private Tile tilePrefab;

    [SerializeField] private Material baseColor, offSetColor;

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, 1, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                spawnedTile.GetComponent<Renderer>().material =
                    (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0) ? baseColor : offSetColor;
            }
        }
    }
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
