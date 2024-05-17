using System;
using System.Diagnostics;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
    [Space]
    [SerializeField] private float heightSpeed = 5f;
    [SerializeField] private float minHeight = 3f;
    [SerializeField] private float maxHeight = 15f;
    private float targetHeight;

    void Start()
    {
        targetHeight = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineOrbitalTransposer>().m_FollowOffset.y;
    }

    void Update()
    {
        VerticleMovement();
    }

    void VerticleMovement()
    {
        float currentHeight = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineOrbitalTransposer>().m_FollowOffset.y;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            targetHeight = Mathf.Min(targetHeight + heightSpeed * Time.deltaTime, maxHeight);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            targetHeight = Mathf.Max(targetHeight - heightSpeed * Time.deltaTime, minHeight);
        }

        virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineOrbitalTransposer>().m_FollowOffset.y =
            Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * heightSpeed);
        
        virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineOrbitalTransposer>().m_FollowOffset.y = 
            Mathf.Clamp(virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineOrbitalTransposer>().m_FollowOffset.y, minHeight, maxHeight);
    }
}