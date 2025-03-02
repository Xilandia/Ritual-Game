using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileBehavior
{
	Passable, Blocked, Border, Gap, Feature
}

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject highlight, clicklight;
	
	[SerializeField] private Tile north, northeast, east, southeast, south, southwest, west, northwest;
	[SerializeField] private int coordX, coordY;

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
        isClicked = !isClicked;
        clicklight.SetActive(isClicked);
    }

	public void SetNorth(Tile n)
	{
		north = n;
	}

	public void SetNorthEast(Tile ne)
	{
		northeast = ne;
	}

	public void SetEast(Tile e)
	{
		east = e;
	}

	public void SetSouthEast(Tile se)
	{
		southeast = se;
	}

	public void SetSouth(Tile s)
	{
		south = s;
	}

	public void SetSouthWest(Tile sw)
	{
		southwest = sw;
	}

	public void SetWest(Tile w)
	{
		west = w;
	}

	public void SetNorthWest(Tile nw)
	{
		northwest = nw;
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
