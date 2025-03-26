using System.Collections;
using System.Collections.Generic;

public enum CharacterFaceDirection
{
    North, South, West, East
}

public interface ICharacter
{  
    string Name { get; }
    StatBlock stats { get; }
    List<IItem> inventory { get; }
    IItem equippedItem { get; }
    CharacterFaceDirection faceDirection { get; }

    Tile GetCurrentTile();
    void SetCurrentTile(Tile tile);
    int GetMovementRange();
    void TurnCharacter(CharacterFaceDirection direction);
    void EquipItem(IItem item);
    void Unequip();
    void AddItemToInventory(IItem item);
    void DiscardItemFromInventory(IItem item);
    void UseEquippedItem();

    void TakeDamage(int damage);
}
