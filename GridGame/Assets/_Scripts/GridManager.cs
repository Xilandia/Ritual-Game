using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width { get; private set; }
    public int height { get; private set; }

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform gridParent;

    [SerializeField] private Material baseColor, offSetColor, pitMaterial, blockedMaterial, gapMaterial;

    private Tile[,] tileGrid;
    private HashSet<Tile> reachableTiles = new HashSet<Tile>();
    private bool isTileSelected;
    private Tile selectedTile;

    public static GridManager Instance { get; private set; }

    public void Init(int iWidth, int iHeight)
    {
        Instance = this;
        width = iWidth;
        height = iHeight;
        GenerateGrid();
        SetGridNeighbors();
        RandomizeGrid();
        GenerateItemsForTesting();
        //XYTest();
    }

    void GenerateGrid()
    {
        tileGrid = new Tile[width + 2, height + 2];

        for (int x = 0; x < width + 2; x++)
        {
            var spawnedTile = Instantiate(tilePrefab, new Vector3(x, 1, 0), Quaternion.identity);
            spawnedTile.name = $"Border Tile {x} 0";
            spawnedTile.Init(TileBehavior.Border, x, 0);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            spawnedTile.transform.SetParent(gridParent);
            tileGrid[x, 0] = spawnedTile;

            spawnedTile = Instantiate(tilePrefab, new Vector3(x, 1, height + 1), Quaternion.identity);
            spawnedTile.name = $"Border Tile {x} {height + 1}";
            spawnedTile.Init(TileBehavior.Border, x, height + 1);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            spawnedTile.transform.SetParent(gridParent);
            tileGrid[x, height + 1] = spawnedTile;
        }

        for (int y = 1; y <= height; y++)
        {
            var spawnedTile = Instantiate(tilePrefab, new Vector3(0, 1, y), Quaternion.identity);
            spawnedTile.name = $"Border Tile 0 {y}";
            spawnedTile.Init(TileBehavior.Border, 0, y);
            spawnedTile.GetComponent<Renderer>().material = pitMaterial;
            spawnedTile.transform.SetParent(gridParent);
            tileGrid[0, y] = spawnedTile;

            spawnedTile = Instantiate(tilePrefab, new Vector3(width + 1, 1, y), Quaternion.identity);
            spawnedTile.name = $"Border Tile {width + 1} {y}";
            spawnedTile.Init(TileBehavior.Border, width + 1, y);
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
                spawnedTile.Init(TileBehavior.Passable, x, y);
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

    void RandomizeGrid() // fix when width < height - seems to break often
    {
        HashSet<int> randomNumbers = new HashSet<int>();
        int divider = width > height ? width : height;

        while (randomNumbers.Count < 30)
        {
            randomNumbers.Add(Random.Range(0, width * height));
        }

        List<int> randomTiles = randomNumbers.ToList();

        for (int i = 0; i < 10; i++)
        {
            tileGrid[randomTiles[i] % divider + 1, randomTiles[i] / divider + 1].behavior = TileBehavior.Blocked; 
            tileGrid[randomTiles[i] % divider + 1, randomTiles[i] / divider + 1].GetComponent<Renderer>().material = blockedMaterial;
            tileGrid[randomTiles[i + 10] % divider + 1, randomTiles[i + 10] / divider + 1].behavior = TileBehavior.Gap;
            tileGrid[randomTiles[i + 10] % divider + 1, randomTiles[i + 10] / divider + 1].GetComponent<Renderer>().material = gapMaterial;
            tileGrid[randomTiles[i + 20] % divider + 1, randomTiles[i + 20] / divider + 1]
                .AssignItem(ItemHandler.Instance.CreateItem());

            Debug.Log($"Chosen tiles: {randomTiles[i]}, {randomTiles[i + 10]}, {randomTiles[i + 20]}");
            Debug.Log($"Tile {randomTiles[i] % divider + 1} {randomTiles[i] / divider + 1} is now blocked.");
            Debug.Log($"Tile {randomTiles[i + 10] % divider + 1} {randomTiles[i + 10] / divider + 1} is now a gap.");
            Debug.Log($"Tile {randomTiles[i + 20] % divider + 1} {randomTiles[i + 20] / divider + 1} now has an item.");
        }
    }

    void GenerateItemsForTesting()
    {
        tileGrid[1, 1].AssignItem(ItemHandler.Instance.CreateItem("Ballista"));
        tileGrid[2, 1].AssignItem(ItemHandler.Instance.CreateItem("Blast"));
        tileGrid[3, 1].AssignItem(ItemHandler.Instance.CreateItem("Javelin"));
        tileGrid[4, 1].AssignItem(ItemHandler.Instance.CreateItem("Katana"));
        tileGrid[5, 1].AssignItem(ItemHandler.Instance.CreateItem("Rapier"));
    }


    void XYTest()
    {
        tileGrid[1, 2].GetComponent<Renderer>().material = blockedMaterial;
        tileGrid[1, 3].GetComponent<Renderer>().material = blockedMaterial;
        tileGrid[2, 4].GetComponent<Renderer>().material = blockedMaterial;
        tileGrid[5, 4].GetComponent<Renderer>().material = blockedMaterial;

        for (int i = 7; i < 10; i++)
        {
            for (int j = 7; j < 9; j++)
            {
                tileGrid[i, j].GetComponent<Renderer>().material = blockedMaterial;
            }
        }
    }
    public void PlaceCharacter(ICharacter character, int x, int y)
    {
        if (tileGrid[x, y].behavior == TileBehavior.Passable)
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
        if (selectedTile.TileHasCharacter() && selectedTile != tile)
        {
            Tile[] path = Pathfinder.Instance.FindPath(selectedTile, tile);

            if (path != null)
            {
                MoveAction movement = new MoveAction(selectedTile.GetCharacter(), path, 0);
                InitiativeManager.Instance.AddMovementAction(movement, 1);

                selectedTile.DeselectTile();
                DeselectTile();
                ClearReachableTiles();
            }
            else
            {
                Debug.Log("No path found.");
                Debug.Log(this);
                Debug.Log(tile);
            }
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

        if (tile.behavior == TileBehavior.Passable)
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

    public void CharacterUseItem()
    {
        if (selectedTile.TileHasCharacter())
        {
            selectedTile.CharacterUseItem();
        }
    }

    public void CharacterTurn(CharacterFaceDirection direction)
    {
        if (selectedTile.TileHasCharacter())
        {
            selectedTile.CharacterTurn(direction);
        }
    }

    public Tile GetTile(int x, int y) 
    {
        return tileGrid[x, y];
    }

    /*public Tile GetSelectedTile() // Might be needed, but not sure yet
    {
        return selectedTile;
    }

    public bool IsTileSelected()
    {
        return isTileSelected;
    }*/
}
