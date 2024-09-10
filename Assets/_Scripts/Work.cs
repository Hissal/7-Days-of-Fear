using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Work : MonoBehaviour
{
    private const int WORKSTART = 8;
    private const int WORKEND = 18;

    [SerializeField] private Animator animator;
    [SerializeField] AnimationClip anim;
    [SerializeField] private Transform backFromWorkPositionTransform;

    [SerializeField] Cinematic workCinematic;

    private int totalTimeLate;

    [SerializeField] private Outline outline;
    [SerializeField] private QuestObjective questObjective;
    private bool active = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (other.GetComponent<FirstPersonController>() != null)
        {
            GoToWork();
        }
    }

    private void GoToWork()
    {
        questObjective.OnComplete();

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
        TimeManager.SetTime(TimeManager.day, WORKEND, 0);
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
