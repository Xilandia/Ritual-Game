using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ManaType
{
    Neutral,
    Fire, Water, Air, Earth,
    Light, Dark,
    Life, Death
}

public class ManaParticle : MonoBehaviour
{
    [SerializeField] private ManaType manaType;
    [SerializeField] private int manaAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
