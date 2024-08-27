using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public Transform playerTransform { get; private set; }
    [field: SerializeField] public FirstPersonController playerController { get; private set; }
    [SerializeField] private InteractionHandler playerInteractionHandler;

    [SerializeField] private EnemyAI enemyAI;

    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private List<HidingSpot> hidingSpots;
    [SerializeField] private BreathHoldingMinigame breathHoldingMinigame;

    public bool paused { get; private set; }
    private float timeScaleBeforePause;
    private bool cursorVisibleBeforePause;

    private CursorLockMode lockModeBeforePause;

    private int currentDay;
    public int GetCurrentDay()
    {
        return currentDay;
    }

    private void Start()
    {
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
        paused = true;
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0;

        lockModeBeforePause = Cursor.lockState;
        cursorVisibleBeforePause = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

    public void StunEnemy(float time)
    {
        enemyAI.Stun(time);
    }
    public void UnStunEnemy()
    {
        enemyAI.UnStun();
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
