using Cinemachine;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField] private CinemachineVirtualCamera camera3D;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float groundMaxMoveSpeed = 35f;
    [SerializeField] private float airMaxMoveSpeed = 20f;
    [Space]
    public bool isGrounded;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private float checkDistance = 0.666f;
    [SerializeField] private LayerMask groundLayer;
    [Space]
    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private bool isJumping = false;

    private Vector3 desiredVelocity;
    private float acceleration;
    private Vector3 currentMoveVelocity;
    
    [Header("Gravity")]
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float currentGravityMultiplier = 1;
    [Space]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float downwardGravityMultiplier = 6f;
    [SerializeField] private float upwardGravityMultiplier = 1.7f;
    private float defaultGravityScale = 1;

    private bool hasRun = false;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();

        camera3D = GameObject.Find("3D Camera").GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        Move();
        Jump();

        CalculateMoveVelocity();
        GroundCheck();
    }

    void FixedUpdate()
    {
        ApplyGravity();
    }

    void GroundCheck()
    {
        if (controller.isGrounded) velocity.y = 0;

        RaycastHit hit;
        isGrounded = Physics.SphereCast(transform.position, checkRadius, Vector3.down, out hit, checkDistance, groundLayer);
        DebugDrawCapsule(transform.position, transform.position + Vector3.down * checkDistance, checkRadius, isGrounded ? Color.blue : Color.red);
    }

    void ApplyGravity()
    {
        currentGravityMultiplier = defaultGravityScale;

        if (velocity.y > 0 && isJumping)
        {
            currentGravityMultiplier = upwardGravityMultiplier;
        }
        else if (velocity.y < 0 || !isJumping)
        {
            currentGravityMultiplier = downwardGravityMultiplier;
        }
        //else if (controller.velocity.y == 0) currentGravityMultiplier = defaultGravityScale;
        
        velocity.y += gravity * currentGravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void CalculateMoveVelocity()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        Vector3 forward = camera3D.transform.forward;
        Vector3 right = camera3D.transform.right;
        forward.y = 0; right.y = 0;

        desiredVelocity = (forward * moveVertical + right * moveHorizontal) * Mathf.Max(moveSpeed, 0);
    }
    
    void Move()
    {
        acceleration = controller.isGrounded ? groundMaxMoveSpeed : airMaxMoveSpeed;
        float maxSpeedChange = acceleration;
        currentMoveVelocity.x = Mathf.MoveTowards(currentMoveVelocity.x, desiredVelocity.x, maxSpeedChange);
        currentMoveVelocity.z = Mathf.MoveTowards(currentMoveVelocity.z, desiredVelocity.z, maxSpeedChange);
        
        controller.Move(currentMoveVelocity * Time.deltaTime);

        /*float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = camera3D.transform.forward;
        Vector3 right = camera3D.transform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();

        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized * moveSpeed;
        controller.Move(movement * Time.deltaTime);*/
    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            
            velocity.y = Mathf.Sqrt(maxJumpHeight * -2 * gravity);
            
            if (controller.velocity.y > 0)
            {
                velocity.y = Mathf.Max(velocity.y - controller.velocity.y, 0f);
            }
            else if (controller.velocity.y < 0)
            {
                velocity.y += Mathf.Abs(controller.velocity.y);
            }
        }

        if (Input.GetButtonUp("Jump")) isJumping = false;
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