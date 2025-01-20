using UnityEngine;
using System;

public class Health : MonoBehaviour, Damageable
{
    public float hp;
    public event Action Die;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Damageable.Die()
    {
        OnDie();
    }

    protected virtual void OnDie()
    {
        Die?.Invoke();
    }
}