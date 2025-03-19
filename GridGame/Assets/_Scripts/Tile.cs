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
    public int coordX { get; private set; }
    public int coordY { get; private set; }

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

    public Tile previousTileInPath;
    public int fCost;
    public int gCost;
    public int hCost;

    private Vector3 originalPosition;
    private float bounceHeight;
    private float bounceDuration;
    private float bounceStartTime;
    [SerializeField] private bool isBouncing;

    public void Init(int Ibehavior, int Ix, int Iy)
    {
		behavior = (TileBehavior) Ibehavior;
		coordX = Ix;
		coordY = Iy;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isBouncing)
        {
            ApplyBounce();
        }
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

    public bool TileHasCharacter()
    {
        return character != null;
    }

    public ICharacter GetCharacter()
    {
        return character;
    }

    public void MoveCharacter(Tile tile)
    {
        if (tile.PlaceCharacter(character))
        {
            character.SetCurrentTile(tile);
            character = null;
        }
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

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void TakeDamage(int damage)
    {
        TileDamaged(damage);

        if (character != null)
        {
            character.TakeDamage(damage);
        }
    }

    private void TileDamaged(int damage)
    {
        bounceHeight = Mathf.Clamp(damage * 0.1f, 0.1f, 2f); // Linear scaling
        bounceDuration = Mathf.Clamp(0.2f * Mathf.Pow(damage, 0.5f), 0.2f, 1.5f); // Exponential scaling
        bounceStartTime = Time.time;
        isBouncing = true;
    }

    private void ApplyBounce()
    {
        float elapsedTime = Time.time - bounceStartTime;
        float t = elapsedTime / bounceDuration;

        if (t < 1f)
        {
            // Full rise and fall using a quadratic function
            float height = bounceHeight * (4 * t * (1 - t));
            transform.position = originalPosition + new Vector3(0, height, 0);
        }
        else
        {
            transform.position = originalPosition; // Reset to ground level
            isBouncing = false;
        }
    }

    public void DebugStatus()
    {
        Debug.Log("Tile: " + coordX + ", " + coordY);
        Debug.Log("Behavior: " + behavior);
        Debug.Log("Character: " + character);
    }
}
