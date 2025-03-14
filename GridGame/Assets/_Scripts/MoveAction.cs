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
        TargetTile[0].MoveCharacter(TargetTile[TargetTile.Length - 1]);
        //throw new System.NotImplementedException();
    }
}
