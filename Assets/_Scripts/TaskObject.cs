using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskObject : MonoBehaviour
{
    [SerializeField] private Task task;

    TaskSystem taskSystem;

    private void Start()
    {
        taskSystem = TaskSystem.Instance;

        task.onSucces += SucceedTask;
        task.onFail += FailTask;
    }

    private void BeginTask()
    {
        // TODO Player Loses Control Untill succes or fail
        // TODO Choose Task Type to Do
    }

    private void SucceedTask()
    {
        taskSystem.TaskSuccess(task);
    }

    private void FailTask()
    {
        taskSystem.TaskFail(task);
    }
}
