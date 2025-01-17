using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskObject : Interactable
{
    [SerializeField] private Task task;

    private TaskSystem taskSystem;

    private bool active = false;
    private bool paused = false;

    //[SerializeField] private bool morningTask;
    //[SerializeField] private bool eveningTask;

    [SerializeField] private float volume = 0.2f;
    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip loopSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;

    [SerializeField] private QuestObjective questObjective;

    private AudioManager audioManager;

    private AudioSource loopingAudioSource;

    private void OnEnable()
    {
        if (questObjective != null)
        {
            questObjective.OnObjectiveActivated += SetActive;
            questObjective.OnObjectiveHighlight += Highlight;
            questObjective.OnObjectivePaused += Pause;
        }

        task.OnSuccess += SucceedTask;
        task.OnFail += FailTask;
    }

    private void Start()
    {
        taskSystem = TaskSystem.Instance;
        audioManager = AudioManager.Instance;

        //if (morningTask) TimeManager.OnMorning += ActivateTask;
        //if (eveningTask) TimeManager.OnMorning += ActivateTask;
    }

    private void Pause(bool pause)
    {
        paused = pause;
        base.OnLoseFocus();
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
        if (startSound != null) audioManager.PlayAudioClip(startSound, transform.position, volume);
        if (loopSound != null) loopingAudioSource = audioManager.PlayAudioClip(loopSound, transform.position, volume, true);
    }

    private void SucceedTask(Task task)
    {
        base.OnLoseFocus();
        if (!active || paused) return;

        if (successSound != null) audioManager.PlayAudioClip(successSound, transform.position, volume);

        if (loopingAudioSource != null)
        {
            loopingAudioSource.Stop();
            loopingAudioSource = null;
        }

        questObjective.OnComplete();
        SetActive(false);
    }

    private void FailTask(Task task)
    {
        base.OnLoseFocus();
        if (!active || paused) return;

        if (failSound != null) audioManager.PlayAudioClip(failSound, transform.position, volume);

        if (loopingAudioSource != null)
        {
            loopingAudioSource.Stop();
            loopingAudioSource = null;
        }

        questObjective.OnComplete();
        SetActive(false);
    }

    public override void OnFocus()
    {
        if (!active || paused) return;

        base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        if (!active || paused) return;

        base.OnLoseFocus();
    }

    public override void OnInteract()
    {
        if (!active || paused) return;

        BeginTask();
    }

    private void OnDisable()
    {
        task.OnSuccess -= SucceedTask;
        task.OnFail -= FailTask;

        if (questObjective != null)
        {
            questObjective.OnObjectiveActivated -= SetActive;
            questObjective.OnObjectiveHighlight -= Highlight;
            questObjective.OnObjectivePaused -= Pause;
        }

        //if (morningTask) TimeManager.OnMorning -= ActivateTask;
        //if (eveningTask) TimeManager.OnMorning -= ActivateTask;
    }
}
