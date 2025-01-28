using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    private GameObject[] enemies;
    public TextMeshProUGUI enemyCounter;

    private static EnemyCounter instance;
    public static EnemyCounter Instance
    {
        get { return instance; }
    }

    #region Unity Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // Enforce singleton pattern
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
            UpdateEnemyCounter();
        }
    }

    #endregion

    #region UI

    public void UpdateEnemyCounter()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Find all objects taged Damageable
        int enemyCount = enemies.Length;
        enemyCount = Mathf.Max(0, enemyCount); // Ensure the enemy count doesn't go below 0 if there are no enemies
        enemyCounter.text = "TO KILL: " + enemyCount.ToString(); // Ensure the enemy count doesn't go below 0 if there are no enemies
    }

    #endregion
}





