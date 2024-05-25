using System;
using UnityEngine;

public class MaterialColorCycle : MonoBehaviour
{
    private SwitchDimension dimension;
    
    [SerializeField] private Material normalMaterial;
    [SerializeField] private float cycleTime = 5;
    [SerializeField] private float vertexAnimationIntensity3D = 0.01f;
    [SerializeField] private float vertexAnimationIntensity2D = 0f;

    private float currentHue = 0;

    private void Awake()
    {
        dimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();
    }

    void Update()
    {
        NormalMaterial();
    }

    void NormalMaterial()
    {
        currentHue += Time.deltaTime / cycleTime;
        if (currentHue > 1f)
        {
            currentHue -= 1f;
        }
        Color newColor = Color.HSVToRGB(currentHue, 1f, 1f, true);
        normalMaterial.SetColor("_GoochBrightColor", newColor);

        if (dimension.currentState == SwitchDimension.GameState.ThreeDimension)
        {
            normalMaterial.SetFloat("_VertexAnimationIntensity", vertexAnimationIntensity3D);
        }
        else normalMaterial.SetFloat("_VertexAnimationIntensity", vertexAnimationIntensity2D);
    }
}
