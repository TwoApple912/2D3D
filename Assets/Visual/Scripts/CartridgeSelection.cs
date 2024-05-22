using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartridgeSelection : MonoBehaviour
{
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    private int currentCartridge;

    private void Awake()
    {
        SelectCartridge(0);
    }

    private void SelectCartridge(int _index)
    {
        previousButton.interactable = (_index != 0);
        nextButton.interactable = (_index != transform.childCount - 1);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _index);
        }
        
    }

    public void ChangeCartridge(int _change)
    {
        currentCartridge += _change;
        SelectCartridge(currentCartridge);
    }
}
