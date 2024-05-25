using Cinemachine;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera3D;
    private AllowInput input;
    
    private CharacterController controller;
    private HandleVisual visual;
    
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

    [Header("Coyote Time & Jump Buffering")]
    [SerializeField] float coyoteTimeCounter;
    [SerializeField] private float jumpBufferCounter;
    [Space]
    [SerializeField] private float coyoteTimeDuration = 0.2f;
    [SerializeField] private float jumpBufferingDuration = 0.2f;
    
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
        input = GameObject.Find("Game Manager").GetComponent<AllowInput>();
        
        controller = GetComponent<CharacterController>();
        visual = GetComponentInChildren<HandleVisual>();

        camera3D = GameObject.Find("3D Camera").GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (input.allowInput)
        {
            UpdateCoyoteTimeAndJumpBufferVariables();
            
            Move();
            Jump();
        }

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

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration;
        }
    }

    void ApplyGravity()
    {
        Debug.Log("Begin apply gravity " + velocity.y);
        
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
        
        Debug.Log("End apply gravity " + velocity.y);
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
        if ((isGrounded || coyoteTimeCounter > 0) && jumpBufferCounter > 0)
        {
            isJumping = true;
            visual.ApplyJumpSnS();
            
            velocity.y = Mathf.Sqrt(maxJumpHeight * -2 * gravity);
            
            if (controller.velocity.y > 0)
            {
                velocity.y = Mathf.Max(velocity.y - controller.velocity.y, 0f);
            }
            else if (controller.velocity.y < 0)
            {
                velocity.y += Mathf.Abs(controller.velocity.y);
            }
            
            controller.Move(velocity * Time.deltaTime);
            
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }

        if (Input.GetButtonUp("Jump")) isJumping = false;
    }

    void UpdateCoyoteTimeAndJumpBufferVariables()
    {
        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferingDuration;
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;
        if (coyoteTimeCounter > 0) coyoteTimeCounter -= Time.deltaTime;
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