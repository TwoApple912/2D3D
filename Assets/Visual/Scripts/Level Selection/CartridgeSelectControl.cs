using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartridgeSelectControl : MonoBehaviour
{
    public GameObject gameCartridgeHolder;
    public GameObject selectionButton;

    private void Start()
    {
        selectionButton.SetActive(false);
    }

    public void enableGHC()
    {
        gameCartridgeHolder.SetActive(true);
        selectionButton.SetActive(true);
    }

    public void disableGHC()
    {
        // gameCartridgeHolder.SetActive(false);
        selectionButton.SetActive(false);
    }
}
