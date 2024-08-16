using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskObject : MonoBehaviour
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
}
