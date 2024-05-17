using System;
using UnityEngine;

public class Character2DSpecificFunctionsCapsuleCollider  : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundOffset = 0.1f; // Adjust this to set how far below the actual collider ends the SphereCast starts and ends
    [Space]
    public RaycastHit standingPosition;

    private bool feetObjectDetected = false;
    private int levelElementLayer;

    void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        levelElementLayer = LayerMask.GetMask("Level Element");
    }

    void Update()
    {
        GetFeetObject();
        //MoveToFeetObject();
    }

    void GetFeetObject()
    {
        Vector3 center = transform.position + capsuleCollider.center;
        Vector3 start = center + transform.forward * (capsuleCollider.height / 2 + capsuleCollider.radius);
        Vector3 end = center - transform.forward * (capsuleCollider.height / 2 + capsuleCollider.radius);

        // The direction from start to end (normalized) and the distance between them
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        // Adjust the start and end points downward by the radius + groundOffset
        start -= new Vector3(0, capsuleCollider.radius + groundOffset, 0);
        end -= new Vector3(0, capsuleCollider.radius + groundOffset, 0);

        RaycastHit hit;
        if (Physics.SphereCast(start, capsuleCollider.radius, direction, out hit, distance, groundLayer))
        {
            standingPosition = hit;
        }
    }

    public void DisableOverlappingColliders()
    {
        Vector3 capsuleTop = transform.TransformPoint(capsuleCollider.center + Vector3.forward * capsuleCollider.height / 2);
        Vector3 capsuleBottom = transform.TransformPoint(capsuleCollider.center - Vector3.forward * capsuleCollider.height / 2);
        Collider[] overlapColliders = Physics.OverlapCapsule(capsuleTop, capsuleBottom, capsuleCollider.radius - 0.1f, levelElementLayer);
        
        foreach (var collider in overlapColliders)
        {
            MeshCollider meshCollider = collider.gameObject.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.convex = true;
                meshCollider.isTrigger = true;
                collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                collider.gameObject.AddComponent<DisableIsTrigger>();
            }
        }
    }

    void OnDrawGizmos()
    {
        if (capsuleCollider == null) return;

        Vector3 center = transform.position + capsuleCollider.center;
        Vector3 start = center + transform.forward * (capsuleCollider.height / 2 + capsuleCollider.radius);
        Vector3 end = center - transform.forward * (capsuleCollider.height / 2 + capsuleCollider.radius);

        // Adjust the start and end points downward by the radius + groundOffset for visualization
        start -= new Vector3(0, groundOffset, 0);
        end -= new Vector3(0, groundOffset, 0);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(start, capsuleCollider.radius);
        Gizmos.DrawWireSphere(end, capsuleCollider.radius);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(standingPosition.point, 0.25f);
    }
}