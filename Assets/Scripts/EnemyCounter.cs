using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    private GameObject[] enemies;
    public TextMeshProUGUI enemyCounter;

    public void UpdateEnemyCounter()
    {
        enemies = GameObject.FindGameObjectsWithTag("Damageable"); // Find all objects taged Damageable
        int enemyCount = enemies.Length;
        enemyCount = Mathf.Max(0, enemyCount); // Ensure the enemy count doesn't go below 0 if there are no enemies
        enemyCounter.text = "TO KILL: " + enemyCount.ToString(); // Ensure the enemy count doesn't go below 0 if there are no enemies
    }
}
