using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeChecker : MonoBehaviour
{
    public static RangeChecker instance;

    void Start()
    {
        instance = this;
    }

    public (Tile[], int[]) GetLegalTargets(bool[,] hitMap, int[,] damageMap, int sourceX, int sourceY,
        CharacterFaceDirection direction)
    {
        List<Tile> legalTargets = new List<Tile>();
        List<int> damageValues = new List<int>();

        int mapWidth = hitMap.GetLength(0);
        int mapHeight = hitMap.GetLength(1);

        // Find the pivot in the damageMap (cell with -1). With the new reading,
        // (0,0) is bottom left so the pivot will be in that coordinate system.
        int pivotX = -1, pivotY = -1;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (damageMap[x, y] == -1)
                {
                    pivotX = x;
                    pivotY = y;
                    break;
                }
            }

            if (pivotX != -1)
                break;
        }

        // If no pivot is found, default to the center.
        if (pivotX == -1 || pivotY == -1)
        {
            pivotX = mapWidth / 2;
            pivotY = mapHeight / 2;
        }

        // Iterate over every cell in the hitMap.
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (!hitMap[x, y])
                    continue;

                // Skip the pivot cell (the holder) as it represents the character.
                if (x == pivotX && y == pivotY)
                    continue;

                // Get the damage value for this cell.
                int cellDamage = damageMap[x, y];

                // Compute the offset from the pivot (now in a bottom-left origin system).
                int dx = x - pivotX;
                int dy = y - pivotY;

                int rotatedX = 0, rotatedY = 0;
                // Rotate the offset according to the character's facing.
                // The default attack map is drawn for North.
                switch (direction)
                {
                    case CharacterFaceDirection.North:
                        rotatedX = dx;
                        rotatedY = dy;
                        break;
                    case CharacterFaceDirection.East:
                        rotatedX = dy;
                        rotatedY = -dx;
                        break;
                    case CharacterFaceDirection.South:
                        rotatedX = -dx;
                        rotatedY = -dy;
                        break;
                    case CharacterFaceDirection.West:
                        rotatedX = -dy;
                        rotatedY = dx;
                        break;
                }

                // Calculate the target tile's grid coordinates.
                int targetX = sourceX + rotatedX;
                int targetY = sourceY + rotatedY;

                // Check line of sight: every tile along the line (except the source)
                // must have a behavior of Passable or Feature.
                List<Vector2Int> line = GetLine(sourceX, sourceY, targetX, targetY);
                bool hasLineOfSight = true;
                foreach (Vector2Int pos in line)
                {
                    if (pos.x == sourceX && pos.y == sourceY)
                        continue;

                    Tile t = GridManager.Instance.GetTile(pos.x, pos.y);
                    if (t == null || t.behavior != TileBehavior.Passable)
                    {
                        hasLineOfSight = false;
                        break;
                    }
                }

                if (hasLineOfSight)
                {
                    Tile targetTile = GridManager.Instance.GetTile(targetX, targetY);
                    if (targetTile)
                    {
                        legalTargets.Add(targetTile);
                        damageValues.Add(cellDamage);
                    }
                }
            }
        }

        return (legalTargets.ToArray(), damageValues.ToArray());
    }

    // Bresenham's Line Algorithm to compute the line from (x0,y0) to (x1,y1)
    private List<Vector2Int> GetLine(int x0, int y0, int x1, int y1)
    {
        List<Vector2Int> points = new List<Vector2Int>();

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;
        int x = x0;
        int y = y0;

        while (true)
        {
            points.Add(new Vector2Int(x, y));
            if (x == x1 && y == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }

        return points;
    }
}
