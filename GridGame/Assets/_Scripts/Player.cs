using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacter
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private int movementRange = 3;
    public string Name { get; private set; }
    public StatBlock stats { get; private set; }
    public List<IItem> inventory { get; private set; }
    public IItem equippedItem { get; private set; }
    public CharacterFaceDirection faceDirection { get; private set; }

    public void Init()
    {
        Name = "Player";
        stats = new StatBlock();
        inventory = new List<IItem>();
        equippedItem = null;
        faceDirection = CharacterFaceDirection.Down;
        stats.MovementRange = movementRange;
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
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

    public void TakeDamage(int damage)
    {
        // stats.Health -= damage;
    }
}
