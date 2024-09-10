using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField] private List<Light> attatchedLights;
    private List<LightFlicker> lightFlickers = new List<LightFlicker>();

    public override void OnInteract()
    {
        if (lightFlickers.Count == 0)
        {
            foreach (var light in attatchedLights)
            {
                lightFlickers.Add(light.GetComponent<LightFlicker>());
            }
        }

        bool turnOn = true;

        foreach (var light in attatchedLights)
        {
            if (light.intensity != 0f)
            {
                turnOn = false;
            }
        }

        foreach (var light in lightFlickers)
        {
            if (turnOn)
            {
                light.TurnOnLight();
            }
            else
            {
                light.TurnOffLight();
            }
        }

        base.OnInteract();
    }
}
