using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    public float targetAspectRatio = 16f / 9f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please ensure the main camera is tagged 'MainCamera'.");
            return;
        }

        float targetAspect = targetAspectRatio;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        mainCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        if (scaleHeight < 1.0f)
        {
            Rect rect = mainCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            mainCamera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = mainCamera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            mainCamera.rect = rect;
        }
    }

    void OnPreCull()
    {
        GL.Clear(true, true, Color.black);
    }
}
