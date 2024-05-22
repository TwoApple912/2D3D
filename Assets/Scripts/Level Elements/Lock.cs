using System;
using System.Collections;
using UnityEngine;

public class Lock : MonoBehaviour
{
    private Renderer renderer;
    private Destination destination;
    private KeyAndLockMaterialPropertyBlock materialScript;
    private ParticleSystem particleSystem;
    
    public bool unlocked = false;
    [Space]
    [SerializeField] private float playerInRangeCheckDistance = 18f;
    [Space]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveDuration = 2f;
    
    private bool hasRun = false;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        destination = GetComponentInParent<Destination>();
        materialScript = GetComponent<KeyAndLockMaterialPropertyBlock>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (unlocked && InRangeCheck() && !hasRun)
        {
            StartCoroutine(UnlockLock());

            hasRun = true;
        }
    }

    IEnumerator UnlockLock()
    {
        yield return new WaitForSeconds(0.5f);
        
        particleSystem.Play();
        
        Material[] mat = new Material[1] {dissolveMaterial};
        renderer.materials = mat;
        
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

    bool InRangeCheck()
    {
        float distance = Vector3.Distance(transform.position, GetActivePlayer().position);
        
        if (distance < playerInRangeCheckDistance) return true;
        return false;
    }

    Transform GetActivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            if (player.activeInHierarchy)
            {
                return player.transform;
            }
        }
        return null;
    }
}