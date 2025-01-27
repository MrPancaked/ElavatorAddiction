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
        enemies = GameObject.FindGameObjectsWithTag("Damageable");
        enemyCounter.text = "Enemies: " + enemies.Length;
    }
}
