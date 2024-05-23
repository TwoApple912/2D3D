using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public GameObject mainCamera;

    [Header("Shake Settings")]
    public float defaultDuration = 0.5f;
    public float defaultStrength = 0.3f;
    public float xOffset = 0.0f;
    public float yOffset = 0.0f;

    // Method to trigger shake with default values
    public void TriggerShake()
    {
        StartCoroutine(Shake(defaultDuration, defaultStrength, xOffset, yOffset));
    }

    // Method to trigger shake with custom values
    public void TriggerShake(float duration, float strength)
    {
        StartCoroutine(Shake(duration, strength, xOffset, yOffset));
    }

    // Method to trigger shake with more parameters
    public void TriggerShake(float duration, float strength, float xOffset, float yOffset)
    {
        StartCoroutine(Shake(duration, strength, xOffset, yOffset));
    }

    private IEnumerator Shake(float duration, float strength, float xOffset, float yOffset)
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1f, 1f) * strength + xOffset;
            float y = Random.Range(-1f, 1f) * strength + yOffset;

            mainCamera.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }
}