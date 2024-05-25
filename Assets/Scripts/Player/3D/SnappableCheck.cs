using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappableCheck : MonoBehaviour
{
    private SwitchDimension dimension;
    private SnapDimensionToAxes _snapDimensionToAxes;
    private AllowInput input;
    
    private Collider collider;

    public bool allowSnap = true;
    [Space]
    [SerializeField] private LayerMask checkLayer;

    [Header("Obstruct Visualizer")]
    [SerializeField] private GameObject visualizerPrefab;
    [SerializeField] private float moveDuration = 1.5f;

    private Vector3 currentPointForVisual;
    private List<GameObject> visualizers = new List<GameObject>();

    private void Awake()
    {
        dimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();
        _snapDimensionToAxes = GameObject.Find("2D Dimension").GetComponent<SnapDimensionToAxes>();
        input = GameObject.Find("Game Manager").GetComponent<AllowInput>();
        
        collider = GetComponent<Collider>();

        checkLayer = LayerMask.GetMask("Default", "Level Element");
    }

    private void Update()
    {
        UpdateAllowSnapBool();
        DrawUnsnappableRay();
    }

    private void OnDisable()
    {
        foreach (GameObject visualizer in visualizers)
        {
            Destroy(visualizer);
        }
        visualizers.Clear();
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
        foreach (Vector3 point in points)
        {
            RaycastHit hit;
            Debug.DrawRay(point, direction * length, Color.yellow);
            if (Physics.Raycast(point, direction, out hit, length, checkLayer))
            {
                currentPointForVisual = hit.point;
                return false;
            }
        }
        return true;
    }

    void DrawUnsnappableRay()
    {
        if (Input.GetButtonDown("Switch Dimension") && input.allowInput && dimension.currentState == SwitchDimension.GameState.ThreeDimension && !allowSnap)
        {
            GameObject visualizer = Instantiate(visualizerPrefab, transform.position, Quaternion.identity);
            visualizers.Add(visualizer);

            StartCoroutine(MoveVisualizerToPosition(visualizer, currentPointForVisual, moveDuration));
        }
    }

    IEnumerator MoveVisualizerToPosition(GameObject visualizer, Vector3 target, float duration)
    {
        float time = 0;
        Vector3 startPosition = visualizer.transform.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            t = Mathf.SmoothStep(0, 1, t);

            visualizer.transform.position = Vector3.Lerp(startPosition, target, t);

            yield return null;
        }

        visualizer.transform.position = target;
        visualizer.GetComponent<ParticleSystem>().Stop();
        foreach (Transform child in visualizer.transform)
        {
            if (child.name == "Burst") child.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(2f);
        Destroy(visualizer);
        visualizers.Remove(visualizer);
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