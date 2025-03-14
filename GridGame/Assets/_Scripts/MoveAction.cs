using System.Collections.Generic;

public class MoveAction : Action
{
    public MoveAction(ICharacter source, Tile targetTile) : base("Move", source, targetTile)
    {

    }


    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
}
