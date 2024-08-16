using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSystem : MonoBehaviour
{
    [SerializeField] private FirstPersonController playerController;

    private int completedTasks;
    private int failedTasks;

    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject QTEPrefab;
    [SerializeField] private GameObject ButtonSpamPrefab;
    [SerializeField] private GameObject PrecicionBarPrefab;

    private void Start()
    {
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController on TaskSystem is null... Calling FindObjectOfType");
            playerController = FindObjectOfType<FirstPersonController>();
        }

        if (canvas == null)
        {
            Debug.LogWarning("Canvas on TaskSystem is null... Calling FindObjectOfType");
            canvas = FindObjectOfType<Canvas>().transform;
        }
    }

    public void StartTask(Task task, Vector2 positionOnScreen)
    {
        playerController.canMove = false;

        switch (task.type)
        {
            case Task.TaskType.QTE:
                ShowTask(task, QTEPrefab, positionOnScreen);
                break;

            case Task.TaskType.ButtonSpam:
                ShowTask(task, ButtonSpamPrefab, positionOnScreen);
                break;

            case Task.TaskType.PrecicionBar:
                ShowTask(task, PrecicionBarPrefab, positionOnScreen);
                break;

            default:
                break;
        }
    }

    private void ShowTask(Task task, GameObject taskPrefab, Vector2 positionOnScreen)
    {
        TaskAction taskAction = Instantiate(taskPrefab, positionOnScreen, Quaternion.identity, canvas).GetComponent<TaskAction>();
        taskAction.transform.localPosition = positionOnScreen;
        taskAction.task = task;
        task.OnSuccess += TaskSuccess;
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
