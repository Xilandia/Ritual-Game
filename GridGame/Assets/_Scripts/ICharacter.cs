public interface ICharacter
{  
    string Name { get; }
    StatBlock stats { get; }

    Tile GetCurrentTile();
    void SetCurrentTile(Tile tile);
    int GetMovementRange();
}
