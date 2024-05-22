using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Pixelate")]
public class Pixelate : MonoBehaviour
{
    public Shader shader;
    int _pixelSizeX = 1;
    int _pixelSizeY = 1;
    Material _material;
    [Range(1, 20)]
    public int pixelSizeX = 1;
    [Range(1, 20)]
    public int pixelSizeY = 1;
    public bool lockXY = true;
    private bool isPixelated = false;
    private bool isEscapeActive = false;
    private bool rememberedPixelatedState = false;
    private Coroutine pixelateCoroutine;

    void Start()
    {
        // Start with the adjusted pixelation level
        pixelateCoroutine = StartCoroutine(AdjustPixelation(_pixelSizeX, _pixelSizeY, pixelSizeX, pixelSizeY));
        isPixelated = true;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null) _material = new Material(shader);
        _material.SetInt("_PixelateX", _pixelSizeX);
        _material.SetInt("_PixelateY", _pixelSizeY);
        Graphics.Blit(source, destination, _material);
    }

    void OnDisable()
    {
        DestroyImmediate(_material);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !isEscapeActive)
        {
            if (pixelateCoroutine != null)
            {
                StopCoroutine(pixelateCoroutine);
            }

            if (isPixelated)
            {
                pixelateCoroutine = StartCoroutine(AdjustPixelation(_pixelSizeX, _pixelSizeY, 0, 0));
            }
            else
            {
                pixelateCoroutine = StartCoroutine(AdjustPixelation(_pixelSizeX, _pixelSizeY, pixelSizeX, pixelSizeY));
            }

            isPixelated = !isPixelated;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pixelateCoroutine != null)
            {
                StopCoroutine(pixelateCoroutine);
            }

            if (!isEscapeActive)
            {
                // Remember the current state
                rememberedPixelatedState = isPixelated;

                // Turn off pixelation
                pixelateCoroutine = StartCoroutine(AdjustPixelation(_pixelSizeX, _pixelSizeY, 0, 0));
                isPixelated = false;
            }
            else
            {
                // Revert to the remembered state
                if (rememberedPixelatedState)
                {
                    pixelateCoroutine = StartCoroutine(AdjustPixelation(_pixelSizeX, _pixelSizeY, pixelSizeX, pixelSizeY));
                }
                else
                {
                    pixelateCoroutine = StartCoroutine(AdjustPixelation(_pixelSizeX, _pixelSizeY, 0, 0));
                }
                isPixelated = rememberedPixelatedState;
            }

            isEscapeActive = !isEscapeActive;
        }
    }

    IEnumerator AdjustPixelation(int startX, int startY, int targetX, int targetY)
    {
        float duration = 0.1f; // Adjust the duration for smoother transitions
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _pixelSizeX = Mathf.RoundToInt(Mathf.Lerp(startX, targetX, elapsed / duration));
            _pixelSizeY = Mathf.RoundToInt(Mathf.Lerp(startY, targetY, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        _pixelSizeX = targetX;
        _pixelSizeY = targetY;
    }
}
