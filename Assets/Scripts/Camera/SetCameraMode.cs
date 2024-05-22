using Cinemachine;
using UnityEngine;

public class SetCameraMode : MonoBehaviour
{
    private SwitchDimension dimension;
    
    private CinemachineVirtualCamera virtualCamera;

    [Header("Perspective Parameters")]
    [SerializeField] private float perspectiveFOV = 60;
    [SerializeField] private Vector3 perspectiveFollowOffset = new Vector3(-6,1.8f,0);
    
    [Header("Orthographic Parameters")]
    [SerializeField] private float orthoSize = 3.38f;
    [SerializeField] private Vector3 orthoFollowOffset = new Vector3(-50,0.75f,0);
    
    private void Awake()
    {
        dimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        if (dimension.currentState == SwitchDimension.GameState.ThreeDimension)
        {
            var lensSettings = virtualCamera.m_Lens;
            lensSettings.ModeOverride = LensSettings.OverrideModes.Perspective;
            lensSettings.FieldOfView = perspectiveFOV;
            virtualCamera.m_Lens = lensSettings;
            
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = perspectiveFollowOffset;
            
            var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer == null)
            {
                virtualCamera.AddCinemachineComponent<CinemachineComposer>();
            }
        }
        else
        {
            var lensSettings = virtualCamera.m_Lens;
            lensSettings.OrthographicSize = orthoSize;
            lensSettings.ModeOverride = LensSettings.OverrideModes.Orthographic;
            virtualCamera.m_Lens = lensSettings;

            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = orthoFollowOffset;

            var aim = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (aim != null)
            {
                DestroyImmediate(aim);
            }
        }
    }
}
