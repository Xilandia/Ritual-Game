using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinder Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Tile[] FindPath(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile> { start };
        List<Tile> closedList = new List<Tile>();

        for (int x = 1; x <= GridManager.Instance.GetWidth(); x++)
        {
            for (int y = 1; y <= GridManager.Instance.GetHeight(); y++)
            {
                Tile tile = GridManager.Instance.GetTile(x, y);
                tile.gCost = int.MaxValue;
                tile.CalculateFCost();
                tile.previousTileInPath = null;
            }
        }

        start.gCost = 0;
        start.hCost = CalculateDistanceCost(start, end);
        start.CalculateFCost();

        while (openList.Count > 0)
        {
            Tile currentTile = GetLowestFCostTile(openList);

            if (currentTile == end)
            {
                return CalculatePath(end);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (Tile neighborTile in GetNeighborList(currentTile))
            {
                if (closedList.Contains(neighborTile)) continue;

                if (neighborTile.behavior != TileBehavior.Passable)
                {
                    closedList.Add(neighborTile);
                    continue;
                }

                int tentativeGCost = currentTile.gCost + CalculateDistanceCost(currentTile, neighborTile);

                if (tentativeGCost < neighborTile.gCost)
                {
                    neighborTile.previousTileInPath = currentTile;
                    neighborTile.gCost = tentativeGCost;
                    neighborTile.hCost = CalculateDistanceCost(neighborTile, end);
                    neighborTile.CalculateFCost();

                    if (!openList.Contains(neighborTile))
                    {
                        openList.Add(neighborTile);
                    }
                }
            }
        }

        return null; // Path not found (deal with this later)
    }

    private int CalculateDistanceCost(Tile start, Tile end)
    {
        int xDistance = Mathf.Abs(start.coordX - end.coordX);
        int yDistance = Mathf.Abs(start.coordY - end.coordY);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private Tile GetLowestFCostTile(List<Tile> tileList)
    {
        Tile lowestFCostTile = tileList[0];

        for (int i = 1; i < tileList.Count; i++)
        {
            if (tileList[i].fCost < lowestFCostTile.fCost)
            {
                lowestFCostTile = tileList[i];
            }
        }

        return lowestFCostTile;
    }

    private List<Tile> GetNeighborList(Tile currentTile)
    {
        return new List<Tile>
        {
            currentTile.North,
            currentTile.NorthEast,
            currentTile.East,
            currentTile.SouthEast,
            currentTile.South,
            currentTile.SouthWest,
            currentTile.West,
            currentTile.NorthWest
        }.Where(tile => tile != null).ToList();
    }

    private Tile[] CalculatePath(Tile end)
    {
        List<Tile> path = new List<Tile>();

        path.Add(end);
        Tile currentTile = end;

        while (currentTile.previousTileInPath != null)
        {
            path.Add(currentTile.previousTileInPath);
            currentTile = currentTile.previousTileInPath;
        }

        path.Reverse();
        return path.ToArray();
    }
}
