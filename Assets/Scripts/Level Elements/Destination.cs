using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Destination : MonoBehaviour
{
    private SwitchDimension dimension;
    private SnapDimensionToAxes _snapDimensionToAxes;
    private AllowInput input;
    
    [SerializeField] private bool accessible = true;
    [SerializeField] private bool playerInRange = false;

    [SerializeField] private bool Only2D = false;
    [SerializeField] private bool onCorrectAxes = false;
    [Space]
    [SerializeField] private float requiredHoldTime = 0.75f;
    [Space]
    public string nextScene;
    private enum axes
    {
        positiveX, negativeX,
        positiveZ, negativeZ,
    }
    [SerializeField] private axes faceDirection;
    [Space]
    [SerializeField] private GameObject goalCamera;
    [SerializeField] private GameObject goalReachedParticle;

    private bool hasRun = false; // Use for GoalReached()
    private GameObject player;
    private float holdTime = 0f;

    private void Awake()
    {
        dimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();
        _snapDimensionToAxes = GameObject.Find("2D Dimension").GetComponent<SnapDimensionToAxes>();
        input = GameObject.Find("Game Manager").GetComponent<AllowInput>();
    }

    private void Update()
    {
        CheckCurrentAxes();
        AccessibilityCheck();
    }

    private void LateUpdate()
    {
        GoalReached();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    void GoalReached()
    {
        if (playerInRange && onCorrectAxes && AccessibilityCheck())
        {
            if (Input.GetKey(KeyCode.Q)) holdTime += Time.deltaTime;
            if (Input.GetButton("Switch Dimension")) holdTime = 0f;

            if (holdTime > requiredHoldTime && !hasRun)
            {
                TransitionToNextLevel playerTransitionScript = player.GetComponent<TransitionToNextLevel>();
                if (playerTransitionScript != null)
                {
                    input.allowInput = false;

                    StartCoroutine(playerTransitionScript.BeginTransitionToNextScene(nextScene));
                }
                
                goalCamera.SetActive(true);

                hasRun = true;
            }
        }
        else holdTime = 0f;
    }

    void CheckCurrentAxes()
    {
        if (!Only2D && dimension.currentState == SwitchDimension.GameState.ThreeDimension)
        {
            onCorrectAxes = true;
        }
        else if (dimension.currentState == SwitchDimension.GameState.TwoDimension)
        {
            float angle = 0f;
            switch (faceDirection)
            {
                case axes.negativeZ:
                    angle = 0;
                    break;
                case axes.negativeX:
                    angle = 90;
                    break;
                case axes.positiveZ:
                    angle = 180;
                    break;
                case axes.positiveX:
                    angle = 270;
                    break;
            }

            if (Mathf.RoundToInt(_snapDimensionToAxes.currentCamAngle) == angle) onCorrectAxes = true;
            else onCorrectAxes = false;
        }
    }

    bool AccessibilityCheck()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Lock")
            {
                accessible = false;
                return false;
            }
        }
        
        accessible = true;
        return true;
    }
}