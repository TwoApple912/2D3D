using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement2DCapsuleCollider : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxJumpHeight = 3f;

    [Header("Step Climb")]
    [SerializeField] private bool lowerStepDetected;
    [Space]
    [SerializeField] private float stepHeight = 0.3f;
    [Tooltip("This is to determine how much character elevates upward when registered a stepable step. Updates every frames so it should create a smooth transition with small steps.")]
    [SerializeField] private float stepSmooth = 0.1f;
    [SerializeField] private float stepCheckDistance = 0.3f;
    [Space]
    [SerializeField] private GameObject stepBase;
    [SerializeField] private GameObject stepUpper;
    [SerializeField] private GameObject stepLower;
    
    [Header("Ground Check")]
    public bool isGrounded;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckOffset = 0.1f;
    
    [Header("Gravity")]
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float currentGravityMultiplier = 1f;
    [Space]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpGravityMultiplier = 0.75f;
    [SerializeField] private float maxFallGravityMultiplier = 1.25f;
    [SerializeField] private float gravityIncreaseRate = 0.05f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        stepBase = transform.Find("stepBase").gameObject;
        stepUpper = transform.Find("stepUpper").gameObject;
        stepLower = transform.Find("stepLower").gameObject;

        stepUpper.transform.position = new Vector3(stepBase.transform.position.x, stepBase.transform.position.y + stepHeight, stepBase.transform.position.z);
        stepLower.transform.position = new Vector3(stepBase.transform.position.x, stepBase.transform.position.y - stepHeight, stepBase.transform.position.z);
    }

    void Update()
    {
        Jump();
        GroundCheck();
        LockLocalRotation();
    }

    void FixedUpdate()
    {
        Move();
        ApplyGravity();
        CheckSlopeAndSteps();
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
        if (!isGrounded)
        {
            if (currentGravityMultiplier < maxFallGravityMultiplier)
            {
                currentGravityMultiplier += gravityIncreaseRate * Time.fixedDeltaTime;
                currentGravityMultiplier = Mathf.Min(currentGravityMultiplier, maxFallGravityMultiplier);
            }
        }
        else
        {
            currentGravityMultiplier = 1f;
            if (velocity.y < 0) velocity.y = 0;
        }

        velocity.y += gravity * currentGravityMultiplier * Time.fixedDeltaTime;
        rb.velocity = new Vector3(rb.velocity.x, velocity.y, 0);
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        Vector3 localMovement = new Vector3(moveHorizontal, 0, 0);
        Vector3 globalMovement = transform.TransformDirection(localMovement) * moveSpeed;
        rb.MovePosition(rb.position + globalMovement * Time.fixedDeltaTime);
    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(maxJumpHeight * -2 * gravity);
            rb.velocity = new Vector3(rb.velocity.x, velocity.y, 0);
            currentGravityMultiplier = jumpGravityMultiplier;
        }
    }

    void CheckSlopeAndSteps()
    {
        RaycastHit hitRightBase;
        Vector3 baseRStart = stepBase.transform.position + transform.right * stepCheckDistance + capsuleCollider.height / 2 * transform.forward;
        Vector3 baseREnd = stepBase.transform.position + transform.right * stepCheckDistance + capsuleCollider.height / 2 * -transform.forward;
        if (Physics.Linecast(baseRStart, baseREnd, out hitRightBase))
        {
            RaycastHit hitRightUpper;
            if (!Physics.Linecast(baseRStart + transform.up.normalized * stepHeight + transform.right.normalized * 0.1f, baseREnd + transform.up.normalized * stepHeight + transform.right.normalized * 0.1f, out hitRightUpper))
            {
                rb.position += new Vector3(0, stepSmooth, 0);
            }
        }
        
        RaycastHit hitLeftBase;
        Vector3 baseLStart = stepBase.transform.position - transform.right * stepCheckDistance + capsuleCollider.height / 2 * transform.forward;
        Vector3 baseLEnd = stepBase.transform.position - transform.right * stepCheckDistance + capsuleCollider.height / 2 * -transform.forward;
        if (Physics.Linecast(baseLStart, baseLEnd, out hitLeftBase))
        {
            RaycastHit hitLeftUpper;
            if (!Physics.Linecast(baseLStart + transform.up.normalized * stepHeight - transform.right.normalized * 0.1f, baseLEnd + transform.up.normalized * stepHeight - transform.right.normalized * 0.1f, out hitLeftUpper))
            {
                rb.position += new Vector3(0, stepSmooth, 0);
            }
        }
        
        Debug.DrawLine(baseRStart, baseREnd, Color.magenta);
        Debug.DrawLine(baseRStart + transform.up.normalized * stepHeight + transform.right.normalized * 0.1f, baseREnd + transform.up.normalized * stepHeight + transform.right.normalized * 0.1f, Color.white);
    }

    void LockLocalRotation()
    {
        //transform.localRotation = Quaternion.identity;
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
