using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

    public float JumpPadForce;
    public float cooldownTime = 1f;  // Cooldown time in seconds

    private Vector3 JumpPadDirection;
    private bool isCoolingDown = false;  // Flag to indicate if the cooldown is active

    public void Start()
    {
        JumpPadDirection = transform.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>(); // Get the Rigidbody once

        if (rb != null && !isCoolingDown) // Check if a Rigidbody exists, and if cooldown is active
        {
            float appliedForce = JumpPadForce; // By default, apply the full force

            if (other.CompareTag("Damageable")) // Check if the colliding object is tagged "enemy"
            {
                appliedForce = JumpPadForce / 3f; // Reduce the force by half
            }

            rb.AddForce(JumpPadDirection * appliedForce, ForceMode.Impulse);
            StartCooldown(); // Begin the cooldown process
        }
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        Invoke(nameof(ResetCooldown), cooldownTime);
    }

    private void ResetCooldown()
    {
        isCoolingDown = false;
    }
}