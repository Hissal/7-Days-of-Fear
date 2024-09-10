using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskObject : Interactable
{
    [SerializeField] private Task task;

    private TaskSystem taskSystem;

    private bool active = false;

    //[SerializeField] private bool morningTask;
    //[SerializeField] private bool eveningTask;

    [SerializeField] private QuestObjective questObjective;

    private void Start()
    {
        if (questObjective != null)
        {
            questObjective.OnObjectiveActivated += ActivateTask;
            questObjective.OnObjectiveHighlight += Highlight;
        }

        taskSystem = TaskSystem.Instance;

        task.OnSuccess += SucceedTask;
        task.OnFail += FailTask;

        //if (morningTask) TimeManager.OnMorning += ActivateTask;
        //if (eveningTask) TimeManager.OnMorning += ActivateTask;
    }

    private void ActivateTask()
    {
        active = true;
    }
    private void Highlight()
    {
        outline.enabled = true;
    }

    private void DeactivateTask()
    {
        active = false;
    }

    private void BeginTask()
    {
        taskSystem.StartTask(task, Vector2.zero);
    }

    private void SucceedTask(Task task)
    {
        questObjective.OnComplete();
        DeactivateTask();
        base.OnLoseFocus();
    }

    private void FailTask(Task task)
    {
        questObjective.OnComplete();
        DeactivateTask();
        base.OnLoseFocus();
    }

    public override void OnFocus()
    {
        if (!active) return;

        base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        if (!active) return;

        base.OnLoseFocus();
    }

    public override void OnInteract()
    {
        if (!active) return;

        BeginTask();
    }

    private void OnDisable()
    {
        task.OnSuccess -= SucceedTask;
        task.OnFail -= FailTask;

        //if (morningTask) TimeManager.OnMorning -= ActivateTask;
        //if (eveningTask) TimeManager.OnMorning -= ActivateTask;
    }
}
