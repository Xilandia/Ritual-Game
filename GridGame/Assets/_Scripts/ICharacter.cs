using System.Collections;
using System.Collections.Generic;

public enum CharacterFaceDirection
{
    Up, Down, Left, Right
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
}
