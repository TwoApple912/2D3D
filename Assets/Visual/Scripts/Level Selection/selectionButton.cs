using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class selectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;
    private string isHoverBool = "isHover";

    void Start()
    {
        // Automatically get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
        }
    }

    // Method called when the mouse pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator != null && !string.IsNullOrEmpty(isHoverBool))
        {
            animator.SetBool(isHoverBool, true);
        }
    }

    // Method called when the mouse pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator != null && !string.IsNullOrEmpty(isHoverBool))
        {
            animator.SetBool(isHoverBool, false);
        }
    }
}