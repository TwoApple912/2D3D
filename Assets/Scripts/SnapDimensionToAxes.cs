using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SnapDimensionToAxes : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCam;
    
    [Header("Script variables")]
    public int numberOfAxes = 4;
    [Space]
    public float currentCamAngle;
    public float snappedCamAngle;
    [Space]
    [SerializeField] private bool isControlledByOrbitalVirtualCam = false;

    [Header("Others")]
    [SerializeField] private Transform player2D;
    [SerializeField] private Transform player3D;

    void Start()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
    }
    
    private void Update()
    {
        currentCamAngle = NormalizeAngle(transform.eulerAngles.y);
    }

    public void SnapRotation()
    {
        if (!isControlledByOrbitalVirtualCam) SnapNonVirtualCam();
        else SnapOrbitalVirtualCam();

        MaintainPlayer2DPosition();
    }

    void SnapNonVirtualCam()
    {
        float currentYRotation = transform.eulerAngles.y;
        float step = 360 / numberOfAxes;

        float targetYRotation = Mathf.Round(currentYRotation / step) * step;
        targetYRotation = NormalizeAngle(targetYRotation);
        
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);

        snappedCamAngle = targetYRotation;
    }

    void SnapOrbitalVirtualCam()
    {
        CinemachineOrbitalTransposer orbitalTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>();
        
        if (orbitalTransposer != null)
        {
            float currentHeading = orbitalTransposer.m_XAxis.Value;
            float step = 360 / numberOfAxes;
            float targetHeading = Mathf.Round(currentHeading / step) * step;
            targetHeading = NormalizeAngle(targetHeading);

            orbitalTransposer.m_XAxis.Value = targetHeading;
        }
    }
    
    private float NormalizeAngle(float angle)
    {
        if (angle >= 360) angle -= 360;
        else if (angle < 0) angle += 360;
        
        return angle;
    }

    void MaintainPlayer2DPosition()
    {
        // Ensure the Player2D position is set correctly after dimension snap.
        player2D.position = player3D.position;
    }
}