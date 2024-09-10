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
            questObjective.OnObjectiveActivated += SetActive;
            questObjective.OnObjectiveHighlight += Highlight;
        }

        taskSystem = TaskSystem.Instance;

        task.OnSuccess += SucceedTask;
        task.OnFail += FailTask;

        //if (morningTask) TimeManager.OnMorning += ActivateTask;
        //if (eveningTask) TimeManager.OnMorning += ActivateTask;
    }

    private void SetActive(bool active)
    {
        this.active = active;
    }
    private void Highlight()
    {
        outline.enabled = true;
    }

    private void BeginTask()
    {
        taskSystem.StartTask(task, Vector2.zero);
    }

    private void SucceedTask(Task task)
    {
        questObjective.OnComplete();
        SetActive(false);
        base.OnLoseFocus();
    }

    private void FailTask(Task task)
    {
        questObjective.OnComplete();
        SetActive(false);
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

    private void OnDestroy()
    {
        task.OnSuccess -= SucceedTask;
        task.OnFail -= FailTask;

        questObjective.OnObjectiveActivated -= SetActive;
        questObjective.OnObjectiveHighlight -= Highlight;

        //if (morningTask) TimeManager.OnMorning -= ActivateTask;
        //if (eveningTask) TimeManager.OnMorning -= ActivateTask;
    }
}
