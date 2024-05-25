using System;
using System.Diagnostics;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private SwitchDimension dimension;
    
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;

    [SerializeField] private float speed = 5f;

    private float progress = 0f;
    private bool movingToB = true;
    public Vector3 lastPosition;
    [SerializeField] private Transform player2D;

    private void Awake()
    {
        dimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (dimension.currentState == SwitchDimension.GameState.ThreeDimension) player2D = null;
    }

    private void FixedUpdate()
    {
        MovePlatform();

        CalculateMovementDelta();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player2D = other.transform;
            //player2D.position = new Vector3(transform.position.x, player2D.position.y, transform.position.z);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player2D = null;
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
        if (player2D != null)
        {
            player2D.transform.position += deltaPosition;
            player2D.GetComponent<PlayerMovement2D>().isGrounded = true;
        }
        
        lastPosition = transform.position;
    }
}