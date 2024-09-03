using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField] Light attatchedLight;

    [SerializeField] private float lightIntensity;

    public override void OnInteract()
    {
        if (attatchedLight.intensity == lightIntensity)
        {
            attatchedLight.intensity = 0;
        }
        else
        {
            attatchedLight.intensity = lightIntensity;
        }

        base.OnInteract();
    }
}
