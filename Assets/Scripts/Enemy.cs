using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyModel;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        health.Die += Die; //Subscribing to death events so that enemy object can perform death logic.
    }
    private void OnDisable()
    {
        health.Die -= Die; //Unsubscribing from death events so that we do not have memory leaks if the object gets destroyed.
    }
    public void Die()
    {
        GetComponent<DropLoot>().SpawnLoot(transform.position);
        enemyModel.SetActive(false);
    }
}