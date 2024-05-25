using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeInOnEnabled : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    [SerializeField] private float fadeInDuration = 1f;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetAlpha(0);
        StartCoroutine(FadeInText());
    }

    private void SetAlpha(float alpha)
    {
        if (textMeshPro != null)
        {
            Color color = textMeshPro.color;
            color.a = alpha;
            textMeshPro.color = color;
        }
    }

    private IEnumerator FadeInText()
    {
        float elapsed = 0.0f;
        Color originalColor = textMeshPro.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1);

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textMeshPro.color = targetColor; // Ensure the alpha is set to 1 at the end
    }
}