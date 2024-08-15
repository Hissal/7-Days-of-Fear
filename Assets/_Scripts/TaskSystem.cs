using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSystem : MonoBehaviour
{
    [SerializeField] private FirstPersonController playerController;

    private int completedTasks;
    private int failedTasks;

    [SerializeField] private GameObject QTEPrefab;
    [SerializeField] private GameObject ButtonSpamPrefab;
    [SerializeField] private GameObject PrecicionBarPrefab;


    private void StartTask(Task task, Vector2 positionOnScreen)
    {
        playerController.canMove = false;

        switch (task.type)
        {
            case Task.TaskType.QTE:

                break;
            default:
                break;
        }
    }

    public void TaskSuccess(Task task)
    {
        TaskDone(task);
        completedTasks++;
    }

    public void TaskFail(Task task)
    {
        TaskDone(task);
        failedTasks++;
    }

    private void TaskDone(Task task)
    {
        playerController.canMove = true;
    }


    public static TaskSystem Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
