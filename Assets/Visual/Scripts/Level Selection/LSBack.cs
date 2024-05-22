using UnityEngine;

public class ParentButtonHover : MonoBehaviour
{
    private string isLSBack = "LSBack";
    public bool isBack = true;
    public bool clicked = false;

    void Start()
    {
        SetAnimatorBool(isBack);
    }
    public void selectButtonClicked()
    {
        isBack = !isBack;
        SetAnimatorBool(isBack);
        clicked = true;
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape) && clicked)
        {
            clicked = false;
            isBack = !isBack;
            SetAnimatorBool(isBack);
        }
    }

    
    private void SetAnimatorBool(bool value)
    {
        Animator[] childAnimators = GetComponentsInChildren<Animator>();
        foreach (Animator animator in childAnimators)
        {
            if (animator != null && !string.IsNullOrEmpty(isLSBack))
            {
                animator.SetBool(isLSBack, value);
            }
        }
    }
}