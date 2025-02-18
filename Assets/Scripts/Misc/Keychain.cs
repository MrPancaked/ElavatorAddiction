using UnityEngine;

public class Keychain : MonoBehaviour
{
    public GameObject key; // Key object (child of the keychain)
    public GameObject keychainParent; // Parent object (keychain holder)

    public float springForce = 50f; // How "tight" the spring is
    public float damper = 5f; // How much the spring resists movement
    public float maxDistance = 2f; // Max distance the key can swing from the parent

    private SpringJoint springJoint;

    void Start()
    {
        // Ensure both the key and keychain parent have Rigidbody components
        if (key.GetComponent<Rigidbody>() == null)
            key.AddComponent<Rigidbody>();

        if (keychainParent.GetComponent<Rigidbody>() == null)
            keychainParent.AddComponent<Rigidbody>();

        // Set up the SpringJoint
        springJoint = key.AddComponent<SpringJoint>();
        springJoint.connectedBody = keychainParent.GetComponent<Rigidbody>(); // Attach the key to the parent
        springJoint.spring = springForce; // Set the spring force (tension)
        springJoint.damper = damper; // Set the damper for how much the spring resists movement
        springJoint.maxDistance = maxDistance; // Set how far the key can swing from the parent
        springJoint.autoConfigureConnectedAnchor = false; // Manually adjust anchor positions
        springJoint.anchor = Vector3.zero; // Key's anchor point (relative to the key)
        springJoint.connectedAnchor = Vector3.zero; // Parent's anchor point (relative to the parent)
    }

    void Update()
    {
        // Optional: Update key's position based on some input or force here
        // For example, you can move the parent around with the mouse or keyboard

        float moveSpeed = 5f;
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
        float vertical = Input.GetAxis("Vertical") * moveSpeed;

        keychainParent.transform.Translate(new Vector3(horizontal, 0, vertical) * Time.deltaTime);
    }
}
