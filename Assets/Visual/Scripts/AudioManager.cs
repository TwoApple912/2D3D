using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public string eventPath1; // Path to the first FMOD event
    public string eventPath2; // Path to the second FMOD event
    public string snapTo2DEventPath; // Path to the Snap to 2D one-shot event

    private EventInstance eventInstance1;
    private EventInstance eventInstance2;
    private bool isEvent1Muted = true; // Initially set to true to mute the first event at start
    private bool isAllMuted = false; // Initially set to false, not all events are muted

    void Start()
    {
        // Initialize the FMOD events
        eventInstance1 = RuntimeManager.CreateInstance(eventPath1);
        eventInstance2 = RuntimeManager.CreateInstance(eventPath2);

        // Start the events (background music)
        eventInstance1.start();
        eventInstance2.start();

        // Mute the first event initially
        eventInstance1.setVolume(0f);
    }

    void Update()
    {
        // Check for the L key press to toggle mute
        if (Input.GetKeyDown(KeyCode.L) && !isAllMuted)
        {
            ToggleMute();
        }

        // Check for the Escape key press to mute/unmute all events
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMuteAll();
        }
    }

    void ToggleMute()
    {
        if (isEvent1Muted)
        {
            // Unmute event1 and mute event2
            eventInstance1.setVolume(1f); // Adjust volume to the desired level
            eventInstance2.setVolume(0f);
        }
        else
        {
            // Mute event1 and unmute event2
            eventInstance1.setVolume(0f);
            eventInstance2.setVolume(1f); // Adjust volume to the desired level
        }
        isEvent1Muted = !isEvent1Muted;

        // Play the Snap to 2D one-shot sound
        PlaySnapTo2DSound();
    }

    void ToggleMuteAll()
    {
        if (isAllMuted)
        {
            // Unmute the event that was active before muting all
            if (isEvent1Muted)
            {
                eventInstance2.setVolume(1f); // Adjust volume to the desired level
            }
            else
            {
                eventInstance1.setVolume(1f); // Adjust volume to the desired level
            }
        }
        else
        {
            // Mute the currently active event
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
        // Release the event instances when the object is destroyed
        eventInstance1.release();
        eventInstance2.release();
    }
}
