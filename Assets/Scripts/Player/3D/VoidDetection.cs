using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidDetection : MonoBehaviour
{
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.GetComponent<Void>() != null)
        {
            Debug.Log("You");
            transform.position = hit.gameObject.GetComponent<Void>().respawnLocation;
         }
    }
}
