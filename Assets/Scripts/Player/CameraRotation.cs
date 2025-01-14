using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraRotation : MonoBehaviour
{
    public Camera mainCamera;
    public Rigidbody rb;

    private void FixedUpdate()
    {     
        RotatePlayerTowardsCamera();
    }

    private void RotatePlayerTowardsCamera()// Rotate the player towards the camera every physics update
    {
        if (mainCamera != null && rb != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0f; // Ignore the y-axis rotation

            if (cameraForward != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(cameraForward);
                rb.MoveRotation(newRotation);
            }
        }
    }
}