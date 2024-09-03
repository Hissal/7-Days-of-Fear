using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnabler : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private bool day1;
    [SerializeField] private bool day2;
    [SerializeField] private bool day3;
    [SerializeField] private bool day4;
    [SerializeField] private bool day5;
    [SerializeField] private bool day6;
    [SerializeField] private bool day7;

    private void Start()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

        TimeManager.OnDayChanged += DayChanged;
    }

    private void DayChanged(int day)
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
}
