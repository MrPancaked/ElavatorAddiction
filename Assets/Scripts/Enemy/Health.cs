//--------------------------------------------------------------------------------------------------
// Description: Manages the health of the game object and triggers a death event when health reaches zero.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using System;

public class Health : MonoBehaviour, Damageable
{
    #region Variables

    public float hp;         /// Current health points
    public event Action HandleDeathMethod; /// Event triggered when health reaches zero
    public event Action<float> TakeDamageMethod;

    #endregion

    #region Health Logic

    public void TakeDamage(float damage) /// Applies damage to the health points.
    {
        hp -= damage;
        TakeDamageMethod?.Invoke(damage);

        if (hp <= 0)
        {
            HandleDeath();
        }
    }

    void Damageable.Die() /// Triggers the OnDie virtual method for inheritance.
    {
        HandleDeath();
    }

    protected virtual void HandleDeath() // Renamed from OnDie
    {
        HandleDeathMethod?.Invoke();
    }

    #endregion
}