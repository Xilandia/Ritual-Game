using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            InitiativeManager.Instance.TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            GridManager.Instance.CharacterUseItem();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GridManager.Instance.CharacterStartRitual();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            GridManager.Instance.CharacterTurn(CharacterFaceDirection.South);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GridManager.Instance.CharacterTurn(CharacterFaceDirection.North);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GridManager.Instance.CharacterTurn(CharacterFaceDirection.West);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GridManager.Instance.CharacterTurn(CharacterFaceDirection.East);
        }
    }
}
