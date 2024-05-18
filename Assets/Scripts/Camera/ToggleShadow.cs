using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleShadow : MonoBehaviour
{
    public bool noShadow = false;

    [SerializeField] private Light[] softLights;
    [SerializeField] private Light[] hardLights;

    private void OnPreRender()
    {
        foreach (Light light in softLights) light.shadows = LightShadows.None;
        foreach (Light light in hardLights) light.shadows = LightShadows.None;
    }

    /*private void OnPostRender()
    {
        foreach (Light light in softLights) light.shadows = LightShadows.Soft;
        foreach (Light light in hardLights) light.shadows = LightShadows.Hard;
    }*/
}