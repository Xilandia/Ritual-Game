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
        manaParticle.particleAge++;
        manaParticle.velocity = ChooseFlowEquation(manaParticle);

        int dx = Mathf.RoundToInt(manaParticle.velocity.x);
        int dy = Mathf.RoundToInt(manaParticle.velocity.y);

        manaParticle.inertia = (dx == 0 && dy == 0) ? manaParticle.inertia + 1 : 1;
        manaParticle.particleX += dx;
        manaParticle.particleY += dy;
        manaParticle.prevOrb = manaParticle.nextOrb;

        if (manaParticle.particleX < 0)
        {
            manaParticle.particleX = 0;
        }

        if (manaParticle.particleX >= GridManager.Instance.width)
        {
            manaParticle.particleX = GridManager.Instance.width + 1;
        }

        if (manaParticle.particleY < 0)
        {
            manaParticle.particleY = 0;
        }

        if (manaParticle.particleY >= GridManager.Instance.height)
        {
            manaParticle.particleY = GridManager.Instance.height + 1;
        }

        return manaParticle;
    }

    private Vector2 ChooseFlowEquation(ManaParticle manaParticle)
    {
        Vector2 flow;
        int x = manaParticle.particleX; 
        int y = manaParticle.particleY;
        int t = manaParticle.particleAge;
        int mult = manaParticle.inertia; 
        ManaType type = manaParticle.type;

        /*switch (type)
        {
            case ManaType.Neutral:
                flow = new Vector2(0.3f, 0.3f);
                break;
            case ManaType.Fire:
                flow = new Vector2(1f, 0f);
                break;
            case ManaType.Water:
                flow = new Vector2(0f, -1f);
                break;
            case ManaType.Air:
                flow = new Vector2(-1f, 0f);
                break;
            case ManaType.Earth:
                flow = new Vector2(0f, 1f);
                break;
            case ManaType.Metal:
                flow = new Vector2(1f, 1f);
                break;
            case ManaType.Wood:
                flow = new Vector2(-1f, -1f);
                break;
            case ManaType.Electric:
                flow = new Vector2(1f, -1f);
                break;
            case ManaType.Emotion:
                flow = new Vector2(-1f, 1f);
                break;
            case ManaType.Light:
                flow = new Vector2(1f, 0.5f);
                break;
            case ManaType.Dark:
                flow = new Vector2(-1f, -0.5f);
                break;
            default:
                flow = new Vector2(0f, 0f);
                break;
        }*/
        //Vector2 center = new Vector2(15, 15);
        flow = LissajousFlow(x, y, t);
        //Debug.Log(flow);
        return flow * mult;
    }

    Vector2 SinusoidalFlow(int x, int y, float t)
    {
        float freq = 0.5f;    // spatial frequency
        float speed = 2.0f;   // temporal speed
        float amp = 1.0f;   // max velocity

        float vx = Mathf.Sin((y * freq) + t * speed);
        float vy = Mathf.Cos((x * freq) - t * speed);
        return new Vector2(vx, vy) * amp;
    }


    Vector2 VortexFlow(int x, int y, float t, Vector2 center) // very swirly, may be useful for something (channels?)
    {
        Vector2 dir = new Vector2(x, y) - center;
        float dist2 = dir.sqrMagnitude + 0.001f;
        float strength = 2.0f;    // swirl intensity

        // Perpendicular (−y, x) gives rotation
        Vector2 swirl = new Vector2(-dir.y, dir.x).normalized;
        return swirl * (strength / dist2);
    }

    Vector2 NoiseCurlFlow(int x, int y, float t) // mana particles seem too low resolution for this
    {
        float scale = 1f;
        float timeScale = 0.2f;

        // Sample noise at offset positions
        float n1 = Mathf.PerlinNoise(x * scale, (y + 1) * scale + t * timeScale);
        float n2 = Mathf.PerlinNoise((x + 1) * scale, y * scale + t * timeScale);
        float n3 = Mathf.PerlinNoise(x * scale, (y - 1) * scale + t * timeScale);
        float n4 = Mathf.PerlinNoise((x - 1) * scale, y * scale + t * timeScale);

        // Approximate curl = (dN/dy, -dN/dx)
        float vx = (n1 - n3) * 0.5f;
        float vy = -(n2 - n4) * 0.5f;
        return new Vector2(vx, vy);
    }

    Vector2 RadialPulseFlow(int x, int y, float t, Vector2 center) // pulls close and pushes far, useful for siphon
    { // alternatively, good for pull to character? have to make sure I don't interact with particles that would get pushed away
        Vector2 dir = new Vector2(x, y) - center; // pulls and pushes straight, so need to add some noise / curve
        float r = dir.magnitude;
        float wave = Mathf.Sin(r * 0.5f - t * 2.0f);  // pulse frequency
        return dir.normalized * wave;
    }

    Vector2 LissajousFlow(int x, int y, float t)
    {
        float a = 2.0f, b = 3.0f;   // frequency multipliers
        float δ = Mathf.PI / 2;     // phase offset

        float vx = Mathf.Sin(a * (x + t) + δ);
        float vy = Mathf.Sin(b * (y - t));
        return new Vector2(vx, vy);
    }

    Vector2 DualVortexFlow(int x, int y, float t)
    {
        // Vortex centers move in circles
        Vector2 c1 = new Vector2(10 + Mathf.Cos(t * 0.5f) * 5f, 10 + Mathf.Sin(t * 0.5f) * 5f);
        Vector2 c2 = new Vector2(20 + Mathf.Sin(t * 0.3f) * 7f, 20 + Mathf.Cos(t * 0.3f) * 7f);

        // Direction to each center
        Vector2 d1 = new Vector2(x, y) - c1;
        Vector2 d2 = new Vector2(x, y) - c2;

        // Swirl vectors (perpendiculars)
        Vector2 v1 = new Vector2(-d1.y, d1.x).normalized / (d1.sqrMagnitude + 1f);
        Vector2 v2 = new Vector2(d2.y, -d2.x).normalized / (d2.sqrMagnitude + 1f);

        // Combine vortices
        return (v1 + v2) * 2f;
    }

    Vector2 SaddleFlow(int x, int y, float t)
    {
        float strength = 0.05f;
        float vx = x * strength;     // pushes outward in X
        float vy = -y * strength;     // pulls inward in Y
        return new Vector2(vx, vy);
    }

    Vector2 CheckerboardWaveFlow(int x, int y, float t)
    {
        float blockSize = 5f;
        float baseDir = Mathf.Sign(Mathf.Sin((x + y) / blockSize * Mathf.PI));
        float wave = Mathf.Sin(t * 0.5f + (x - y) * 0.2f);

        // Particles alternate left/right in a checker pattern, but pulse over time.
        return new Vector2(baseDir * wave, 0);
    }

    Vector2 SinkFlow(int x, int y, float t)
    {
        Vector2 sink = new Vector2(15, 15);  // center of sink
        Vector2 dir = sink - new Vector2(x, y);
        float dist = dir.magnitude + 0.001f;
        float strength = 5f * Mathf.Exp(-dist * 0.2f);
        return dir.normalized * strength;
    }

    Vector2 MovingAttractorFlow(int x, int y, float t)
    {
        float radius = 12f;
        // Attractor moves in a circle
        Vector2 center = new Vector2(Mathf.Cos(t * 0.3f), Mathf.Sin(t * 0.3f)) * radius + new Vector2(15, 15);

        Vector2 dir = center - new Vector2(x, y);
        float d = dir.magnitude + 0.001f;

        // Oscillate between attract (positive) and repel (negative)
        float sign = Mathf.Sin(t * 2.0f) > 0 ? 1f : -1f;
        float strength = 3f / d;

        return dir.normalized * strength * sign;
    }
}
