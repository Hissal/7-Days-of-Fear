using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Work : MonoBehaviour
{
    private const int WORKSTART = 8;
    private const int WORKEND = 18;

    [SerializeField] private Animator animator;
    [SerializeField] AnimationClip anim;
    [SerializeField] private Transform backFromWorkPositionTransform;

    private int totalTimeLate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FirstPersonController>() != null)
        {
            StartCoroutine(GoToWork());
        }
    }

    private IEnumerator GoToWork()
    {
        if (TimeManager.hour >= WORKSTART)
        {
            int hoursLate = TimeManager.hour - WORKSTART;
            totalTimeLate += TimeManager.minute + (hoursLate * 60);
        }

        // Play Working Animation (MaybeMinigame)
        animator.Play(anim.name);

        GameManager.Instance.TakeAwayPlayerControl();
        
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f);

        BackHome();
    }

    private void BackHome()
    {
        TimeManager.SetTime(TimeManager.day, WORKEND, 0);

        GameManager gameManager = GameManager.Instance;
        gameManager.playerTransform.position = backFromWorkPositionTransform.position;
        gameManager.playerTransform.rotation = backFromWorkPositionTransform.rotation;
        gameManager.GivePlayerControlBack();
    }
}
