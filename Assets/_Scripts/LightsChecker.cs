using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsChecker : MonoBehaviour
{
    [SerializeField] private LightSwitch[] lightSwitches;

    [SerializeField] private QuestObjective questObjective;
    private bool checkLights = false;
    private bool lightsOff;

    private void Awake()
    {
        TimeManager.OnEvening += TurnCheckLightsOff;
        TimeManager.OnMorning += TurnCheckLightsOff;
    }

    private void Start()
    {
        questObjective.OnObjectiveActivated += ObjectiveActivated;

        foreach (LightSwitch lightSwitch in lightSwitches)
        {
            lightSwitch.OnLightSwitch += OnLightSwitch;
        }
    }

    private void ObjectiveActivated(bool active)
    {
        if (active)
        {
            checkLights = true;
            StartCoroutine(CheckLightsAfterDelay(1f));
        }
    }
    private IEnumerator CheckLightsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CheckLights();
    }

    private void OnLightSwitch()
    {
        StopAllCoroutines();
        if (checkLights) CheckLights();
    }

    private void CheckLights()
    {
        print("Checking lights... AllLightsOff = " + AreAllLightsOff());

        if (AreAllLightsOff())
        {
            questObjective.OnComplete();
            lightsOff = true;
        }
        else if (lightsOff)
        {
            Debug.Log("Lights are on again");
            questObjective.ConditionBroken();
        }

        HighlightTurnedOnLightSwitches();
    }

    private void HighlightTurnedOnLightSwitches()
    {
        foreach (LightSwitch lightSwitch in lightSwitches)
        {
            if (lightSwitch.LightsOn)
            {
                lightSwitch.Highlight();
            }
            else
            {
                lightSwitch.DeHighligh();
            }
        }
    }

    public void TurnCheckLightsOff()
    {
        checkLights = false;
    }

    public bool AreAllLightsOn()
    {
        bool allLightsOn = true;

        foreach (LightSwitch lightSwitch in lightSwitches)
        {
            if (!lightSwitch.LightsOn)
            {
                allLightsOn = false;
            }
        }
        return allLightsOn;
    }
    public  bool AreAllLightsOff()
    {
        bool allLightsOff = true;

        foreach (LightSwitch lightSwitch in lightSwitches)
        {
            if (lightSwitch.LightsOn)
            {
                allLightsOff = false;
            }
        }
        return allLightsOff;
    }
}
