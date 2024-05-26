using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public string startFirst;
    public string startSecond;
    public string snapTo2DEventPath;

    private EventInstance eventInstance1;
    private EventInstance eventInstance2;
    private bool isEvent1Muted = true;
    private bool isAllMuted = false;
    public SnappableCheck snappableCheck;

    void Awake()
    {
        GameObject player3D = GameObject.Find("Player3D");
        if (player3D != null)
        {
            snappableCheck = player3D.GetComponent<SnappableCheck>();
        }

        // Subscribe to scene load event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        eventInstance1 = RuntimeManager.CreateInstance(startSecond);
        eventInstance2 = RuntimeManager.CreateInstance(startFirst);

        eventInstance1.start();
        eventInstance2.start();

        eventInstance1.setVolume(0f);
    }

    void Update()
    {
        if (Input.GetButtonDown("Switch Dimension") && !isAllMuted && snappableCheck != null && snappableCheck.allowSnap)
        {
            ToggleMute();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMuteAll();
        }
    }

    void ToggleMute()
    {
        if (isEvent1Muted)
        {
            eventInstance1.setVolume(1f);
            eventInstance2.setVolume(0f);
        }
        else
        {
            eventInstance1.setVolume(0f);
            eventInstance2.setVolume(1f);
        }
        isEvent1Muted = !isEvent1Muted;

        PlaySnapTo2DSound();
    }

    void ToggleMuteAll()
    {
        if (isAllMuted)
        {
            if (isEvent1Muted)
            {
                eventInstance2.setVolume(1f);
            }
            else
            {
                eventInstance1.setVolume(1f);
            }
        }
        else
        {
            eventInstance1.setVolume(0f);
            eventInstance2.setVolume(0f);
        }
        isAllMuted = !isAllMuted;
    }

    public void PlaySnapTo2DSound()
    {
        RuntimeManager.PlayOneShot(snapTo2DEventPath);
    }

    void OnDestroy()
    {
        // Stop and release FMOD events
        eventInstance1.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance1.release();
        eventInstance2.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance2.release();

        // Unsubscribe from scene load event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop and release FMOD events when a new scene is loaded
        eventInstance1.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance1.release();
        eventInstance2.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance2.release();
    }
}
