using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disbleObject : MonoBehaviour
{
    public GameObject pausedBG;
    public void disableThis()
    {
        gameObject.SetActive(false);
    }
}
