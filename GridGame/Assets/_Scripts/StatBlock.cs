using System.Collections.Generic;

public class StatBlock
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Strength { get; set; }
    public int Magic { get; set; }
    public int Skill { get; set; }
    public int Speed { get; set; }
    public int Luck { get; set; }
    public int Defense { get; set; }
    public int Resistance { get; set; }
    public int MovementRange { get; set; }
    public int Constitution { get; set; }
    public Dictionary<int, int> StatChanges { get; protected set; }
}
