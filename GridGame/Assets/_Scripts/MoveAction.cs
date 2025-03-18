using System.Collections.Generic;

public class MoveAction : Action
{
    public MoveAction(ICharacter source, Tile[] targetTile, int positionOnPath) : base("Move", source, targetTile)
    {
        Parameters["Speed"] = source.GetMovementRange();
        Parameters["PositionOnPath"] = positionOnPath;
    }


    public override void Execute()
    {
        int speed = Parameters.ContainsKey("Speed") ? (int)Parameters["Speed"] : -1;
        int pos = Parameters.ContainsKey("PositionOnPath") ? (int)Parameters["PositionOnPath"] : -1;
        int intendedIndex = pos + speed;
        int targetIndex = intendedIndex < TargetTile.Length ? intendedIndex : TargetTile.Length - 1;

        if (speed == -1 || pos == -1)
        {
            // Figure out how to yell about this
            return;
        }

        TargetTile[pos].MoveCharacter(TargetTile[targetIndex]);
        Parameters["PositionOnPath"] = targetIndex;

        if (targetIndex != TargetTile.Length - 1)
        {
            InitiativeManager.Instance.AddMovementAction(this, InitiativeManager.Instance.currentPhase + 1);
        }
        else
        {
            // Destroy action?
            return;
        }
    }
}
