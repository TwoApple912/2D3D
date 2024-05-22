using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionScene : MonoBehaviour
{
    
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneData.NextScene);
    }

}