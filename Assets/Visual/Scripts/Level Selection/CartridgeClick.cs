using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class CartridgeClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private string animationBool = "inserted";
    private string isHoverBool = "isHover";
    private string fmodEventPath = "event:/Insert Cartridge";
    public string sceneToLoad; // This will be set to the specific level
    public string transitionSceneName = "Transition LS"; // Name of the transition scene

    private Animator animator;
    private bool fmodEventPlayed = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
        }

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    public void PlayFmodEvent()
    {
        if (!string.IsNullOrEmpty(fmodEventPath) && !fmodEventPlayed)
        {
            RuntimeManager.PlayOneShot(fmodEventPath);
            fmodEventPlayed = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (animator != null && !string.IsNullOrEmpty(animationBool))
        {
            animator.SetBool(animationBool, true);
        }
        PlayFmodEvent();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator != null && !string.IsNullOrEmpty(isHoverBool))
        {
            animator.SetBool(isHoverBool, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator != null && !string.IsNullOrEmpty(isHoverBool))
        {
            animator.SetBool(isHoverBool, false);
        }
    }

    private void LoadTransitionScene()
    {
        SceneData.NextScene = sceneToLoad;
        SceneManager.LoadScene(transitionSceneName);
    }
}
