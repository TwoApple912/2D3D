using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TransitionToNextLevel : MonoBehaviour
{
    private MeshRenderer[] renderers;
    private MeshRenderer[] eyeRenderers;

    private RotateModel rotateScript;
    
    [SerializeField] private Material dissolveMaterial;

    [Header("Parameters")]
    [SerializeField] private float initialDelayDuration = 2f;
    [SerializeField] private float dissolveDuration = 3f;
    [SerializeField] private float finalDelayDuration = 1f;

    [Header("Particles")]
    [SerializeField] private GameObject goalReachedParticle;
        
    private void Awake()
    {
        renderers = new[]
        {
            transform.Find("Model/Leg").GetComponent<MeshRenderer>(),
            transform.Find("Model/Head").GetComponent<MeshRenderer>()
        };
        eyeRenderers = new[]
        {
            transform.Find("Model/LEye").GetComponent<MeshRenderer>(),
            transform.Find("Model/REye").GetComponent<MeshRenderer>()
        };

        rotateScript = GetComponentInChildren<RotateModel>();
        
        if (dissolveMaterial == null) Debug.LogError("Attach Dissolve material to TransitionToNextLevel.cs");
    }

    public IEnumerator BeginTransitionToNextScene(string nextScene)
    {
        rotateScript.transitioningToNextLevel = true;
        
        /* Initial zoom in phase */
        float outlineWidthvalue = 0.1f; float elapsed = 0;
        while (elapsed < initialDelayDuration)
        {
            outlineWidthvalue = Mathf.Lerp(0.1f, 0, elapsed / initialDelayDuration);
            
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] mats = renderers[i].materials;
                mats[0].SetFloat("_OutlineWidth", outlineWidthvalue);
                renderers[i].materials = mats;
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < renderers.Length; i++) // Replace material in the end
        {
            Material[] mats = renderers[i].materials;
            mats[0] = dissolveMaterial;
            renderers[i].materials = mats;
        }
        
        goalReachedParticle.transform.position = transform.position;
        goalReachedParticle.SetActive(true);
        
        /* Offset noise texture */
        for (int i = 0; i < renderers.Length; i++)
        {
            Vector2 bodyOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            renderers[i].material.SetTextureOffset("_NoiseTex", bodyOffset);
        }
        
        Vector2 eyeOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        Vector2 eyeTiling = new Vector2
        (
            (Random.Range(0f, 1f) < 0.5f) ? Random.Range(0f, 1f) : Random.Range(1f, 12f),
            (Random.Range(0f, 1f) < 0.5f) ? Random.Range(0f, 1f) : Random.Range(1f, 12f)
        );
        for (int i = 0; i < eyeRenderers.Length; i++)
        {
            eyeRenderers[i].material.SetTextureOffset("_NoiseTex", eyeOffset);
            eyeRenderers[i].material.SetTextureScale("_NoiseTex", eyeTiling);
        }
        
        /* Player dissolve phase */
        MeshRenderer[] newRenderers = new MeshRenderer[renderers.Length + eyeRenderers.Length];
        Array.Copy(renderers, newRenderers, renderers.Length);
        for (int i = 0; i < eyeRenderers.Length; i++) newRenderers[renderers.Length + i] = eyeRenderers[i]; // Add eyeRenderer
        
        float cutOffValue = 0; elapsed = 0;
        while (elapsed < dissolveDuration)
        {
            cutOffValue = Mathf.Lerp(0, 1, elapsed / dissolveDuration);

            for (int i = 0; i < newRenderers.Length; i++)
            {
                Material[] mats = newRenderers[i].materials;
                
                mats[0].SetFloat("_Cutoff", cutOffValue);

                newRenderers[i].materials = new Material[] { mats[0] };
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < newRenderers.Length; i++) // Make sure it lands on 1
        {
            Material[] mats = newRenderers[i].materials;
                
            mats[0].SetFloat("_Cutoff", 1);
            
            newRenderers[i].materials = mats;
        }

        yield return new WaitForSeconds(finalDelayDuration);

        SceneManager.LoadScene(nextScene);
    }
}