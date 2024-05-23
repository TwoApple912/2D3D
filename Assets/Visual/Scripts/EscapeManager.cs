using FMODUnity;
using UnityEngine;

public class EscapeManager : MonoBehaviour
{
    public GameObject[] UIElementsForMenu; // Array of GameObjects for the UI elements in the menu
    public string escapeSoundEvent; // Path to the FMOD one-shot event for the escape sound
    public GameObject ESCcam; // The camera GameObject with an Animator component
    private Animator escAnimator; // Animator component of ESCcam
    private bool ESCPressed = true;
    private bool areElementsDisabled = true; // Initially set to true, all GameObjects are disabled at the start

    void Start()
    {
        // Get the Animator component from ESCcam
        escAnimator = ESCcam.GetComponent<Animator>();
        if (escAnimator == null)
        {
            Debug.LogError("Animator component not found on ESCcam.");
        }

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
            ToggleESC();
        }
    }

    void ToggleActiveState()
    {
        if (areElementsDisabled)
        {
            foreach (GameObject obj in UIElementsForMenu)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject obj in UIElementsForMenu)
            {
                obj.SetActive(false);
            }
        }
        areElementsDisabled = !areElementsDisabled;
    }

    void ToggleESC()
    {
        if (ESCPressed)
        {
            ESCcam.SetActive(true);
            if (escAnimator != null)
            {
                escAnimator.SetBool("isPaused", true);
            }
        }
        else
        {
            if (escAnimator != null)
            {
                escAnimator.SetBool("isPaused", false);
            }
        }

        ESCPressed = !ESCPressed;
    }

    void PlayEscapeSound()
    {
        RuntimeManager.PlayOneShot(escapeSoundEvent);
    }
}
