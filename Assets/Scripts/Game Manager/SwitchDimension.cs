using System.Collections;
using Cinemachine;
using UnityEngine;

public class SwitchDimension : MonoBehaviour
{
    private SyncPlayerLocation _syncPlayerLocation;
    private AllowInput input;
    
    private SnapDimensionToAxes _snapDimensionToAxes;
    private SnappableCheck _snappableCheck;
    
    public enum GameState
    {
        ThreeDimension,
        TwoDimension
    }

    public GameState currentState = GameState.ThreeDimension;
    
    [Header("State Setting")]
    [SerializeField] private GameObject character3D;
    [SerializeField] private GameObject character2D;
    [Space]
    [SerializeField] private CinemachineVirtualCamera virtualCam3D;
    [SerializeField] private CinemachineVirtualCamera virtualCam2D;

    [Header("Walls")]
    [Tooltip("Horizontal walls are walls that form a 90 degree with the horizontal axis.")]
    [SerializeField] private GameObject[] horizontalWalls;
    [Tooltip("Vertical walls are walls that form a 90 degree with the vertical axis.")]
    [SerializeField] private GameObject[] verticalWalls;
    
    private bool hasRun = false;

    void Start()
    {
        _syncPlayerLocation = GetComponent<SyncPlayerLocation>();
        input = GetComponent<AllowInput>();
        
        _snapDimensionToAxes = GameObject.Find("2D Dimension").GetComponent<SnapDimensionToAxes>();
        _snappableCheck = GameObject.Find("Player3D").GetComponent<SnappableCheck>();
        
        SwitchState(currentState);
    }

    void LateUpdate()
    {
        if (Input.GetButtonDown("Switch Dimension") && _snappableCheck.allowSnap && input.allowInput)
        {
            SwitchState(currentState == GameState.ThreeDimension ? GameState.TwoDimension : GameState.ThreeDimension);
            _snapDimensionToAxes.SnapRotation();
        }

        Disable2DimensionMovement();
        Disable2DWallCollision();
    }

    public void SwitchState(GameState newState)
    {
        OnStateChange();

        currentState = newState;
        switch (currentState)
        {
            case GameState.ThreeDimension:
                ThreeDimension();
                break;
            case GameState.TwoDimension:
                TwoDimension();
                break;
        }
    }

    void OnStateChange()
    {
        
    }

    #region 3D
    
    void ThreeDimension()
    {
        if (hasRun) _syncPlayerLocation.SwitchTo3D();
        else hasRun = true;
        
        virtualCam2D.gameObject.SetActive(false);
        character2D.SetActive(false);
        
        virtualCam3D.gameObject.GetComponent<TopDownCamera>().enabled = true;
        character3D.SetActive(true);
    }

    void TwoDimension()
    {
        if (hasRun) _syncPlayerLocation.SwitchTo2D();
        else hasRun = true;
        
        virtualCam3D.gameObject.GetComponent<TopDownCamera>().enabled = false; // Disable camera movement
        character3D.SetActive(false);
        
        virtualCam2D.gameObject.SetActive(true);
        character2D.SetActive(true);
        
        // Disable overlapping object with the player
        //character2D.GetComponent<Character2DSpecificFunctions>().DisableOverlappingColliders();
    }

    #endregion
    
    #region 2D
    
    void Disable2DimensionMovement()
    {
        if (currentState == GameState.TwoDimension) GameObject.Find("2D Dimension").GetComponent<FakeChildTransform>().enabled = false;
        else GameObject.Find("2D Dimension").GetComponent<FakeChildTransform>().enabled = true;
    }

    void Disable2DWallCollision()
    {
        if (currentState == GameState.TwoDimension)
        {
            if (_snapDimensionToAxes.snappedCamAngle == 0 || _snapDimensionToAxes.snappedCamAngle == 180)
            {
                for (int i = 0; i < horizontalWalls.Length; i++)
                {
                    horizontalWalls[i].GetComponent<Collider>().enabled = false;
                }
            }
            else if (_snapDimensionToAxes.snappedCamAngle == 90 || _snapDimensionToAxes.snappedCamAngle == 270)
            {
                for (int i = 0; i < verticalWalls.Length; i++)
                {
                    verticalWalls[i].GetComponent<Collider>().enabled = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < horizontalWalls.Length; i++)
            {
                horizontalWalls[i].GetComponent<Collider>().enabled = true;
            }
            for (int i = 0; i < verticalWalls.Length; i++)
            {
                verticalWalls[i].GetComponent<Collider>().enabled = true;
            }
        }
    }
    
    #endregion
}