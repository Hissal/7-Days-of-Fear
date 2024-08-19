using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskObject : Interactable
{
    [SerializeField] private Task task;

    private TaskSystem taskSystem;

    private bool canBeInteractedWith = true;

    private void Start()
    {
        taskSystem = TaskSystem.Instance;

        task.OnSuccess += SucceedTask;
        task.OnFail += FailTask;
    }

    private void BeginTask()
    {
        taskSystem.StartTask(task, Vector2.zero);
    }

    private void SucceedTask(Task task)
    {
        canBeInteractedWith = false;

        base.OnLoseFocus();
    }

    private void FailTask(Task task)
    {

    }

    public override void OnFocus()
    {
        if (!canBeInteractedWith) return;

        base.OnFocus();

        //GetComponent<Renderer>().material.color = Color.white;
    }

    public override void OnLoseFocus()
    {
        if (!canBeInteractedWith) return;

        base.OnLoseFocus();

        //GetComponent<Renderer>().material.color = Color.gray;
    }

    public override void OnInteract()
    {
        if (!canBeInteractedWith) return;

        BeginTask();
    }
}
