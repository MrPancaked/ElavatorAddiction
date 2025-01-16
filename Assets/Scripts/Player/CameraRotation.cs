//--------------------------------------------------------------------------------------------------
// Description: Rotates the player towards the camera's forward direction.
//--------------------------------------------------------------------------------------------------
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraRotation : MonoBehaviour
{
    #region Variables

    [Header("Player Setup")]
    public Camera mainCamera; /// The main camera used for rotation
    public Rigidbody rb; /// The player's rigidbody to rotate

    #endregion

    #region Unity Methods

    private void FixedUpdate()
    {
        RotatePlayerTowardsCamera(); 
    }

    #endregion

    #region Rotation Logic

    private void RotatePlayerTowardsCamera() /// Rotate the player towards the camera
    {
        if (mainCamera != null && rb != null) /// If a camera and Rigidbody exists
        {
            Vector3 cameraForward = mainCamera.transform.forward; /// Get the camera's forward direction
            cameraForward.y = 0f; /// Ignore the y-axis rotation (so it's always on the horizontal plane)

            if (cameraForward != Vector3.zero)  /// If a forward direction exists
            {
                Quaternion newRotation = Quaternion.LookRotation(cameraForward);  /// Get the rotation needed to look in forward direction
                rb.MoveRotation(newRotation);  /// Rotate the player
            }
        }
    }

    #endregion
}