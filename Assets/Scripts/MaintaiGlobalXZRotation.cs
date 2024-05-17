using UnityEngine;

public class MaintaiGlobalXZRotation : MonoBehaviour
{
    private float initialGlobalRotationY;

    void Start()
    {
        initialGlobalRotationY = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        
        currentEulerAngles.x = 0;
        currentEulerAngles.z = 0;
        
        transform.rotation = Quaternion.Euler(currentEulerAngles);
    }
}
