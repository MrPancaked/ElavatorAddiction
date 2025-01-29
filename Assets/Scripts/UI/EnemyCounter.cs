using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    public TextMeshProUGUI enemyCounter;
    public int enemyCount;

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
            InitiatlizeEnemyCount();
        }
    }

    private void Start()
    {
        InitiatlizeEnemyCount();
    }
    #endregion

    #region UI

    public void InitiatlizeEnemyCount()
    {
        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyArray)
        {
            if (enemy.activeInHierarchy)  // Only count active enemies
            {
                enemyCount++;
                
            }
        }
        enemyCounter.text = "TO KILL: " + enemyCount.ToString(); // Ensure the enemy count doesn't go below 0 if there are no enemies
    }

    public void UpdateEnemyCounter()
    {
        enemyCounter.text = "TO KILL: " + enemyCount.ToString(); // Ensure the enemy count doesn't go below 0 if there are no enemies
    }

    #endregion
}





