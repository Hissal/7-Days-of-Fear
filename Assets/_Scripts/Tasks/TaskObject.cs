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
        OnLoseFocus();
    }

    private void FailTask(Task task)
    {
        
    }

    public override void OnFocus()
    {
        if (!canBeInteractedWith) return;

        GetComponent<Renderer>().material.color = Color.white;
        print("Looking at" + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        if (!canBeInteractedWith) return;

        GetComponent<Renderer>().material.color = Color.gray;
        print("No longer looking at " + gameObject.name);
    }

    public override void OnInteract()
    {
        if (!canBeInteractedWith) return;

        BeginTask();
        print("Interacted with " + gameObject.name);
    }
}
