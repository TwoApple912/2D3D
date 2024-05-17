using System.Collections.Generic;
using UnityEngine;

public class Capture2D : MonoBehaviour
{
    public Camera renderCamera;
    public RenderTexture renderTexture;
    public GameObject environment;

    void Start()
    {
        //ClearRenderTexture(); // Optional
    }

    public void UpdateTexture()
    {
        CaptureScene();
        Texture2D capturedTexture = ConvertToTexture2D(renderTexture);
        Texture2D edgeTexture = ApplySobel(capturedTexture, 0.1f);

        List<Vector2> colliderPoints = ExtractEdgePoints(edgeTexture);
        UpdateCollider(colliderPoints);
    }

    #region Capture 2D
    void CaptureScene()
    {
        renderCamera.targetTexture = renderTexture;
        renderCamera.Render();
    }

    Texture2D ConvertToTexture2D(RenderTexture rTexture)
    {
        Texture2D texture = new Texture2D(rTexture.width, rTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rTexture;
        texture.ReadPixels(new Rect(0,0, rTexture.width, rTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        return texture;
    }

    Texture2D ApplySobel(Texture2D input, float threshold)
    {
        int width = input.width;
        int height = input.height;
        Texture2D output = new Texture2D(width, height);

        float[,] sobelX = {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

        float[,] sobelY = {
            { -1, -2, -1 },
            { 0, 0, 0 },
            { 1, 2, 1 }
        };

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                float gx = 0;
                float gy = 0;

                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        Color pixel = input.GetPixel(x + kx, y + ky);
                        gx += pixel.a * sobelX[kx + 1, ky + 1];
                        gy += pixel.a * sobelY[kx + 1, ky + 1];
                    }
                }

                float edgeStrength = Mathf.Sqrt(gx * gx + gy * gy);
                Color edgeColor = edgeStrength > threshold ? Color.white : Color.black;
                output.SetPixel(x, y, edgeColor);
            }
        }
        
        output.Apply();
        return output;
    }
    
    #endregion

    #region Set Collider

    List<Vector2> ExtractEdgePoints(Texture2D edgeTexture)
    {
        List<Vector2> points = new List<Vector2>();
        for (int y = 0; y < edgeTexture.height; y++)
        {
            for (int x = 0; x < edgeTexture.width; x++)
            {
                Color pixel = edgeTexture.GetPixel(x, y);
                if (pixel == Color.white) // Assuming edge pixels are white
                {
                    points.Add(new Vector2(x, y));
                }
            }
        }

        return points;
    }

    void UpdateCollider(List<Vector2> points)
    {
        PolygonCollider2D polyCollider = environment.GetComponent<PolygonCollider2D>();
        if (polyCollider == null) polyCollider = environment.AddComponent<PolygonCollider2D>();
        
        polyCollider.SetPath(0, points.ToArray());
    }

    #endregion

    void ClearRenderTexture()
    {
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }
}
