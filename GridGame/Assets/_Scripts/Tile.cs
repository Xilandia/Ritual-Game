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

    void OnMouseDown()
    {
        isClicked = true;
        clicklight.SetActive(isClicked);
        GridManager.Instance.SelectTile(this);

        if (character != null)
        {
            GridManager.Instance.ShowReachableTiles(this, character.GetMovementRange(), character is Player);
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
    }

    public void SetUnreachable()
    {
        playerReachable.SetActive(false);
        enemyReachable.SetActive(false);
    }

    public bool IsPassable()
    {
        return behavior == TileBehavior.Passable;
    }

    public bool PlaceCharacter(ICharacter character)
    {
        if (this.character == null)
        {
            this.character = character;
            return true;
        }
        return false;
    }
}
