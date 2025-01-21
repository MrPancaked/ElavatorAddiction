//--------------------------------------------------------------------------------------------------
// Description: Manages the health of the game object and triggers a death event when health reaches zero.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using System;

public class Health : MonoBehaviour, Damageable
{
    #region Variables

    public float hp;         /// Current health points
    public event Action Die; /// Event triggered when health reaches zero

    #endregion

    #region Health Logic

    public void TakeDamage(float damage) /// Applies damage to the health points.
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Damageable.Die() /// Triggers the OnDie virtual method for inheritance.
    {
        OnDie();
    }

    protected virtual void OnDie() /// Invokes the Die event.
    {
        Die?.Invoke();
    }

    #endregion
}