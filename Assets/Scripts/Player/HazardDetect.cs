using System;
using System.Collections;
using System.Security.Cryptography;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class HazardDetect : MonoBehaviour
{
    private CinemachineVirtualCamera camera2D;
    private CinemachineVirtualCamera camera3D;
    [SerializeField] private GameObject deathParticle;
    private GameObject restartUIText;

    private AllowInput input;
    [Space]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float transitionDuration = 0.12f;
    [SerializeField] private float deathParticleDelay = 0.1f;
    [SerializeField] private float restartUITextDelay = 2f;

    [Header("Shake Parameters")]
    [SerializeField] private float shakeMagnitude = 0.1f;
    
    [Header("2D Camera Death Parameters")]
    [SerializeField] private float new2DOrthoSize = 3.7f;
    [SerializeField] private float new2DFarClipPlane = 66f;
    [SerializeField] private Vector3 new2DFollowOffset = new Vector3(0, 2, -37);
    
    [Header("3D Camera Death Parameters")]
    [SerializeField] private float new3DFOV = 37f;

    private bool hasRun = false;
    private bool died = false;
    
    private void Awake()
    {
        camera2D = GameObject.Find("2D Dimension").transform.Find("2D Camera").GetComponent<CinemachineVirtualCamera>();
        camera3D = GameObject.Find("3D Camera").GetComponent<CinemachineVirtualCamera>();
        restartUIText = GameObject.Find("Canvas").transform.Find("Restart").gameObject;
        input = GameObject.Find("Game Manager").GetComponent<AllowInput>();
        
        Debug.Log("Restart");
    }

    private void Update()
    {
        if (died) PressKeyToReload();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Hazard") && !hasRun)
        {
            Death();

            hasRun = true;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Hazard") && !hasRun)
        {
            Death();

            hasRun = true;
        }
    }

    private void Death()
    {
        input.allowInput = false;
        
        StartCoroutine(Shake());
        
        CinemachineVirtualCamera currentCamera = currentCMCam();
        if (currentCamera.m_Lens.Orthographic)
        {
            StartCoroutine(LerpCameraOrthographicSize(currentCamera, new2DOrthoSize, transitionDuration));
            StartCoroutine(LerpCameraFarClipPlane(currentCamera, new2DFarClipPlane, transitionDuration));
            var transposer = currentCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
                StartCoroutine(LerpTransposerFollowOffset(transposer, new2DFollowOffset, transitionDuration));
        }
        else
        {
            StartCoroutine(LerpCameraFieldOfView(currentCamera, new3DFOV, transitionDuration));
        }
    }

    #region Camera Coroutines
    
    private IEnumerator LerpCameraOrthographicSize(CinemachineVirtualCamera cam, float targetSize, float duration)
    {
        yield return new WaitForSeconds(shakeDuration + 0.18f);
        
        float time = 0;
        float startSize = cam.m_Lens.OrthographicSize;
        while (time < duration)
        {
            cam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cam.m_Lens.OrthographicSize = targetSize; // Ensure target size at the end
    }

    private IEnumerator LerpCameraFieldOfView(CinemachineVirtualCamera cam, float targetFOV, float duration)
    {
        yield return new WaitForSeconds(shakeDuration + 0.18f);
        
        float time = 0;
        float startFOV = cam.m_Lens.FieldOfView;
        while (time < duration)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cam.m_Lens.FieldOfView = targetFOV; // Ensure target FOV at the end
        
        yield return new WaitForSeconds(deathParticleDelay);
        
        deathParticle.transform.position = transform.position;
        deathParticle.SetActive(true);
        restartUIText.SetActive(true);
        transform.Find("Model").gameObject.SetActive(false);
        Destroy(GameObject.Find("Head")); Destroy(GameObject.Find("Head"));

        yield return new WaitForSeconds(restartUITextDelay);
        died = true;
    }

    private IEnumerator LerpCameraFarClipPlane(CinemachineVirtualCamera cam, float targetClipPlane, float duration)
    {
        float time = 0;
        float startClipPlane = cam.m_Lens.FarClipPlane;
        while (time < duration)
        {
            cam.m_Lens.FarClipPlane = Mathf.Lerp(startClipPlane, targetClipPlane, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cam.m_Lens.FarClipPlane = targetClipPlane; // Ensure target value at the end
    }

    private IEnumerator LerpTransposerFollowOffset(CinemachineTransposer transposer, Vector3 targetOffset, float duration)
    {
        yield return new WaitForSeconds(shakeDuration + 0.18f);
        
        float time = 0;
        Vector3 startOffset = transposer.m_FollowOffset;
        while (time < duration)
        {
            transposer.m_FollowOffset = Vector3.Lerp(startOffset, targetOffset, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transposer.m_FollowOffset = targetOffset; // Ensure target value at the end

        yield return new WaitForSeconds(deathParticleDelay);
        
        deathParticle.transform.position = transform.position;
        deathParticle.SetActive(true);
        restartUIText.SetActive(true);
        transform.Find("Model").gameObject.SetActive(false);
        Destroy(GameObject.Find("Head")); Destroy(GameObject.Find("Head"));
        
        yield return new WaitForSeconds(restartUITextDelay);
        died = true;
    }

    CinemachineVirtualCamera currentCMCam()
    {
        if (camera2D.gameObject.activeInHierarchy) return camera2D;
        else return camera3D;
    }
    
    #endregion
    
    private IEnumerator Shake()
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float remainingTime = shakeDuration - elapsed;
            float currentMagnitude = shakeMagnitude * (remainingTime / shakeDuration);
            
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    void PressKeyToReload()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
