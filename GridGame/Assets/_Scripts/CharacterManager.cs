using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private GridManager gridManager;

    public void Init()
    {
        CreatePlayer();
        CreateEnemy();
        gridManager.PlaceCharacter(playerPrefab, 1, 1);
        gridManager.PlaceCharacter(enemyPrefab, 16, 16);
    }

    void CreatePlayer()
    {
        Instantiate(playerPrefab, new Vector3(1, 1, 1), Quaternion.identity);
    }

    void CreateEnemy()
    {
        Instantiate(enemyPrefab, new Vector3(5, 1, 5), Quaternion.identity);
    }
}
