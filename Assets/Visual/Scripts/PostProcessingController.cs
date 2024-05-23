using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class AdjustChromaticAberration : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    public float transitionDuration = 1.0f; 
    public float returnDuration = 1.0f; 
    private bool isNegative = true;
    public SnappableCheck snapable;

    void Start()
    {
        // Automatically find the SnappableCheck component on the "Player3D" GameObject
        GameObject player3D = GameObject.Find("Player3D");
        if (player3D != null)
        {
            snapable = player3D.GetComponent<SnappableCheck>();
            if (snapable == null)
            {
                Debug.LogError("SnappableCheck component not found on Player3D GameObject.");
            }
        }
        else
        {
            Debug.LogError("Player3D GameObject not found.");
        }

        if (postProcessVolume == null)
        {
           return;
        }
        if (!postProcessVolume.profile.TryGetSettings(out chromaticAberration))
        {
            Debug.LogError("Chromatic Aberration settings not found on the PostProcessVolume.");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Switch Dimension"))
        {
            if (snapable != null && snapable.allowSnap)
            {
                if (isNegative)
                {
                    StartCoroutine(AdjustChromaticAberrationCoroutine(0, 1));
                }
                else
                {
                    StartCoroutine(AdjustChromaticAberrationCoroutine(0, 1));
                }

                isNegative = !isNegative;
            }
        }
    }

    IEnumerator AdjustChromaticAberrationCoroutine(float startValue, float endValue)
    {
        yield return StartCoroutine(ChangeIntensity(startValue, endValue, transitionDuration));
        yield return StartCoroutine(ChangeIntensity(endValue, startValue, returnDuration));
    }

    IEnumerator ChangeIntensity(float startValue, float endValue, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(startValue, endValue, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        chromaticAberration.intensity.value = endValue;
    }
}
