using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public string eventPath1; 
    public string eventPath2; 
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
    }
    void Start()
    {

        eventInstance1 = RuntimeManager.CreateInstance(eventPath1);
        eventInstance2 = RuntimeManager.CreateInstance(eventPath2);

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
        eventInstance1.release();
        eventInstance2.release();
    }
}
