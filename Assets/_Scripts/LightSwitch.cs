using Assets._Scripts.Managers_Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField] private List<Light> attatchedLights;
    private List<LightFlicker> lightFlickers = new List<LightFlicker>();
    public bool LightsOn => attatchedLights.TrueForAll(light => light.intensity != 0f);
    public event Action OnLightSwitch = delegate { };

    [SerializeField] private AudioClip switchOnSound;
    [SerializeField] private AudioClip switchOffSound;

    public override void OnInteract()
    {
        ToggleLights();
        OnLightSwitch?.Invoke();

        base.OnInteract();
    }

    public void Highlight()
    {
        outline.enabled = true;
    }
    public void DeHighligh()
    {
        outline.enabled = false;
    }

    public void ToggleLights()
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

        if (turnOn)
        {
            TurnOnLights();
        }
        else
        {
            TurnOffLights();
        }
    }

    public void TurnOnLights()
    {
        if (lightFlickers.Count == 0)
        {
            foreach (var light in attatchedLights)
            {
                lightFlickers.Add(light.GetComponent<LightFlicker>());
            }
        }

        if (!LightsOn)
        {
            if (switchOnSound != null)
            {
                AudioManager.Instance.PlayAudioClip(switchOnSound, transform.position, 0.2f);
            }
            foreach (var light in lightFlickers)
            {
                light.TurnOnLight();
            }
        }
    }

    public void TurnOffLights()
    {
        if (lightFlickers.Count == 0)
        {
            foreach (var light in attatchedLights)
            {
                lightFlickers.Add(light.GetComponent<LightFlicker>());
            }
        }

        AudioManager.Instance.PlayAudioClip(switchOffSound, transform.position, 0.2f);

        foreach (var light in lightFlickers)
        {
            light.TurnOffLight();
        }
    }
}
