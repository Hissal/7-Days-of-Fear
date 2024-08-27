using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class Cinematic : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private bool playOnStart;
    private bool played;

    [field: Header("PlayerProperties")]
    [field: SerializeField] public Vector3 playerStartPosition { get; private set; }
    [field: SerializeField] public Vector3 playerStartRotation { get; private set; }
    [field: SerializeField] public Vector3 cameraStartRotation { get; private set; }
    [field: SerializeField] public float setupTime { get; private set; }

    private void Start()
    {
        played = false;
        if (playOnStart) CinematicManager.Instance.PlayCinematic(this);
    }

    public void PlayCinematic()
    {
        if (played)
        {
            Debug.LogWarning($"Trying to play cinematic for the second time {this}");
            return;
        }

        played = true;
        director.stopped += EndCinematic;
        director.Play();
    }

    public void EndCinematic(PlayableDirector director)
    {
        GameManager.Instance.UnStunEnemy();
        GameManager.Instance.GivePlayerControlBack();
        gameObject.SetActive(false);
    }
}
