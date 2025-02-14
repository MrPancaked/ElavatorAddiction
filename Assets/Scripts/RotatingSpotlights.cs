//--------------------------------------------------------------------------------------------------
// Description: Rotates the spotlight lol
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSpotlights : MonoBehaviour
{
    [Header("References")]
    public Transform Spotlights;

    [Header("Vartiables")]
    public float rotationSpeed;

    void Update()
    {
        Spotlights.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}