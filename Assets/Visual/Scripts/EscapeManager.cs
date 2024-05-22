using FMODUnity;
using UnityEngine;

public class EscapeManager : MonoBehaviour
{
    public GameObject[] UIElementsForMenu; // Array of GameObjects for the UI elements in the menu
    public string escapeSoundEvent; // Path to the FMOD one-shot event for the escape sound

    private bool areElementsDisabled = true; // Initially set to true, all GameObjects are disabled at the start

    void Start()
    {
        // Disable all GameObjects at the start
        foreach (GameObject obj in UIElementsForMenu)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        // Check for the Escape key press to toggle the active state of all GameObjects
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleActiveState();
            PlayEscapeSound();
        }
    }

    void ToggleActiveState()
    {
        if (areElementsDisabled)
        {
            // Enable all GameObjects
            foreach (GameObject obj in UIElementsForMenu)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            // Disable all GameObjects
            foreach (GameObject obj in UIElementsForMenu)
            {
                obj.SetActive(false);
            }
        }
        areElementsDisabled = !areElementsDisabled;
    }

    void PlayEscapeSound()
    {
        RuntimeManager.PlayOneShot(escapeSoundEvent);
    }
}