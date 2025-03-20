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

    public Tile[] GetLegalTargets(bool[,] hitMap, int sourceX, int sourceY, CharacterFaceDirection direction)
    {
        List<Tile> legalTargets = new List<Tile>();

        int mapWidth = hitMap.GetLength(0);
        int mapHeight = hitMap.GetLength(1);

        // Assume the hitMap is centered on the character.
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                // Only consider cells marked as valid in the hitMap.
                if (!hitMap[i, j])
                    continue;

                // Compute the offset from the center.
                int offsetX = i - centerX;
                int offsetY = j - centerY;

                // Rotate the offset based on the character’s facing.
                int rotatedX = 0, rotatedY = 0;
                switch (direction)
                {
                    case CharacterFaceDirection.North:
                        rotatedX = offsetX;
                        rotatedY = offsetY;
                        break;
                    case CharacterFaceDirection.South:
                        rotatedX = -offsetX;
                        rotatedY = -offsetY;
                        break;
                    case CharacterFaceDirection.East:
                        rotatedX = offsetY;
                        rotatedY = -offsetX;
                        break;
                    case CharacterFaceDirection.West:
                        rotatedX = -offsetY;
                        rotatedY = offsetX;
                        break;
                }

                // Calculate the target tile's grid coordinates.
                int targetX = sourceX + rotatedX;
                int targetY = sourceY + rotatedY;

                // Check line of sight: all tiles from the source to the target (excluding the source)
                // must be either Passable or Feature.
                List<Vector2Int> line = GetLine(sourceX, sourceY, targetX, targetY);
                bool hasLineOfSight = true;
                foreach (Vector2Int pos in line)
                {
                    // Skip checking the source tile.
                    if (pos.x == sourceX && pos.y == sourceY)
                        continue;

                    Tile t = GridManager.Instance.GetTile(pos.x, pos.y);
                    if (t == null ||
                        (t.behavior != TileBehavior.Passable && t.behavior != TileBehavior.Feature))
                    {
                        hasLineOfSight = false;
                        break;
                    }
                }

                if (hasLineOfSight)
                {
                    Tile targetTile = GridManager.Instance.GetTile(targetX, targetY);
                    if (targetTile)
                        legalTargets.Add(targetTile);
                }
            }
        }

        return legalTargets.ToArray();
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
