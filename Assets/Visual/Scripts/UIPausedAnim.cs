using UnityEngine;
using System.Collections.Generic;

public class UIPausedAnim : MonoBehaviour
{
    public List<Animator> animators; // List to hold multiple Animator components

    // Method to set the isPaused bool parameter
    public void SetPauseState(bool state)
    {
        foreach (Animator animator in animators)
        {
            if (animator != null)
            {
                animator.SetBool("isPaused", state);
            }
        }
    }

    // For toggling the pause state
    private bool isPaused = false;

    void Update()
    {
        // Check if the 'Escape' key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused; // Toggle the pause state
            SetPauseState(isPaused);
        }
    }

    // For testing in the Inspector
    [ContextMenu("Pause All Animators")]
    public void TestPause()
    {
        SetPauseState(true);
    }

    [ContextMenu("Unpause All Animators")]
    public void TestUnpause()
    {
        SetPauseState(false);
    }
}