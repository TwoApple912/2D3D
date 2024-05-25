using System;
using System.Collections;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CollectableScript : MonoBehaviour
{
    [SerializeField] private string collectibleName = "SecretItem";
    private ParticleSystem collectibleParticle;
    private FloatingObject floatingScript;
    [Space]
    [SerializeField] private float secondUntilDestroyed = 3f;
    [Space]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveDuration = 2f;

    private Vector3 offset;

    private void Awake()
    {
        collectibleParticle = GameObject.Find("Particle Manager").transform.Find("CollectibleParticle").GetComponent<ParticleSystem>();
        floatingScript = GetComponent<FloatingObject>();
    }

    private void Start()
    {
        offset = collectibleParticle.transform.position - transform.position;
    }

    private void Update()
    {
        collectibleParticle.transform.position = transform.position + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PlayerPrefs.GetInt(collectibleName, 0) == 0)
            {
                int currentCount = PlayerPrefs.GetInt("SecretItemCount", 0);
                currentCount++;
                PlayerPrefs.SetInt("SecretItemCount", currentCount);
                PlayerPrefs.SetInt(collectibleName, 1);

                PlayerPrefs.Save();
            }

            StartCoroutine(ObtainedVisual());
        }
    }
    
    IEnumerator ObtainedVisual()
    {
        if (floatingScript) floatingScript.enabled = false;
        collectibleParticle.Stop();
        collectibleParticle.transform.Find("Pulse").gameObject.SetActive(true);

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Debug.Log(renderers.Length);
        MaterialPropertyBlock[] propertyBlocks = new MaterialPropertyBlock[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = dissolveMaterial;
            propertyBlocks[i] = new MaterialPropertyBlock();
        }

        float elapsed = 0;
        Color startColor = collectibleParticle.main.startColor.color;

        while (elapsed < dissolveDuration)
        {
            float cutOffValue = Mathf.Lerp(0, 1, elapsed / dissolveDuration);

            for (int i = 0; i < renderers.Length; i++)
            {
                propertyBlocks[i].SetFloat("_Cutoff", cutOffValue);
                propertyBlocks[i].SetColor("_Color", startColor);
                renderers[i].SetPropertyBlock(propertyBlocks[i]);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final state is set for all renderers
        for (int i = 0; i < renderers.Length; i++)
        {
            propertyBlocks[i].SetFloat("_Cutoff", 1.0f);
            renderers[i].SetPropertyBlock(propertyBlocks[i]);
        }

        Destroy(gameObject);
    }
}
