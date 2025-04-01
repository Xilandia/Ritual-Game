using System;
using System.Collections.Generic;

public class AttackAction : Action
{
    public int[] DamageToDeal { get; private set; }

    public AttackAction(ICharacter source, Tile[] targetTile, int[] damageToDeal) : base("Move", source, targetTile)
    {
        DamageToDeal = damageToDeal;
    }


    public override void Execute()
    {
        float damageMultiplier = Source.GetDamageMultiplier();

        for (int i = 0; i < TargetTile.Length; i++)
        {
            TargetTile[i].TakeDamage((int)Math.Round(DamageToDeal[i] * damageMultiplier));
        }
    }
}
