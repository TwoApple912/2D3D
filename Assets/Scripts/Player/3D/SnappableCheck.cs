
using UnityEngine;

public class SnappableCheck : MonoBehaviour
{
    private SnapDimensionToAxes _snapDimensionToAxes;
    private Collider collider;

    public bool allowSnap = true;
    [Space]
    [SerializeField] private LayerMask checkLayer;

    private void Start()
    {
        _snapDimensionToAxes = GameObject.Find("2D Dimension").GetComponent<SnapDimensionToAxes>();
        collider = GetComponent<Collider>();

        checkLayer = LayerMask.GetMask("Default", "Level Element");
    }

    private void Update()
    {
        UpdateAllowSnapBool();
    }

    void UpdateAllowSnapBool()
    {
        Vector3 top = new Vector3(collider.bounds.center.x, collider.bounds.max.y, collider.bounds.center.z);
        Vector3 bottom = new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z);
        Vector3 front = new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.max.z);
        Vector3 back = new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.min.z);
        Vector3 left = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.center.z);
        Vector3 right = new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.center.z);
        Vector3[] points = new Vector3[] { top, bottom, front, back, left, right };

        Vector3 direction2DDimension = _snapDimensionToAxes.gameObject.transform.forward;
        direction2DDimension.y = 0;

        float angle = Mathf.Atan2(direction2DDimension.z, direction2DDimension.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 90) * 90;
        float radians = snappedAngle * Mathf.Deg2Rad;
        
        Vector3 snappedDirection = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        Debug.DrawRay(transform.position, snappedDirection, Color.red);

        allowSnap = CheckRaycasts(points, snappedDirection, 120);
        if (allowSnap) allowSnap = CheckRaycasts(points, -snappedDirection, 120);
    }

    bool CheckRaycasts(Vector3[] points, Vector3 direction, float length)
    {
        int hitCount = 0;
        
        foreach (Vector3 point in points)
        {
            Debug.DrawRay(point, direction * length, Color.yellow);
            if (Physics.Raycast(point, direction, length, checkLayer))
            {
                hitCount++;
            }
        }

        if (hitCount > points.Length / 2)
        {
            return false;
        }
        else return true;
    }

    private void OnDrawGizmos()
    {
        /*Vector3 top = new Vector3(collider.bounds.center.x, collider.bounds.max.y, collider.bounds.center.z);
        Vector3 bottom = new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z);
        Vector3 front = new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.max.z);
        Vector3 back = new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.min.z);
        Vector3 left = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.center.z);
        Vector3 right = new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.center.z);
        
        Gizmos.DrawSphere(top, 0.1f);
        Gizmos.DrawSphere(bottom, 0.1f);
        Gizmos.DrawSphere(front, 0.1f);
        Gizmos.DrawSphere(back, 0.1f);
        Gizmos.DrawSphere(left, 0.1f);
        Gizmos.DrawSphere(right, 0.1f);*/
    }
}