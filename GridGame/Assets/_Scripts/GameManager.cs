using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CharacterManager characterManager;

    void Start()
    {
        gridManager.Init();
        characterManager.Init();
    }

    void Update()
    {
        
    }
}
