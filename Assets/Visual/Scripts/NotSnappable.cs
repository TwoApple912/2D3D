using UnityEngine;

public class NotSnappable : MonoBehaviour
{
    public GameObject objectToActivate;

    [SerializeField] private SnappableCheck snappableCheck;

    void Awake()
    {
        GameObject player3D = GameObject.Find("Player3D");
        if (player3D != null)
        {
            snappableCheck = player3D.GetComponent<SnappableCheck>();
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Switch Dimension") && snappableCheck != null && snappableCheck.allowSnap == false)
        {
            objectToActivate.SetActive(true);
        }
    }
}