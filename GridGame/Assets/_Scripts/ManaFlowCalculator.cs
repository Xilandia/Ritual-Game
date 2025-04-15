using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaFlowCalculator : MonoBehaviour
{
    public static ManaFlowCalculator Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public ManaParticle CalculateManaFlow(Tile tile, ManaContainer manaContainer, ManaParticle manaParticle)
    {
        // TODO: Enrich this method with your flow rules, based on mana type and surrounding Tiles
        manaParticle.velocity = new Vector2(0f, -1f);
        manaParticle.particleX += Mathf.RoundToInt(manaParticle.velocity.x);
        manaParticle.particleY += Mathf.RoundToInt(manaParticle.velocity.y);
        manaParticle.prevOrb = manaParticle.nextOrb;
        return manaParticle;
    }
}
