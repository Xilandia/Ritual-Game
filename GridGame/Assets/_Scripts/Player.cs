using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacter
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private int movementRange = 3;
    [SerializeField] private int health = 20;
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
        faceDirection = CharacterFaceDirection.East;
        stats.MovementRange = movementRange;
        stats.Health = health;
        stats.MaxHealth = health;
        stats.Strength = 5;
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

    public void TurnCharacter(CharacterFaceDirection direction)
    {
        faceDirection = direction;
        switch (direction)
        {
            case CharacterFaceDirection.North:
                transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
            case CharacterFaceDirection.South:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case CharacterFaceDirection.West:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case CharacterFaceDirection.East:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
        }
    }

    public void EquipItem(IItem item)
    {
        if (inventory.Contains(item)) // Consider checking if weapon too
        {
            equippedItem = item;
        }
    }

    public void Unequip()
    {
        equippedItem = null;
    }

    public void AddItemToInventory(IItem item)
    {
        inventory.Add(item);

        if (equippedItem == null)
        {
            EquipItem(item);
        }

        item.DebugItem();
    }

    public void DiscardItemFromInventory(IItem item)
    {
        inventory.Remove(item);
    }

    public void UseEquippedItem()
    {
        if (equippedItem != null)
        {
            //equippedItem.DebugItem();
            (Tile[] tilesToTarget, int[] damageToDeal) = RangeChecker.instance.GetLegalTargets(equippedItem.GetHitMap(),
                equippedItem.GetDamageMap(), currentTile.coordX, currentTile.coordY, faceDirection);
            AttackAction action = new AttackAction(this, tilesToTarget, damageToDeal);
            InitiativeManager.Instance.AddPostMovementAction(action, 1);
        }
    }

    public int GetMovementRange()
    {
        return stats.MovementRange;
    }

    public float GetDamageMultiplier()
    {
        return stats.Strength;
    }

    public void TakeDamage(int damage)
    {
        stats.Health -= (damage - stats.Defense);

        if (stats.Health <= 0)
        {
            //Die();
            Debug.Log($"Player \"{Name}\" has died");
        }
    }

}
