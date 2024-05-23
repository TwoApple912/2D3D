using System;
using UnityEngine;

public class MovingPlatformCheck : MonoBehaviour
{
    private CharacterController controller;
    
    private Transform platform;
    private Vector3 lastPlatformPosition;

    private void Awake()
    {
        controller = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        if (platform != null)
        {
            Debug.Log("Move");
            Vector3 delta = platform.position - lastPlatformPosition;
            controller.Move(delta);
            
            lastPlatformPosition = platform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            Debug.Log("Entered Moving Platform: " + other.name);
            platform = other.transform;
            lastPlatformPosition = platform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            Debug.Log("Exited Moving Platform");
            platform = null;
        }
    }
}
