using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EscapeDoor : CinematicInteractable
{
    [SerializeField] private QuestObjective questObjective;

    public bool active { get; private set; } = false;
    public bool paused { get; private set; } = false;

    private void OnEnable()
    {
        if (questObjective != null)
        {
            questObjective.OnObjectiveActivated += SetActive;
            questObjective.OnObjectiveHighlight += Highlight;
            questObjective.OnObjectivePaused += Pause;
        }
    }
    private void OnDisable()
    {
        if (questObjective != null)
        {
            questObjective.OnObjectiveActivated -= SetActive;
            questObjective.OnObjectiveHighlight -= Highlight;
            questObjective.OnObjectivePaused -= Pause;
        }
    }

    private void Pause(bool pause)
    {
        paused = pause;
        base.OnLoseFocus();
    }
    private void SetActive(bool active)
    {
        this.active = active;
    }
    private void Highlight()
    {
        outline.enabled = true;
    }

    public override void OnInteract()
    {
        if (active && !paused)
        {
            base.OnInteract();
        }
    }

    public override void OnFocus()
    {
        if (active && !paused)
        {
            base.OnFocus();
        }
    }

    public override void OnLoseFocus()
    {
        if (active && !paused)
        {
            base.OnLoseFocus();
        }
    }

}
