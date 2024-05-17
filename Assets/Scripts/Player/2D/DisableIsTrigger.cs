using UnityEngine;

public class DisableIsTrigger : MonoBehaviour
{
    private MeshCollider meshCollider;

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            meshCollider.isTrigger = false;
            meshCollider.convex = false;
            gameObject.layer = LayerMask.NameToLayer("Level Element");
            
            Destroy(GetComponent<DisableIsTrigger>());

            Debug.Log("Returned to normal");
        }
    }
}
