using UnityEngine;

public class RotateModel : MonoBehaviour
{
    private AllowInput input;
    
    private Rigidbody rb;
    private CharacterController controller;

    public bool transitioningToNextLevel = false;
    [Space]
    [SerializeField] private float rotateSpeed = 75f;
    [Space]
    [Tooltip("Set to enable if it's a 2D character")]
    [SerializeField] private bool steerAtCamera = false;
    [SerializeField, Range(0f, 1f)] private float steerPercentage = 1f;

    private Vector3 lastPosition;

    private void Start()
    {
        input = GameObject.Find("Game Manager").GetComponent<AllowInput>();
        rb = GetComponentInParent<Rigidbody>();
        controller = GetComponentInParent<CharacterController>();
        if (controller != null) lastPosition = transform.parent.position;
    }

    private void Update()
    {
        if (!steerAtCamera) RotateToDirection();
        else RotateToDirectionWithSteer();

        if (transitioningToNextLevel) ReadyForLevelTransition();
    }

    void RotateToDirection()
    {
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && input.allowInput)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // Camera-relative movement vectors
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = 0; // Remove any vertical component
            right.y = 0; // Remove any vertical component
            forward.Normalize(); // Ensure these vectors are normalized
            right.Normalize();

            // Desired direction based on camera perspective and player input
            Vector3 desiredDirection = (forward * moveVertical + right * moveHorizontal).normalized;

            if (desiredDirection.magnitude > 0.1f)
            {
                // Calculate the target rotation based on desired direction
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
        }
        
        /*Vector3 movementDirection = Vector3.zero; // Rotate to move direction

        if (rb != null && rb.velocity.magnitude > 0.1f)
        {
            movementDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        else if (controller != null && (transform.parent.position - lastPosition).magnitude > 0.1f)
        {
            Vector3 position = transform.parent.position - lastPosition;
            movementDirection = new Vector3(position.x, 0f, position.z).normalized;
            
            lastPosition = transform.parent.position;
        }

        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }*/
    }

    void RotateToDirectionWithSteer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Camera-relative movement vectors
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;  // Remove any vertical component
        right.y = 0;    // Remove any vertical component
        forward.Normalize();  // Ensure these vectors are normalized
        right.Normalize();

        // Desired direction based on camera perspective and player input
        Vector3 desiredDirection = (forward * moveVertical + right * moveHorizontal).normalized;

        if (desiredDirection.magnitude > 0.1f)
        {
            // Calculate the target rotation based on desired direction
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void ReadyForLevelTransition()
    {
        Vector3 toCameraDirection = (Camera.main.transform.position - transform.position).normalized;
        toCameraDirection.y = 0;
        
        Quaternion targetRotation = Quaternion.LookRotation(toCameraDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 45 * Time.deltaTime);
    }
}