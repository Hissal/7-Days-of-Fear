using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSystem : MonoBehaviour
{
    private int completedTasks;
    private int failedTasks;

    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject precisionBar;

    private void Start()
    {
        if (canvas == null)
        {
            Debug.LogWarning("Canvas on TaskSystem is null... Calling FindObjectOfType");
            canvas = FindObjectOfType<Canvas>().transform;
        }
    }

    public void StartTask(Task task, Vector2 positionOnScreen)
    {
        GameManager.Instance.TakeAwayPlayerControl();

        switch (task.type)
        {
            case Task.TaskType.PrecicionBar:
                ShowTask(task, precisionBar, positionOnScreen);
                break;

            default:
                break;
        }
    }

    private void ShowTask(Task task, GameObject taskUIObject, Vector2 positionOnScreen)
    {
        Reticle.HideReticle_Static();

        TaskAction taskAction = taskUIObject.GetComponent<TaskAction>();
        taskAction.transform.localPosition = positionOnScreen;
        taskAction.task = task;
        task.OnSuccess += TaskSuccess;
        task.OnFail += TaskFail;

        taskAction.gameObject.SetActive(true);
        taskAction.Init();
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
        Reticle.ShowReticle_Static();

        task.OnSuccess -= TaskSuccess;
        task.OnFail -= TaskFail;

        GameManager.Instance.GivePlayerControlBack();
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
