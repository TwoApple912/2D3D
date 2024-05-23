using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    public string sceneToLoad;
    public bool isClicked = false;
    
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            isClicked = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}