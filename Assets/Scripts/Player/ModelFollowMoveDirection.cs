using UnityEngine;

public class ModelFollowMoveDirection : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterController controller;
    
    public float rotateSpeed = 75f;

    private Vector3 lastPosition;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        controller = GetComponentInParent<CharacterController>();
        if (controller != null) lastPosition = transform.parent.position;
    }

    private void Update()
    {
        Vector3 movementDirection = Vector3.zero;

        if (rb != null && rb.velocity.magnitude > 0.1f)
        {
            movementDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        else if (controller != null && (transform.parent.position - lastPosition).magnitude > 0.1f)
        {
            Vector3 position = transform.parent.position - lastPosition;
            movementDirection = new Vector3(position.x, 0f, position.z);
            
            lastPosition = transform.parent.position;
        }

        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
