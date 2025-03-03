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
    private HashSet<Tile> reachableTiles = new HashSet<Tile>();
    private bool isTileSelected;
    private Tile selectedTile;

    public static GridManager Instance { get; private set; }

    public void Init()
    {
        Instance = this;
        GenerateGrid();
        SetGridNeighbors();
    }

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
                tileGrid[x, y].North = tileGrid[x, y + 1];
                tileGrid[x, y].NorthEast = tileGrid[x + 1, y + 1];
                tileGrid[x, y].East = tileGrid[x + 1, y];
                tileGrid[x, y].SouthEast = tileGrid[x + 1, y - 1];
                tileGrid[x, y].South = tileGrid[x, y - 1];
                tileGrid[x, y].SouthWest = tileGrid[x - 1, y - 1];
                tileGrid[x, y].West = tileGrid[x - 1, y];
                tileGrid[x, y].NorthWest = tileGrid[x - 1, y + 1];
            }
        }
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

            character.SetCurrentTile(tileGrid[x, y]);
        }
    }

    public void MoveCharacter(Tile tile)
    {
        if (selectedTile.TileHasCharacter() && selectedTile != tile && tile.IsReachable())
        {
            selectedTile.MoveCharacter(tile);
            selectedTile.DeselectTile();
            DeselectTile();
            ClearReachableTiles();
        }
    }

    public void SelectTile(Tile tile)
    {
        if (isTileSelected)
        {
            selectedTile.DeselectTile();
            ClearReachableTiles();
        }

        if (selectedTile == tile)
        {
            DeselectTile();
            return;
        }

        selectedTile = tile;
        isTileSelected = true;
    }

    void DeselectTile()
    {
        isTileSelected = false;
        selectedTile = null;
    }

    public void ShowReachableTiles(Tile tile, int movementRange, bool isPlayer)
    {
        if (tile == null || movementRange < 0)
        {
            return;
        }

        if (tile.IsPassable())
        {
            reachableTiles.Add(tile);
            tile.SetReachable(isPlayer);
        }

        ShowReachableTiles(tile.North, movementRange - 1, isPlayer);
        ShowReachableTiles(tile.East, movementRange - 1, isPlayer);
        ShowReachableTiles(tile.South, movementRange - 1, isPlayer);
        ShowReachableTiles(tile.West, movementRange - 1, isPlayer);
    }

    public void ClearReachableTiles()
    {
        foreach (Tile tile in reachableTiles)
        {
            tile.SetUnreachable();
        }
        reachableTiles.Clear();
    }
}
