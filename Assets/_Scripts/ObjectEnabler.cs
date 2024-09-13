using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnabler : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private bool day1 = true;
    [SerializeField] private bool day2 = true;
    [SerializeField] private bool day3 = true;
    [SerializeField] private bool day4 = true;
    [SerializeField] private bool day5 = true;
    [SerializeField] private bool day6 = true;
    [SerializeField] private bool day7 = true;

    private void OnEnable()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        TimeManager.OnDayChanged += DayChanged;
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= DayChanged;
    }

    private void DayChanged(int day)
    {
        try
        {
            if (day1 && day == 1) meshRenderer.enabled = true;
            else if (day == 1) meshRenderer.enabled = false;
            if (day2 && day == 2) meshRenderer.enabled = true;
            else if (day == 2) meshRenderer.enabled = false;
            if (day3 && day == 3) meshRenderer.enabled = true;
            else if (day == 3) meshRenderer.enabled = false;
            if (day4 && day == 4) meshRenderer.enabled = true;
            else if (day == 4) meshRenderer.enabled = false;
            if (day5 && day == 5) meshRenderer.enabled = true;
            else if (day == 5) meshRenderer.enabled = false;
            if (day6 && day == 6) meshRenderer.enabled = true;
            else if (day == 6) meshRenderer.enabled = false;
            if (day7 && day == 7) meshRenderer.enabled = true;
            else if (day == 7) meshRenderer.enabled = false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An exception occurred in ObjectEnabler: " + this.gameObject + " Exception: " + ex.Message);
        }
    }
}
