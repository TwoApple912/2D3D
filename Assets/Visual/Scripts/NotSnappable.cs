using UnityEngine;

public class NotSnappable : MonoBehaviour
{
    public GameObject objectToActivate;

    [SerializeField] private SnappableCheck snappableCheck; 

    void Update()
    {
        if (Input.GetButtonDown("Switch Dimension") && snappableCheck.allowSnap == false)
        {
            objectToActivate.SetActive(true);
        }
    }
}