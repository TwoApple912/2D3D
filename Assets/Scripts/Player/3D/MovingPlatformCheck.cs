using System;
using UnityEngine;

public class MovingPlatformCheck : MonoBehaviour
{
    private CharacterController controller;
    private PlayerMovement3D movement3D;
    
    private Transform platform;
    private Vector3 lastPlatformPosition;

    private void Awake()
    {
        controller = GetComponentInParent<CharacterController>();
        movement3D = GetComponentInParent<PlayerMovement3D>();
    }

    private void Update()
    {
        if (platform != null)
        {
            Debug.Log("Move");
            
            Vector3 delta = platform.position - lastPlatformPosition;
            delta.y = 0;
            
            controller.Move(delta);
            
            lastPlatformPosition = platform.position;
            movement3D.isGrounded = true;
        }
    }

    private void OnDisable()
    {
        platform = null;
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
