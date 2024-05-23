using System;
using System.Collections;
using UnityEngine;

public class ShaderControl : MonoBehaviour
{
    [SerializeField] private Material platformMaterial;
    [Space]
    [SerializeField] private string artisticClampMinPropertyName = "_DrawnClampMin";
    [SerializeField] private float targetArtisticClampMin = 0.21f;
    [SerializeField] private float artisticClampMinLerpDuration = 0.5f;
    [Space]
    [SerializeField] private string brightPropertyName = "_GoochBrightColor";
    [SerializeField] private Color targetBrightColor = Color.black;
    [SerializeField] private float brightLerpDuration = 2f;
    [SerializeField] private float brightStayDuration = 6f;

    private Coroutine artisticClampMinCoroutine;

    private void Start()
    {
        StartCoroutine(DarkColorPulse());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Switch Dimension") && artisticClampMinCoroutine == null)
        {
             artisticClampMinCoroutine = StartCoroutine(ArtisticClampMinPulse(artisticClampMinPropertyName, targetArtisticClampMin, artisticClampMinLerpDuration));
        }
    }

    /// <summary>
    /// Coroutine to lerp a material's float property to a target value over a specified duration.
    /// </summary>
    /// <param name="material">The material to modify.</param>
    /// <param name="propertyName">The name of the float property to lerp.</param>
    /// <param name="targetValue">The target value to lerp to.</param>
    /// <param name="duration">The duration over which to perform the lerp.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator ArtisticClampMinPulse(string propertyName, float targetValue, float duration)
    {
        Debug.Log("Fuck uou");
        
        float initialVal = platformMaterial.GetFloat(propertyName);
        float elapsedTime = 0;

        // Lerp to the target value
        while (elapsedTime < duration)
        {
            float newValue = Mathf.Lerp(initialVal, targetValue, elapsedTime / duration);
            platformMaterial.SetFloat(propertyName, newValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        platformMaterial.SetFloat(propertyName, targetValue); // Ensure it hits the target value

        // Reset elapsed time for the return lerp
        elapsedTime = 0;

        // Lerp back to the initial value
        while (elapsedTime < duration)
        {
            float newValue = Mathf.Lerp(targetValue, initialVal, elapsedTime / duration);
            platformMaterial.SetFloat(propertyName, newValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        platformMaterial.SetFloat(propertyName, initialVal);
    }

    IEnumerator DarkColorPulse()
    {
        Color initialColor = platformMaterial.GetColor(brightPropertyName);

        while (true) // Loop indefinitely
        {
            // Lerp to the target color
            yield return StartCoroutine(LerpColor(brightPropertyName, initialColor, targetBrightColor, brightLerpDuration));

            // Stay at the target color
            yield return new WaitForSeconds(brightStayDuration);

            // Lerp back to the initial color
            yield return StartCoroutine(LerpColor(brightPropertyName, targetBrightColor, initialColor, brightLerpDuration));

            // Stay at the initial color
            yield return new WaitForSeconds(brightStayDuration);
        }
    }
    
    IEnumerator LerpColor(string propertyName, Color startColor, Color endColor, float duration)
    {
        Debug.Log("Color");
        
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            Color newColor = Color.Lerp(startColor, endColor, elapsedTime / duration);
            platformMaterial.SetColor(propertyName, newColor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        platformMaterial.SetColor(propertyName, endColor);
    }
}