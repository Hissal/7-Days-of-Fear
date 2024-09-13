using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Work : Interactable
{
    private const int WORKSTART = 8;
    private const int WORKEND = 18;

    [SerializeField] private Animator animator;
    [SerializeField] AnimationClip anim;
    [SerializeField] private Transform backFromWorkPositionTransform;

    [SerializeField] Cinematic workCinematic;

    private int totalTimeLate;

    [SerializeField] private QuestObjective questObjective;
    private bool active = false;

    [SerializeField] private LightSwitch lightSwitch;

    [SerializeField] private AudioClip goToWorkSound;
    [SerializeField] private AudioClip backHomeSound;

    private void Start()
    {
        if (questObjective != null)
        {
            questObjective.OnObjectiveActivated += SetActive;
            questObjective.OnObjectiveHighlight += Highlight;
        }

        outline.enabled = false;
    }

    private void SetActive(bool active)
    {
        this.active = active;
        if (!active) outline.enabled = false;
    }
    private void Highlight()
    {
        if (outline != null) outline.enabled = true;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!active) return;
    //    if (other.GetComponent<FirstPersonController>() != null)
    //    {
    //        GoToWork();
    //    }
    //}

    public override void OnInteract()
    {
        // TODO Play locked sound if enemy is active
        if (!active || GameManager.Instance.enemyActive) return;

        GoToWork();
        base.OnInteract();
        base.OnLoseFocus();
    }
    override public void OnFocus()
    {
        if (active) base.OnFocus();
    }
    override public void OnLoseFocus()
    {
        if (active) base.OnLoseFocus();
    }

    private void GoToWork()
    {
        AudioManager.Instance.PlayAudioClip(goToWorkSound, transform.position, 0.2f);

        questObjective.OnComplete();
        MentalHealth.Instance.PauseDrainage();
        MentalHealth.Instance.mentalHealthDrainagePauseManual = true;
        MentalHealth.Instance.IncreaseMentalHealth(10f);

        if (TimeManager.hour >= WORKSTART)
        {
            int hoursLate = TimeManager.hour - WORKSTART;
            totalTimeLate += TimeManager.minute + (hoursLate * 60);

            if (totalTimeLate > 120)
            {
                GetFired();
                //return;
            }
        }

        CinematicManager.Instance.PlayCinematic(workCinematic);
        workCinematic.director.stopped += BackHome;
    }

    private void BackHome(PlayableDirector director)
    {
        AudioManager.Instance.PlayAudioClip(backHomeSound, transform.position, 0.2f);
        TimeManager.SetTime(TimeManager.day, WORKEND, 0, false, false);
        TimeManager.OnEveningInvoke();
        MentalHealth.Instance.ResumeDrainage();
        MentalHealth.Instance.mentalHealthDrainagePauseManual = false;
        director.stopped -= BackHome;
    }

    public void TurnOnLights()
    {
        lightSwitch.TurnOnLights();
    }

    private void GetFired()
    {
        print("Got Fired");
    }

    private void OnDisable()
    {
        if (questObjective != null)
        {
            questObjective.OnObjectiveActivated -= SetActive;
            questObjective.OnObjectiveHighlight -= Highlight;
        }
    }
}
