using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICharacter
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private int movementRange = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentTile(Tile tile)
    {
        currentTile = tile;
        this.transform.position = currentTile.transform.position;
    }

    public int GetMovementRange()
    {
        return movementRange;
    }
}
