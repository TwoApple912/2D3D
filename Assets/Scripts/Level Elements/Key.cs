using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Key : MonoBehaviour
{
    private Renderer renderer;
    private ParticleSystem particleSystem;
    private KeyAndLockMaterialPropertyBlock materialScript;
    
    private Destination destination;
    private FloatingObject floatingScript; // Checkpoint

    [SerializeField] Lock associatedLock;
    [Space]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveDuration = 1f;

    private bool hasRun = false;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        materialScript = GetComponent<KeyAndLockMaterialPropertyBlock>();
        
        destination = GameObject.Find("Goal").GetComponent<Destination>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !hasRun)
        {
            associatedLock.unlocked = true;
            
            StartCoroutine(KeyObtainedVisual());

            hasRun = true;
        }
    }

    IEnumerator KeyObtainedVisual()
    {
        
        
        particleSystem.Play();

        renderer.material = dissolveMaterial;
        
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_Color", materialScript.baseColor);
        renderer.SetPropertyBlock(propertyBlock);

        float elapsed = 0;
        while (elapsed < dissolveDuration)
        {
            float cutOffValue = Mathf.Lerp(0, 1, elapsed / dissolveDuration);
            
            renderer.material.SetFloat("_Cutoff", cutOffValue);
            renderer.material.SetColor("_EdgeColor", materialScript.outlineColor);

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
}