using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private Vector3 JumpPadDirection;
    public float JumpPadForce;
    public float cooldownTime = 1f;  // Cooldown time in seconds

    private bool isCoolingDown = false;  // Flag to indicate if the cooldown is active
    private float cooldownTimer = 0f; // Timer for the cooldown

    public void Start()
    {
        JumpPadDirection = transform.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null && !isCoolingDown) // Check if a RigidBody exists, and if cooldown is active
        {
            other.GetComponent<Rigidbody>().AddForce(JumpPadDirection * JumpPadForce, ForceMode.Impulse);
            StartCooldown(); // Begin the cooldown process
        }
    }

    private void StartCooldown()
    {
        isCoolingDown = true; // Activates the cooldown
        cooldownTimer = 0f; // Sets the cooldown time to 0
    }

    private void Update()
    {
        if (isCoolingDown) // If the cooldown is active
        {
            cooldownTimer += Time.deltaTime; // Increment the timer

            if (cooldownTimer >= cooldownTime) // Check if the cooldown is done
            {
                isCoolingDown = false; // Deactivate the cooldown
                cooldownTimer = 0f; // reset the timer
            }
        }
    }
}