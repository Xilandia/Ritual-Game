using System.Collections.Generic;


public abstract class Action
{

    public string ActionName { get; protected set; }
    public ICharacter Source { get; protected set; }
    public Tile[] TargetTile { get; protected set; }

    public Dictionary<string, object> Parameters { get; protected set; }

    public Action(string actionName, ICharacter source, Tile[] targetTile)
    {
        ActionName = actionName;
        Source = source;
        TargetTile = targetTile;
        Parameters = new Dictionary<string, object>();
    }

    public abstract void Execute();

    public override string ToString()
    {
        string sourceName = Source != null ? Source.Name : "None";
        return $"{sourceName} is executing {ActionName}";
    }
}
