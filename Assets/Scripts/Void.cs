using UnityEngine;

public class Void : MonoBehaviour
{
    public Vector3 respawnLocation = new Vector3(0, 50, 0);
    [Space]
    [SerializeField] private GameObject camera2D;
    [SerializeField] private GameObject camera3D;

    private void OnCollisionEnter(Collision other)
    {
        // For 2D player character 
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 cameraOffset = camera2D.transform.position - other.transform.position;
            
            other.transform.position = respawnLocation;
            //camera2D.transform.position = respawnLocation + cameraOffset;
        }
    }
}