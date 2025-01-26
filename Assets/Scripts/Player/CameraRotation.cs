//--------------------------------------------------------------------------------------------------
// Description: Rotates the player towards the camera's forward direction.
//--------------------------------------------------------------------------------------------------
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraRotation : MonoBehaviour
{
    #region Variables

    //prive shit
    private Rigidbody rb; /// The player's rigidbody to rotate
    private Camera playerCamera; /// The main camera used for rotation

    #endregion

    #region Unity Methods

    private void Awake()  /// Makes sure this object survives the scene transition, and that there is only one.
    {
        playerCamera = Camera.main; // Find the main camera
        rb = GetComponent<Rigidbody>(); // Find the Rigidbody
    }

    private void FixedUpdate()
    {
        RotatePlayerTowardsCamera(); 
    }

    #endregion

    #region Rotation Logic

    private void RotatePlayerTowardsCamera() /// Rotate the player towards the camera
    {
        Vector3 cameraForward = playerCamera.transform.forward; /// Get the camera's forward direction
        cameraForward.y = 0f; /// Ignore the y-axis rotation (so it's always on the horizontal plane)

        if (cameraForward != Vector3.zero)  /// If a forward direction exists
        {
            Quaternion newRotation = Quaternion.LookRotation(cameraForward);  /// Get the rotation needed to look in forward direction
            rb.MoveRotation(newRotation);  /// Rotate the player
        }
    }

    #endregion
}