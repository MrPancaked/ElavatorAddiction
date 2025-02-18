//--------------------------------------------------------------------------------------------------
// Description: Launches the player when he steps on the jump pad:)
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class JumpPad : MonoBehaviour
{
    public float JumpPadForce;
    public float cooldownTime = 1f;  // Cooldown time in seconds
    public EventReference JumppadSound;

    private Vector3 JumpPadDirection;
    private bool isCoolingDown = false;  // Flag to indicate if the cooldown is active

    public void Start()
    {
        JumpPadDirection = transform.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>(); // Get the Rigidbody

        if (rb != null && !isCoolingDown) // Null and cooldown check
        {
            float appliedForce = JumpPadForce; // Normal force

            if (other.CompareTag("Damageable")) // Check if not enemy
            {
                appliedForce = JumpPadForce / 3f; // Low force

            }

            rb.AddForce(JumpPadDirection * appliedForce, ForceMode.Impulse); // Launch em up

            StartCooldown(); // Cooldown
        }
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        AudioManager.instance.PlayOneShot(JumppadSound, this.transform.position);
        Invoke(nameof(ResetCooldown), cooldownTime);
    }

    private void ResetCooldown()
    {
        isCoolingDown = false;
    }
}