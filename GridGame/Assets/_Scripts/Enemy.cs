using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICharacter
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private int movementRange = 3;
    public string Name { get; private set; }
    public StatBlock stats { get; private set; }

    public void Init()
    {
        Name = "Enemy";
        stats = new StatBlock();
        stats.MovementRange = movementRange;
    }

    public void SetCurrentTile(Tile tile)
    {
        currentTile = tile;
        this.transform.position = currentTile.transform.position;
    }

    public int GetMovementRange()
    {
        return stats.MovementRange;
    }
}
