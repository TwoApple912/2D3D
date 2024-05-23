using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour, IPointerEnterHandler
{
    public string clickFmodEventPath; 
    public string hoverFmodEventPath; 
    public GameObject gameObjectToEnable; 

    public bool isQuit = false;

    private bool hasBeenClicked = false; 
    
    public void Clicked()
    {
        if (!hasBeenClicked)
        {
            hasBeenClicked = true;
            TriggerClickFMODEvent();
            EnableGameObject();
            if (isQuit)
            {
                Application.Quit();
            }
        }
    }
    
    private void TriggerClickFMODEvent()
    {
        if (!string.IsNullOrEmpty(clickFmodEventPath))
        {
            RuntimeManager.PlayOneShot(clickFmodEventPath);
        }
    }
    
    private void EnableGameObject()
    {
        if (gameObjectToEnable != null)
        {
            gameObjectToEnable.SetActive(true);
        }
    }
    

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverSound();
    }

    private void PlayHoverSound()
    {
        if (!string.IsNullOrEmpty(hoverFmodEventPath))
        {
            RuntimeManager.PlayOneShot(hoverFmodEventPath); ;
        }
    }

    public void restart()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
}
