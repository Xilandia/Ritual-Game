using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileBehavior
{
	Passable, Blocked, Border, Gap, Feature
}

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight, clicklight, playerReachable, enemyReachable;
    [SerializeField] private int coordX, coordY;

    public Tile North { get; set; }
    public Tile NorthEast { get; set; }
    public Tile East { get; set; }
    public Tile SouthEast { get; set; }
    public Tile South { get; set; }
    public Tile SouthWest { get; set; }
    public Tile West { get; set; }
    public Tile NorthWest { get; set; }

    [SerializeField] private TileBehavior behavior;
    [SerializeField] private IBuilding building;
    [SerializeField] private ICharacter character;
    [SerializeField] private IItem item;

    private bool isClicked = false;
    private bool isReachable = false;

    public void Init(int Ibehavior, int Ix, int Iy)
    {
		behavior = (TileBehavior) Ibehavior;
		coordX = Ix;
		coordY = Iy;
    }
 
    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isClicked = true;
            clicklight.SetActive(isClicked);
            GridManager.Instance.SelectTile(this);

            if (character != null)
            {
                GridManager.Instance.ShowReachableTiles(this, character.GetMovementRange(), character is Player);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            GridManager.Instance.MoveCharacter(this);
        }
    }

    public void DeselectTile()
    {
        isClicked = false;
        clicklight.SetActive(isClicked);
    }

    public void SetReachable(bool isPlayer)
    {
        if (isPlayer)
        {
            playerReachable.SetActive(true);
        }
        else
        {
            enemyReachable.SetActive(true);
        }

        isReachable = true;
    }

    public void SetUnreachable()
    {
        playerReachable.SetActive(false);
        enemyReachable.SetActive(false);
        isReachable = false;
    }

    public bool IsPassable()
    {
        return behavior == TileBehavior.Passable;
    }

    public bool IsReachable()
    {
        return isReachable;
    }

    public bool PlaceCharacter(ICharacter character)
    {
        if (!TileHasCharacter())
        {
            this.character = character;
            return true;
        }
        return false;
    }

    public bool TileHasCharacter()
    {
        return character != null;
    }

    public void MoveCharacter(Tile tile)
    {
        if (tile.PlaceCharacter(character))
        {
            character.SetCurrentTile(tile);
            character = null;
        }
    }

    public void DebugStatus()
    {
        Debug.Log("Tile: " + coordX + ", " + coordY);
        Debug.Log("Behavior: " + behavior);
        Debug.Log("Character: " + character);
    }
}
