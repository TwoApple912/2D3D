using System.Collections;
using UnityEngine;

public class HeadToHead : MonoBehaviour
{
    private TrailRenderer trailScript;
    
    [SerializeField] private Transform head;
    [Space]
    [SerializeField] private float trailScriptDisableDuration = 0.6f;

    private void Awake()
    {
        trailScript = GetComponent<TrailRenderer>();
        
        if (GetComponentInParent<PlayerMovement2D>()) head = FindObjectOfType<PlayerMovement3D>().transform.Find("Model/Head");
        else head = FindObjectOfType<PlayerMovement2D>().transform.Find("Model/Head");
    }

    private void OnEnable()
    {
        DisableTrailScript(trailScriptDisableDuration);
    }

    IEnumerator DisableTrailScript(float duration)
    {
        trailScript.enabled = false;

        yield return new WaitForSeconds(duration);
        
        trailScript.enabled = true;
    }
}