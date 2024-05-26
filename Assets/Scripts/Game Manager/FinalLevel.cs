using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalLevel : MonoBehaviour
{
    private Destination destination;

    private void Awake()
    {
        destination = GameObject.Find("Goal").GetComponent<Destination>();
    }

    private void Update()
    {
        if (destination.goalReached)
        {
            PlayerPrefs.SetInt("CompleteTheGame", 1);
            PlayerPrefs.Save();
        }
    }
}