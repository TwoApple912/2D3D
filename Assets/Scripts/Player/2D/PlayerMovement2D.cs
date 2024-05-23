using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement2D : MonoBehaviour
{
    private AllowInput input;
    
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float groundMaxAcceleration = 35f;
    [SerializeField] private float airMaxAcceleration = 20f;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private float jumpHeight = 5f;

    private Vector3 desiredVelocity;
    private float acceleration;
    private float jumpSpeed;

    [Header("Step Climb & Slope Check")]
    [SerializeField] private float slopeThreshold = 45f;
    [SerializeField] private Vector3 slopeMoveDirection;
    /*[SerializeField] private bool lowerStepDetected;
    [Space]
    [SerializeField] private float stepHeight = 0.3f;
    [Tooltip("This is to determine how much character elevates upward when registered a stepable step. Updates every frames so it should create a smooth transition with small steps.")]
    [SerializeField] private float stepSmooth = 0.1f;
    [Tooltip("For some dark mischief, the CapsuleCollider works wih 0.51f while the BoxCollider works with 0.61f.")]
    [SerializeField] private float stepCheckDistance = 0.61f;
    [SerializeField] private float stepCheckUpperOffset = 0.1f;
    [Space]
    [SerializeField] private GameObject stepBase;
    [SerializeField] private GameObject stepUpper;
    [SerializeField] private GameObject stepLower;*/
    
    [Header("Ground Check")]
    public bool isGrounded;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckOffset = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float downwardGravityMultiplier = 5f;
    [SerializeField] private float upwardGravityMultiplier = 1.7f;
    private float defaultGravityScale = 1f;

    void Awake()
    {
        input = GameObject.Find("Game Manager").GetComponent<AllowInput>();
        
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        /*stepBase = transform.Find("stepBase").gameObject;
        stepUpper = transform.Find("stepUpper").gameObject;
        stepLower = transform.Find("stepLower").gameObject;

        stepUpper.transform.position = new Vector3(stepBase.transform.position.x, stepBase.transform.position.y + stepHeight, stepBase.transform.position.z);
        stepLower.transform.position = new Vector3(stepBase.transform.position.x, stepBase.transform.position.y - stepHeight, stepBase.transform.position.z);*/
    }

    void Update()
    {
        if (input.allowInput) Jump();
        
        GroundCheck();
        CalculateMoveVelocity();
        
        LockLocalRotation();
    }

    void FixedUpdate()
    {
        if (input.allowInput) Move();
        else rb.velocity = new Vector3(0, rb.velocity.y, 0);
        
        ApplyGravity();
        
        CheckSteps();
    }

    private void OnCollisionExit(Collision other)
    {
        //isGrounded = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        //NewGroundCheck(other);
    }

    private void OnCollisionStay(Collision other)
    {
        //NewGroundCheck(other);
    }

    void NewGroundCheck(Collision other)
    {
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.GetContact(i).normal;
            isGrounded |= normal.y >= 0.6f;

            //if (isGrounded) rb.useGravity = false;
        }
    }

    void GroundCheck()
    {
        Vector3 center = transform.position + capsuleCollider.center;
        Vector3 start = center + transform.forward * (capsuleCollider.height / 2 - capsuleCollider.radius);
        Vector3 end = center - transform.forward * (capsuleCollider.height / 2 - capsuleCollider.radius);
        start -= new Vector3(0, capsuleCollider.radius + groundCheckOffset, 0);
        end -= new Vector3(0, capsuleCollider.radius + groundCheckOffset, 0);

        RaycastHit hit;
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        if (Physics.SphereCast(start, capsuleCollider.radius, direction, out hit, distance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void ApplyGravity()
    {
        float gravityMultiplier = defaultGravityScale;
        
        if (rb.velocity.y > 0 && isJumping)
        {
            gravityMultiplier = upwardGravityMultiplier;
        }
        else if (rb.velocity.y < 0 || !isJumping)
        {
            gravityMultiplier = downwardGravityMultiplier;
        }
        //else if (rb.velocity.y == 0) gravityMultiplier = defaultGravityScale;
        
        rb.velocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
    }

    private RaycastHit slopeHit;
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    
    void CalculateMoveVelocity()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector3 localMovement = new Vector3(moveHorizontal, 0, 0);
        Vector3 moveDirection = transform.TransformDirection(localMovement);
        
        if (!OnSlope()) desiredVelocity = moveDirection * Mathf.Max(moveSpeed, 0);
        else
        {
            slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
            desiredVelocity = slopeMoveDirection * Mathf.Max(moveSpeed, 0);
        }
    }
    
    void Move()
    {
        Vector3 velocity = rb.velocity;
        
        acceleration = isGrounded ? groundMaxAcceleration : airMaxAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        rb.velocity = velocity;
    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            
            jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);

            if (rb.velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - rb.velocity.y, 0f);
            }
            else if (rb.velocity.y < 0)
            {
                jumpSpeed += Mathf.Abs(rb.velocity.y);
            }

            Vector3 velocity = rb.velocity;
            velocity.y += jumpSpeed;
            rb.velocity = velocity;
        }

        if (Input.GetButtonUp("Jump")) isJumping = false;
    }

    void CheckSteps()
    {
        /*RaycastHit hitRightBase;
        Vector3 baseRStart = stepBase.transform.position + transform.right * stepCheckDistance + capsuleCollider.height / 2 * transform.forward;
        Vector3 baseREnd = stepBase.transform.position + transform.right * stepCheckDistance + capsuleCollider.height / 2 * -transform.forward;
        if (Physics.Linecast(baseRStart, baseREnd, out hitRightBase))
        {
            RaycastHit hitRightUpper;
            if (!Physics.Linecast(baseRStart + transform.up.normalized * stepHeight + transform.right.normalized * 0.1f, baseREnd + transform.up.normalized * stepHeight + transform.right.normalized * stepCheckUpperOffset, out hitRightUpper))
            {
                rb.position += new Vector3(0, stepSmooth, 0);
                Debug.Log(hitRightBase.transform.name);
            }
        }

        RaycastHit hitLeftBase;
        Vector3 baseLStart = stepBase.transform.position - transform.right * stepCheckDistance + capsuleCollider.height / 2 * transform.forward;
        Vector3 baseLEnd = stepBase.transform.position - transform.right * stepCheckDistance + capsuleCollider.height / 2 * -transform.forward;
        if (Physics.Linecast(baseLStart, baseLEnd, out hitLeftBase))
        {
            RaycastHit hitLeftUpper;
            if (!Physics.Linecast(baseLStart + transform.up.normalized * stepHeight - transform.right.normalized * 0.1f, baseLEnd + transform.up.normalized * stepHeight - transform.right.normalized * stepCheckUpperOffset, out hitLeftUpper))
            {
                rb.position += new Vector3(0, stepSmooth, 0);
                Debug.Log(hitLeftBase.transform.name);
            }
        }

        Debug.DrawLine(baseRStart, baseREnd, Color.magenta);
        Debug.DrawLine(baseRStart + transform.up.normalized * stepHeight + transform.right.normalized * 0.1f, baseREnd + transform.up.normalized * stepHeight + transform.right.normalized * stepCheckUpperOffset, Color.white);*/
    }

    void LockLocalRotation()
    {
        transform.localRotation = Quaternion.identity;
    }

    void OnDrawGizmos()
    {
        if (capsuleCollider == null) return;

        Vector3 center = transform.position + capsuleCollider.center;
        Vector3 start = center + transform.forward * (capsuleCollider.height / 2 - capsuleCollider.radius);
        Vector3 end = center - transform.forward * (capsuleCollider.height / 2 - capsuleCollider.radius);
        start -= new Vector3(0, capsuleCollider.radius + groundCheckOffset, 0);
        end -= new Vector3(0, capsuleCollider.radius + groundCheckOffset, 0);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(start, capsuleCollider.radius);
        Gizmos.DrawWireSphere(end, capsuleCollider.radius);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);
    }
}
