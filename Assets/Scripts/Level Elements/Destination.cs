using UnityEngine;
using UnityEngine.SceneManagement;

public class Destination : MonoBehaviour
{
    public string nextScene;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextScene);
            
            Destroy(this.gameObject);
        }
    }
}
