using UnityEngine;

public class FollowMousePosition : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    public float zPosition = 0f; // The fixed Z position where the object should stay

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please ensure the main camera is assigned.");
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // Get the mouse position in screen space
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - zPosition); // Set the Z position relative to the camera

            // Convert the mouse position to world space
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            // Update the object's position to the mouse position
            transform.position = new Vector3(worldPosition.x, worldPosition.y, zPosition);
        }
    }
}