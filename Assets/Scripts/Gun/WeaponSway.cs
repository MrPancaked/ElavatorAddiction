//--------------------------------------------------------------------------------------------------
// Description: Controls the weapon's sway effect based on mouse movement.
//--------------------------------------------------------------------------------------------------
using System;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    #region Variables

    [Header("Sway Settings")]
    public float verticalSway;  /// Amount of vertical sway.
    public float horizontalSway; /// Amount of horizontal sway.
    public float SwaySmoothness; /// Speed of the sway effect.

    #endregion

    #region Sway Logic

    private void Update() /// Handles weapon sway based on mouse movement.
    {   
        float mouseX = Input.GetAxisRaw("Mouse X") * horizontalSway; // get mouse input
        float mouseY = Input.GetAxisRaw("Mouse Y") * verticalSway;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right); // calculate target rotation
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;  
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, SwaySmoothness * Time.deltaTime); // rotate
    }

    #endregion
}