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

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FirstPersonController>() != null)
        {
            GoToWork();
        }
    }

    private void GoToWork()
    {
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

    }
}
