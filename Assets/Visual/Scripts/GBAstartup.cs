using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GBAstartup : MonoBehaviour
{
    public float delayTime;
    private void Start()
    {
        StartCoroutine(loadMenu());
    }

    IEnumerator loadMenu()
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene("Start Menu");
        this.gameObject.SetActive(false);
    }
}
