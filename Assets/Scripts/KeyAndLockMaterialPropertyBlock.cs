using System;
using Unity.VisualScripting;
using UnityEngine;

public class KeyAndLockMaterialPropertyBlock : MonoBehaviour
{
    private Renderer renderer;
    private ParticleSystem particleSystem;
    
    [SerializeField] private bool affectOutlineMaterial;
    [SerializeField] private string outlineMaterialName = "Outline";
    public Color baseColor = new Color(1, 0.6f, 0.12f, 1);
    public Color outlineColor = Color.white;
    [SerializeField] private float outlineWidth = 0.12f;
    
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        Material[] materials = renderer.materials;
        MaterialPropertyBlock[] propertyBlock = new MaterialPropertyBlock[materials.Length];
        
        for (int i = 0; i < materials.Length; i++)
        {
            propertyBlock[i] = new MaterialPropertyBlock();
            
            if (affectOutlineMaterial && materials[i].name.Contains(outlineMaterialName))
            {
                renderer.GetPropertyBlock(propertyBlock[i], i);
                propertyBlock[i].SetColor("_Color", baseColor);
                propertyBlock[i].SetColor("_OutlineColor", outlineColor);
                propertyBlock[i].SetFloat("_OutlineWidth", outlineWidth);
            }
        }

        for (int i = 0; i < materials.Length; i++)
        {
            renderer.SetPropertyBlock(propertyBlock[i], i);
        }

        ParticleSystem.ColorOverLifetimeModule col = particleSystem.colorOverLifetime;
        if (col.enabled)
        {
            Gradient gradient = new Gradient();
            col.color = new ParticleSystem.MinMaxGradient(gradient);

            Gradient newGradient = col.color.gradient;

            GradientColorKey[] colorKeys = newGradient.colorKeys;
            colorKeys[0].color = baseColor;
            newGradient.colorKeys = colorKeys;

            col.color = new ParticleSystem.MinMaxGradient(newGradient);
        }
    }
}