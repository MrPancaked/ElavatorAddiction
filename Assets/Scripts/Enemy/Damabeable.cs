//--------------------------------------------------------------------------------------------------
//  Description: Defines the interface for damageable objects, allowing them to receive damage and die.
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public interface Damageable
{
    public void TakeDamage(float damage); /// Applies damage to the object.
    public void Die(); /// Handles the death of the object.
}