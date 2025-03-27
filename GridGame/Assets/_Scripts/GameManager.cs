using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private ItemHandler itemHandler;

    [SerializeField] private int width;
    [SerializeField] private int height;

    void Start()
    {
        gridManager.Init(width, height);
        characterManager.Init();
    }

    void Update()
    {
        
    }
}
