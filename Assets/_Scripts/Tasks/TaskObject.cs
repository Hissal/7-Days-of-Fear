using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskObject : Interactable
{
    [SerializeField] private Task task;

    private TaskSystem taskSystem;

    private void Start()
    {
        taskSystem = TaskSystem.Instance;

        task.OnSuccess += SucceedTask;
        task.OnFail += FailTask;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            BeginTask();
        }
    }

    private void BeginTask()
    {
        // TODO Player Loses Control Untill succes or fail
        taskSystem.StartTask(task, Vector2.zero);
        // TODO Choose Task Type to Do
    }

    private void SucceedTask(Task task)
    {
        
    }

    private void FailTask(Task task)
    {
        
    }

    public override void OnFocus()
    {
        print("Looking at" + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("No longer looking at " + gameObject.name);
    }

    public override void OnInteract()
    {

        print("Interacted with " + gameObject.name);
    }
}
