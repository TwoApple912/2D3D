using Cinemachine;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField] private CinemachineVirtualCamera camera3D;
    
    [Header("Movement")]
    public float moveSpeed = 5f;
    [Space]
    public bool isGrounded;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private float checkDistance = 0.666f;
    [SerializeField] private LayerMask groundLayer;
    [Space]
    [SerializeField] private float maxJumpHeight = 3f;
    
    [Header("Gravity")]
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float currentGravityMultiplier = 1f;
    [Space]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpGravityMultiplier = 0.75f;
    [SerializeField] private float maxFallGravityMultiplier = 2f;
    [SerializeField] private float gravityIncreaseRate = 1f;

    private bool hasRun = false;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        Jump();
        
        GroundCheck();
    }

    void FixedUpdate()
    {
        ApplyGravity();
    }

    void GroundCheck()
    {
        RaycastHit hit;
        isGrounded = Physics.SphereCast(transform.position, checkRadius, Vector3.down, out hit, checkDistance, groundLayer);
        DebugDrawCapsule(transform.position, transform.position + Vector3.down * checkDistance, checkRadius, isGrounded ? Color.blue : Color.red);
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            // Increase the gravity multiplier gradually until it reaches the maximum
            if (currentGravityMultiplier < maxFallGravityMultiplier)
            {
                currentGravityMultiplier += gravityIncreaseRate * Time.deltaTime;
                currentGravityMultiplier = Mathf.Min(currentGravityMultiplier, maxFallGravityMultiplier);
            }
        }
        else if (isGrounded && !(velocity.y > 0))
        {
            currentGravityMultiplier = 1;
            velocity.y = 0;
        }
        
        velocity.y += gravity * currentGravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = camera3D.transform.forward;
        Vector3 right = camera3D.transform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();
        
        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized * moveSpeed;
        controller.Move(movement * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(maxJumpHeight * -2 * gravity);
            currentGravityMultiplier = jumpGravityMultiplier;
        }
    }

    void DebugDrawCapsule(Vector3 start, Vector3 end, float radius, Color color)
    {
        Vector3 up = (end - start).normalized * radius;
        Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
        Vector3 right = Vector3.Cross(up, forward).normalized * radius;

        // Draw top sphere
        for (int i = 0; i < 360; i += 20)
        {
            Debug.DrawLine(start + Quaternion.Euler(0, i, 0) * right, start + Quaternion.Euler(0, i + 20, 0) * right, color);
            Debug.DrawLine(start + Quaternion.Euler(i, 0, 0) * right, start + Quaternion.Euler(i + 20, 0, 0) * right, color);
            Debug.DrawLine(start + Quaternion.Euler(0, 0, i) * right, start + Quaternion.Euler(0, 0, i + 20) * right, color);
        }

        // Draw end sphere
        for (int i = 0; i < 360; i += 20)
        {
            Debug.DrawLine(end + Quaternion.Euler(0, i, 0) * right, end + Quaternion.Euler(0, i + 20, 0) * right, color);
            Debug.DrawLine(end + Quaternion.Euler(i, 0, 0) * right, end + Quaternion.Euler(i + 20, 0, 0) * right, color);
            Debug.DrawLine(end + Quaternion.Euler(0, 0, i) * right, end + Quaternion.Euler(0, 0, i + 20) * right, color);
        }

        // Draw lines between spheres
        for (int i = 0; i < 360; i += 60)
        {
            Debug.DrawLine(start + Quaternion.Euler(0, i, 0) * right, end + Quaternion.Euler(0, i, 0) * right, color);
            Debug.DrawLine(start + Quaternion.Euler(i, 0, 0) * right, end + Quaternion.Euler(i, 0, 0) * right, color);
            Debug.DrawLine(start + Quaternion.Euler(0, 0, i) * right, end + Quaternion.Euler(0, 0, i) * right, color);
        }
    }
}