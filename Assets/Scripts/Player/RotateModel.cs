using UnityEngine;

public class RotateModel : MonoBehaviour
{
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
        Vector3 movementDirection = Vector3.zero;

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
        }
    }

    void RotateToDirectionWithSteer()
    {
        Vector3 movementDirection = Vector3.zero;
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

        Vector3 toCameraDirection = (Camera.main.transform.position - transform.position).normalized;
        toCameraDirection.y = 0;
        
        if (movementDirection != Vector3.zero)
        {
            Vector3 direction = Vector3.Lerp(movementDirection, toCameraDirection, steerPercentage).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
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