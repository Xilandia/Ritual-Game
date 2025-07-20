using UnityEngine;

public enum ManaType
{
    Neutral,
    Fire, Water, Air, Earth, Metal, Wood, Electric, Emotion,
    Light, Dark,
    None, All
}

public struct ManaParticle
{
    public ManaType type;
    // public ManaAspects aspect;
    public float quantity;
    public int particleX;
    public int particleY;
    public int prevOrb;
    public int nextOrb;
}