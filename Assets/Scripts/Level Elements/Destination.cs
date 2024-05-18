using UnityEngine;

public class Destination : MonoBehaviour
{
    private SwitchDimension _switchDimension;
    private SnapDimensionToAxes _snapDimensionToAxes;
    
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

    private void Start()
    {
        _switchDimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();
        _snapDimensionToAxes = GameObject.Find("2D Dimension").GetComponent<SnapDimensionToAxes>();
    }

    private void Update()
    {
        CheckCurrentAxes();
        
        GoalReached();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void GoalReached()
    {
        if (playerInRange && onCorrectAxes)
        {
            // TODO: Finish level.
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

            if (_snapDimensionToAxes.currentCamAngle == angle) onCorrectAxes = true;
            else onCorrectAxes = false;
        }
        else onCorrectAxes = false;
    }
}