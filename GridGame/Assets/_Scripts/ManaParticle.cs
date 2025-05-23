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
    public int quantity;
    public int particleAge;
    public int particleX;
    public int particleY;
    public int inertia;
    public Vector2 velocity;
    public int prevOrb;
    public int nextOrb;
}