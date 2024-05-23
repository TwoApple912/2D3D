using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;

    [SerializeField] private float speed = 5f;

    private float progress = 0f;
    private bool movingToB = true;
    public Vector3 lastPosition;
    [SerializeField] private GameObject player;
    
    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        MovePlatform();

        CalculateMovementDelta();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;
        }
    }

    void MovePlatform()
    {
        float easedProgress = EaseInOutSine(progress);
        transform.position = Vector3.Lerp(pointA, pointB, easedProgress);
        
        if (movingToB)
            progress += speed * Time.deltaTime / Vector3.Distance(pointA, pointB);
        else
            progress -= speed * Time.deltaTime / Vector3.Distance(pointA, pointB);
        
        if (progress >= 1f && movingToB)
        {
            progress = 1f;
            movingToB = false;
        }
        else if (progress <= 0f && !movingToB)
        {
            progress = 0f;
            movingToB = true;
        }
    }
    
    float EaseInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
    }

    void CalculateMovementDelta()
    {
        Vector3 deltaPosition = transform.position - lastPosition;
        if (player != null)
        {
            player.transform.position += deltaPosition;
        }
        
        lastPosition = transform.position;
    }
}