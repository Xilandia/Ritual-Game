using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private GridManager gridManager;

    [SerializeField] private List<Player> players;
    [SerializeField] private List<Enemy> enemies;

    public void Init()
    {
        players = new List<Player>();
        enemies = new List<Enemy>();
        CreatePlayer();
        CreateEnemy();

        foreach (Player player in players)
        {
            gridManager.PlaceCharacter(player, 2, 2);
        }

        foreach (Enemy enemy in enemies)
        {
            gridManager.PlaceCharacter(enemy, 16, 16);
        }
    }

    void CreatePlayer()
    {
        var player = Instantiate(playerPrefab, new Vector3(1, 1, 1), Quaternion.identity);
        players.Add(player);
    }

    void CreateEnemy()
    {
        var enemy = Instantiate(enemyPrefab, new Vector3(5, 1, 5), Quaternion.identity);
        enemies.Add(enemy);
    }
}
