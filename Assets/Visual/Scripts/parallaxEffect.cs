using UnityEngine;

public class ParallaxRotation : MonoBehaviour
{
    public enum ParallaxMode
    {
        Normal,
        Revert
    }

    public ParallaxMode mode = ParallaxMode.Normal; // Default mode
    public float rotationFactor = 0.1f; // Factor to determine how much the object should rotate
    public float smoothTime = 0.1f; // Smoothing factor for the rotation

    public float maxRotationX = 30f; // Maximum rotation limit for X-axis
    public float maxRotationY = 30f; // Maximum rotation limit for Y-axis
    public float maxRotationZ = 30f; // Maximum rotation limit for Z-axis

    private Vector3 initialRotation;
    private Quaternion originalRotation;
    public Camera currentCam;
    private Quaternion targetRotation;
    private bool parallaxEnabled;
    private Collider objectCollider;

    public bool pressESC = true; // Public bool to control Escape key functionality

    void Start()
    {
        initialRotation = transform.eulerAngles;
        originalRotation = transform.rotation;
        targetRotation = originalRotation;

        // Set initial state based on the mode
        parallaxEnabled = (mode == ParallaxMode.Normal);

        // Get the collider of the object
        objectCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (pressESC && Input.GetKeyDown(KeyCode.Escape))
        {
            parallaxEnabled = !parallaxEnabled;

            if (!parallaxEnabled)
            {
                // Snap back to the original rotation immediately
                transform.rotation = originalRotation;
                targetRotation = originalRotation;
            }
        }

        if (parallaxEnabled || !pressESC)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = currentCam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (objectCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Mouse is over the object, apply parallax effect
                mousePos.z = currentCam.nearClipPlane; // Ensure the Z value is set properly for ScreenToWorldPoint
                Vector3 worldMousePos = currentCam.ScreenToWorldPoint(mousePos);

                // Calculate the target rotation based on the mouse position
                float rotationX = (worldMousePos.y - currentCam.transform.position.y) * rotationFactor;
                float rotationY = -(worldMousePos.x - currentCam.transform.position.x) * rotationFactor;
                float rotationZ = (worldMousePos.z - currentCam.transform.position.z) * rotationFactor;

                // Clamp the rotations to the specified limits
                rotationX = Mathf.Clamp(rotationX, -maxRotationX, maxRotationX);
                rotationY = Mathf.Clamp(rotationY, -maxRotationY, maxRotationY);
                rotationZ = Mathf.Clamp(rotationZ, -maxRotationZ, maxRotationZ);

                // Set the target rotation
                targetRotation = Quaternion.Euler(initialRotation.x + rotationX, initialRotation.y + rotationY, initialRotation.z + rotationZ);
            }
            else
            {
                // Mouse is outside the object, reset to original rotation
                targetRotation = originalRotation;
            }
        }

        // Smoothly interpolate the rotation towards the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothTime * Time.deltaTime);
    }
}
