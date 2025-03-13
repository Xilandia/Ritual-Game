public interface ICharacter
{  
    string Name { get; }
    StatBlock stats { get; }

    void SetCurrentTile(Tile tile);
    int GetMovementRange();
}
