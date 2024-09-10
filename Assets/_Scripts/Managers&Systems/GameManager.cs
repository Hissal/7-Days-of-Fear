using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [field: Header("Player")]
    [field: SerializeField] public Transform playerTransform { get; private set; }
    [field: SerializeField] public FirstPersonController playerController { get; private set; }
    [SerializeField] private InteractionHandler playerInteractionHandler;

    [field: Header("Enemy")]
    [field: SerializeField] public EnemyAI enemyAI { get; private set; }
    [SerializeField] private Transform enemySpawnPosition;
    [SerializeField] private Transform enemyDisabledPosition;

    [field: Header("Hiding")]
    [SerializeField] private List<HidingSpot> hidingSpots;
    [SerializeField] private BreathHoldingMinigame breathHoldingMinigame;

    [field: Header("Other")]
    [SerializeField] private GameObject pauseScreen;

    public bool paused { get; private set; }
    private float timeScaleBeforePause;
    private bool cursorVisibleBeforePause;

    private CursorLockMode lockModeBeforePause;

    public bool isPlayerDead { get; private set; }

    [SerializeField] private PlayableDirector director;

    private int currentDay;
    public int GetCurrentDay()
    {
        return currentDay;
    }

    private void Start()
    {
        HideCursor();

        MentalHealth.Instance.OnMentalHealthReachZero += EnableEnemy;
        DisableEnemy();

        foreach (var hidingSpot in hidingSpots)
        {
            hidingSpot.onPlayerEnter += enemyAI.PlayerEnteredHidingSpot;
        }
    }

    public bool IsPlayerHoldingBreath()
    {
        if (breathHoldingMinigame.gameObject.activeInHierarchy == false) return false;

        if (breathHoldingMinigame.holdingBreath) return true;
        else return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (isPlayerDead) return;

        paused = true;
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;

        lockModeBeforePause = Cursor.lockState;
        cursorVisibleBeforePause = Cursor.visible;

        ShowCursor();

        pauseScreen.transform.SetAsLastSibling();
        pauseScreen.SetActive(true);
    }
    public void UnPause()
    {
        paused = false;
        Time.timeScale = timeScaleBeforePause;

        Cursor.lockState = lockModeBeforePause;
        Cursor.visible = cursorVisibleBeforePause;

        pauseScreen.SetActive(false);
    }

    public void KillPlayer()
    {
        isPlayerDead = true;
        TakeAwayPlayerControl();
        StunEnemy(-1);
        director.Play();
    }

    public void TakeAwayPlayerControl()
    {
        playerController.canMove = false;
        playerInteractionHandler.canInteract = false;
    }

    public void GivePlayerControlBack()
    {
        playerController.canMove = true;
        playerInteractionHandler.canInteract = true;
    }

    public void StunEnemyWithDelay(float delay, float time)
    {
        StartCoroutine(StunAfterDelay());
        IEnumerator StunAfterDelay()
        {
            yield return new WaitForSeconds(delay);
            StunEnemy(time);
        }
    }
    public void StunEnemy(float time)
    {
        enemyAI.Stun(time);
    }
    public void UnStunEnemy()
    {
        enemyAI.UnStun();
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void EnableEnemy()
    {
        enemyAI.Activate(enemySpawnPosition.position, enemySpawnPosition.rotation);
        StartCoroutine(DisableEnemyRoutine());
    }
    private void DisableEnemy()
    {
        enemyAI.Disable(enemyDisabledPosition.position);
        MentalHealth.Instance.IncreaseMentalHealth(25f);
    }
    private IEnumerator DisableEnemyRoutine()
    {
        yield return new WaitUntil(() => MentalHealth.Instance.currentMentalHealth >= 25f);
        DisableEnemy();
    }

    public static GameManager Instance;

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
