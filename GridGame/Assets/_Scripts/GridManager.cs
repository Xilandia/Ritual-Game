using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    [SerializeField] private Tile tilePrefab;

    [SerializeField] private Material baseColor, offSetColor, pitMaterial;

    private Tile[,] tileGrid;

    void GenerateGrid()
    {
        tileGrid = new Tile[width + 2, height + 2];

        for (int x = 0; x < width + 2; x++)
        {
            var spawnedTile = Instantiate(tilePrefab, new Vector3(x, 1, 0), Quaternion.identity);
            spawnedTile.name = $"Border Tile {x} 0";
            spawnedTile.Init(2, x, 0);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            tileGrid[x, 0] = spawnedTile;

            spawnedTile = Instantiate(tilePrefab, new Vector3(x, 1, height + 1), Quaternion.identity);
            spawnedTile.name = $"Border Tile {x} {height + 1}";
            spawnedTile.Init(2, x, height + 1);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            tileGrid[x, height + 1] = spawnedTile;
        }

        for (int y = 1; y <= height; y++)
        {
            var spawnedTile = Instantiate(tilePrefab, new Vector3(0, 1, y), Quaternion.identity);
            spawnedTile.name = $"Border Tile 0 {y}";
            spawnedTile.Init(2, 0, y);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            tileGrid[0, y] = spawnedTile;

            spawnedTile = Instantiate(tilePrefab, new Vector3(width + 1, 1, y), Quaternion.identity);
            spawnedTile.name = $"Border Tile {width + 1} {y}";
            spawnedTile.Init(2, width + 1, y);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            tileGrid[width + 1, y] = spawnedTile;
        }

        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, 1, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.Init(0, x, y);
                spawnedTile.GetComponent<Renderer>().material =
                    (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0) ? baseColor : offSetColor;
                tileGrid[x, y] = spawnedTile;
            }
        }
    }

    void SetGridNeighbors()
    {
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                tileGrid[x, y].SetNorth(tileGrid[x, y + 1]);
                tileGrid[x, y].SetNorthEast(tileGrid[x + 1, y + 1]);
                tileGrid[x, y].SetEast(tileGrid[x + 1, y]);
                tileGrid[x, y].SetSouthEast(tileGrid[x + 1, y - 1]);
                tileGrid[x, y].SetSouth(tileGrid[x, y - 1]);
                tileGrid[x, y].SetSouthWest(tileGrid[x - 1, y - 1]);
                tileGrid[x, y].SetWest(tileGrid[x - 1, y]);
                tileGrid[x, y].SetNorthWest(tileGrid[x - 1, y + 1]);
            }
        }
    }

    void Start()
    {
        GenerateGrid();
        SetGridNeighbors();
    }

}
