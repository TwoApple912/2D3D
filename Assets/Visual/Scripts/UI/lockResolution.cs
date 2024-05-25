using System;
using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    public float targetAspectRatio = 16f / 9f;
    public bool useMainCamera = true;
    private Camera mainCamera;
    public Camera additionalCamera;
    
    void Start()
    {
        mainCamera = Camera.main;

        if (useMainCamera)
        {
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found. Please ensure the main camera is tagged 'MainCamera'.");
                return;
            }
            SetCameraAspect(mainCamera);
        }

        if (additionalCamera == null)
        {
            Debug.LogError("Additional camera not assigned. Please drag and drop the desired camera in the Inspector.");
            return;
        }

        SetCameraAspect(additionalCamera);
    }

    void SetCameraAspect(Camera camera)
    {
        float targetAspect = targetAspectRatio;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }
    }

    void OnPreCull()
    {
        GL.Clear(true, true, Color.black);
    }
}