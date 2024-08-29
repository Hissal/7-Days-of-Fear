using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class Cinematic : MonoBehaviour
{
    [field: SerializeField] public bool isFirstCinematicOfChain;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private bool playOnStart;
    private bool played;

    [field: Header("PlayerStartProperties (Only matters if isFirstCinematicOfChain is true)")]
    [field: SerializeField] public Vector3 playerStartPosition { get; private set; }
    [field: SerializeField] public Vector3 playerStartRotation { get; private set; }
    [field: SerializeField] public Vector3 cameraStartRotation { get; private set; }
    [field: SerializeField] public float setupTime { get; private set; }

    [Header("PlayerEndProperties (Only matters if cinematic is the last one of a chain)")]
    [SerializeField] private Vector3 playerEndPosition;
    [SerializeField] private Vector3 playerEndRotation;
    [SerializeField] private Vector3 cameraEndRotation;

    [Header("EnemyEndProperties (Only matters if cinematic is the last one of a chain)")]
    [SerializeField] private Vector3 enemyEndPosition;
    [SerializeField] private Vector3 enemyEndRotation;

    [Header("QTE")]
    [SerializeField] private bool hasQTE;
    [SerializeField] private float qteAppearanceTime;
    [SerializeField] private float qteDuration;
    [SerializeField] private Vector2 qtePosition;
    [SerializeField] private Cinematic qteSuccessCinematic;
    [SerializeField] private Cinematic qteFailCinematic;

    private Camera cinematicCamera;
    private Cinematic nextCinematic;

    private void Start()
    {
        played = false;
        if (playOnStart) CinematicManager.Instance.PlayCinematic(this);
    }

    public void PlayCinematic(Camera playerCamera, Camera cinematicCamera)
    {
        if (played)
        {
            Debug.LogWarning($"Trying to play cinematic for the second time {this}");
            return;
        }

        GameManager.Instance.enemyAI.GetComponent<MeshRenderer>().enabled = false;

        this.cinematicCamera = cinematicCamera;

        cinematicCamera.fieldOfView = playerCamera.fieldOfView;

        playerCamera.enabled = false;
        cinematicCamera.enabled = true;

        played = true;
        director.stopped += EndCinematic;
        director.Play();

        if (hasQTE) Invoke("DisplayQTE", qteAppearanceTime);
    }

    public void EndCinematic(PlayableDirector director)
    {
        GameManager gameManager = GameManager.Instance;

        director.stopped -= EndCinematic;

        if (nextCinematic != null) CinematicManager.Instance.PlayCinematic(nextCinematic);
        else
        {
            gameManager.playerTransform.position = playerEndPosition;
            gameManager.playerTransform.rotation = Quaternion.Euler(playerEndRotation);
            gameManager.playerController.playerCamera.transform.localRotation = Quaternion.Euler(cameraEndRotation);
            gameManager.playerController.rotationX = cameraEndRotation.x;

            gameManager.enemyAI.transform.position = enemyEndPosition;
            gameManager.enemyAI.transform.rotation = Quaternion.Euler(enemyEndRotation);
            gameManager.enemyAI.GetComponent<MeshRenderer>().enabled = true;

            cinematicCamera.enabled = false;
            gameManager.playerController.playerCamera.enabled = true;

            gameManager.UnStunEnemy();
            gameManager.GivePlayerControlBack();
        }
        
        gameObject.SetActive(false);
    }

    public void DisplayQTE()
    {
        QTE newQTE = QTEManager.Instance.GenerateQTE(qtePosition, qteDuration);

        StartCoroutine(SlowTime());

        newQTE.OnSuccess += OnQTESuccess;
        newQTE.OnFail += OnQTEFail;
    }

    private void OnQTESuccess(QTE qte)
    {
        StopAllCoroutines();
        nextCinematic = qteSuccessCinematic;
        StartCoroutine(ReturnTime());
        UnSubscribeQTE(qte);
    }
    private void OnQTEFail(QTE qte)
    {
        StopAllCoroutines();
        nextCinematic = qteFailCinematic;
        StartCoroutine(ReturnTime());
        UnSubscribeQTE(qte);
    }
    private void UnSubscribeQTE(QTE qte)
    {
        qte.OnSuccess -= OnQTESuccess;
        qte.OnFail -= OnQTEFail;
    }

    private IEnumerator SlowTime()
    {
        float timeElapsed = 0;
        
        while (Time.timeScale != 0f)
        {
            timeElapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1, 0f, timeElapsed / qteDuration);
            if (Time.timeScale < 0f) Time.timeScale = 0f;
            yield return null;
        }
    }
    private IEnumerator ReturnTime()
    {
        float timeElapsed = 0;
        float currentTimeScale = Time.timeScale;

        while (Time.timeScale != 1f)
        {
            timeElapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(currentTimeScale, 1f, timeElapsed / 0.25f);
            if (Time.timeScale > 1f) Time.timeScale = 1f;
            yield return null;
        }
    }
}
