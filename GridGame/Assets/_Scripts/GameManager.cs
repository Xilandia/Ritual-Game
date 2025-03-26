using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private ItemHandler itemHandler;

    void Start()
    {
        gridManager.Init();
        characterManager.Init();
    }

    void Update()
    {
        
    }
}
