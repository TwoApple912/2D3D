using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator;
    public string animationBool;
    public string hoverFmodEventPath;
    public string ownAnimatorBool; 
    

    public Animator ownAnimator;

    void Start()
    {
        SetChildrenActive(false);
        ownAnimator = GetComponent<Animator>();
    }

    private void SetChildrenActive(bool isActive)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetChildrenActive(true);

        if (ownAnimator != null && !string.IsNullOrEmpty(ownAnimatorBool))
        {
            ownAnimator.SetBool(ownAnimatorBool, true); // Set own Animator bool to true on hover
        }

        if (animator != null && !string.IsNullOrEmpty(animationBool))
        {
            animator.SetBool(animationBool, true);
        }

        if (!string.IsNullOrEmpty(hoverFmodEventPath))
        {
            RuntimeManager.PlayOneShot(hoverFmodEventPath);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetChildrenActive(false); // Disable all children

        if (ownAnimator != null && !string.IsNullOrEmpty(ownAnimatorBool))
        {
            ownAnimator.SetBool(ownAnimatorBool, false); // Set own Animator bool to false when not hovering
        }

        if (animator != null && !string.IsNullOrEmpty(animationBool))
        {
            animator.SetBool(animationBool, false);
        }
    }
}