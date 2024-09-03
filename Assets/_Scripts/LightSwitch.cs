using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField] private List<Light> attatchedLights;

    [SerializeField] private float lightIntensity;

    public override void OnInteract()
    {
        bool turnOn = false;

        foreach (var light in attatchedLights)
        {
            if (light.intensity == 0f)
            {
                turnOn = true;
            }
        }

        foreach (var light in attatchedLights)
        {
            if (turnOn)
            {
                light.intensity = lightIntensity;
            }
            else
            {
                light.intensity = 0f;
            }
        }

        base.OnInteract();
    }
}
