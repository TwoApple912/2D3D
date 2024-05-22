using UnityEngine;

public class Destination : MonoBehaviour
{
    private SwitchDimension _switchDimension;
    private SnapDimensionToAxes _snapDimensionToAxes;
    
    [SerializeField] private bool accessible = true;
    [SerializeField] private bool playerInRange = false;
    [SerializeField] private bool onCorrectAxes = false;
    [Space]
    public string nextScene;
    private enum axes
    {
        positiveX, negativeX,
        positiveZ, negativeZ,
    }
    [SerializeField] private axes faceDirection;
    [Space]
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject goalCamera;
    [SerializeField] private GameObject goalReachedParticle;

    private bool hasRun = false; // Use for GoalReached()
    private GameObject player;

    private void Start()
    {
        _switchDimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();
        _snapDimensionToAxes = GameObject.Find("2D Dimension").GetComponent<SnapDimensionToAxes>();
    }

    private void Update()
    {
        CheckCurrentAxes();
        AccessibilityCheck();
        
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
            if (Input.GetAxis("Vertical") > 0.1f && !hasRun)
            {
                TransitionToNextLevel playerTransitionScript = player.GetComponent<TransitionToNextLevel>();
                if (playerTransitionScript != null)
                {
                    StartCoroutine(playerTransitionScript.BeginTransitionToNextScene(nextScene));
                }
                
                goalCamera.SetActive(true);

                hasRun = true;
            }
        }
    }

    void CheckCurrentAxes()
    {
        if (_switchDimension.currentState == SwitchDimension.GameState.TwoDimension)
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
        else onCorrectAxes = false;
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