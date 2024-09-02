using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSystem : MonoBehaviour
{
    private int completedTasks;
    private int failedTasks;

    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject QTEPrefab;
    [SerializeField] private GameObject ButtonSpamPrefab;
    [SerializeField] private GameObject PrecicionBarPrefab;

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
        task.OnFail += TaskFail;
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
        task.OnSuccess -= TaskSuccess;
        task.OnFail -= TaskFail;

        GameManager.Instance.GivePlayerControlBack();
    }

    public void EnableBreathHolding()
    {
        // Breath Holding ui appears and sends message to enemy to walk to hiding spot and play minigame if enemy is close enough on entry

        // TODO on success make enemy walk to another room
        // TODO on fail Kill player
    }

    public void DisableBreathHolding()
    {
        // Disable breath holding
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
