using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform gridParent;

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
            spawnedTile.transform.SetParent(gridParent);
            tileGrid[x, 0] = spawnedTile;

            spawnedTile = Instantiate(tilePrefab, new Vector3(x, 1, height + 1), Quaternion.identity);
            spawnedTile.name = $"Border Tile {x} {height + 1}";
            spawnedTile.Init(2, x, height + 1);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            spawnedTile.transform.SetParent(gridParent);
            tileGrid[x, height + 1] = spawnedTile;
        }

        for (int y = 1; y <= height; y++)
        {
            var spawnedTile = Instantiate(tilePrefab, new Vector3(0, 1, y), Quaternion.identity);
            spawnedTile.name = $"Border Tile 0 {y}";
            spawnedTile.Init(2, 0, y);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            spawnedTile.transform.SetParent(gridParent);
            tileGrid[0, y] = spawnedTile;

            spawnedTile = Instantiate(tilePrefab, new Vector3(width + 1, 1, y), Quaternion.identity);
            spawnedTile.name = $"Border Tile {width + 1} {y}";
            spawnedTile.Init(2, width + 1, y);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            spawnedTile.transform.SetParent(gridParent);
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
                spawnedTile.transform.SetParent(gridParent);
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

    public void Init()
    {
        GenerateGrid();
        SetGridNeighbors();
    }

    public void PlaceCharacter(ICharacter character, int x, int y)
    {
        if (tileGrid[x, y].IsPassable())
        {
            if (!tileGrid[x, y].PlaceCharacter(character))
            {
                Debug.Log("Character already exists on this tile.");
                return;
            }

            Debug.Log($"Character placed at {x}, {y}");
            character.SetCurrentTile(tileGrid[x, y]);
        }
    }
}
