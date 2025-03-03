using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacter
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private int movementRange = 3;

    public void SetCurrentTile(Tile tile)
    {
        currentTile = tile;
        this.transform.position = currentTile.transform.position;
    }

    public int GetMovementRange()
    {
        return movementRange;
    }
}
