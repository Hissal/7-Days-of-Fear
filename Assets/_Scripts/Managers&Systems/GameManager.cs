using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject[] disableOnGameOver;

    [Header("MentalHealthGainedOnEnemyDisable")]
    [SerializeField] private float mentalHealthGainedDay1 = 25f;
    [SerializeField] private float mentalHealthGainedDay2 = 25f;
    [SerializeField] private float mentalHealthGainedDay3 = 25f;
    [SerializeField] private float mentalHealthGainedDay4 = 20f;
    [SerializeField] private float mentalHealthGainedDay5 = 10f;
    [SerializeField] private float mentalHealthGainedDay6 = -10f;
    [SerializeField] private float mentalHealthGainedDay7 = -20f;
    private float mentalHealthGained = 0f;

    public bool paused { get; private set; }
    private float timeScaleBeforePause;
    private bool cursorVisibleBeforePause;

    private CursorLockMode lockModeBeforePause;

    public bool isPlayerDead { get; private set; }

    [SerializeField] private PlayableDirector director;

    private bool enemyFirstAppearance = true;

    public bool enemyActive { get; private set; }

    [SerializeField] private LightFlicker[] lightFlickers;

    private void OnEnable()
    {
        TimeManager.OnDayChanged += SetMentalHealthGained;
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= SetMentalHealthGained;
    }

    private void SetMentalHealthGained(int day)
    {
        switch (day)
        {
            case 1:
                mentalHealthGained = mentalHealthGainedDay1;
                break;
            case 2:
                mentalHealthGained = mentalHealthGainedDay2;
                break;
            case 3:
                mentalHealthGained = mentalHealthGainedDay3;
                break;
            case 4:
                mentalHealthGained = mentalHealthGainedDay4;
                break;
            case 5:
                mentalHealthGained = mentalHealthGainedDay5;
                break;
            case 6:
                mentalHealthGained = mentalHealthGainedDay6;
                break;
            case 7:
                mentalHealthGained = mentalHealthGainedDay7;
                break;
            default:
                break;
        }
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
        if (isPlayerDead) return;

        foreach (GameObject GM in disableOnGameOver)
        {
            GM.SetActive(false);
        }

        isPlayerDead = true;
        TakeAwayPlayerControl();
        StunEnemy(-1);
        director.Play();
    }

    public void TakeAwayPlayerControl()
    {
        playerController.Stun();
        playerInteractionHandler.canInteract = false;
    }

    public void GivePlayerControlBack()
    {
        playerController.Unstun();
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
        enemyAI.UnStun(true);
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

    private IEnumerator FlickerAllLights()
    {
        List<float> lightIntensities = new List<float>();

        foreach (var lightFlicker in lightFlickers)
        {
            lightIntensities.Add(lightFlicker.light.intensity);
        }

        foreach (var lightFlicker in lightFlickers)
        {
            lightFlicker.flicker = true;
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < lightFlickers.Length; i++)
        {
            lightFlickers[i].light.intensity = lightIntensities[i];
        }

        foreach (var lightFlicker in lightFlickers)
        {
            lightFlicker.flicker = false;
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var lightFlicker in lightFlickers)
        {
            lightFlicker.flicker = true;
        }

        yield return new WaitForSeconds(1f);

        foreach (var lightFlicker in lightFlickers)
        {
            lightFlicker.flicker = false;
            int coinFlip = Random.Range(0, 2);
            if (coinFlip == 0)
            {
                lightFlicker.TurnOnLight();
            }
            else
            {
                lightFlicker.TurnOffLight();
            }
        }
    }


    public void EnableEnemyIfNotAppearedYet()
    {
        if (enemyFirstAppearance) MentalHealth.Instance.ReduceMentalHealth(100f);
    }
    private void EnableEnemy()
    {
        if (enemyAI == null) throw new System.Exception("EnemyAI is null");

        if (enemyAI.active) return;

        if (enemyFirstAppearance)
        {
            StartCoroutine(EnableEnemyWithDelay());
            QuestSystem.Instance.PauseCurrentQuest("HIDE!");
            enemyFirstAppearance = false;
        }
        else
        {
            enemyAI.Activate(enemySpawnPosition.position, enemySpawnPosition.rotation, false);
        }

        StartCoroutine(FlickerAllLights());

        

        MentalHealth.Instance.PauseDrainage();

        enemyActive = true;
    }
    IEnumerator EnableEnemyWithDelay()
    {
        yield return new WaitForSeconds(1f);
        enemyAI.Activate(enemySpawnPosition.position, enemySpawnPosition.rotation, false);
    }

    public void EnableEnemyLastEscape()
    {
        if (enemyAI == null) throw new System.Exception("EnemyAI is null");

        if (enemyAI.active) return;

        enemyAI.Activate(enemySpawnPosition.position, enemySpawnPosition.rotation, true);

        MentalHealth.Instance.PauseDrainage();

        enemyActive = true;
    }

    public void DisableEnemy()
    {
        if (enemyAI == null) throw new System.Exception("EnemyAI is null");
        if (enemyAI.cantDeactivate) return;

        if (QuestSystem.Instance.paused) QuestSystem.Instance.ResumeCurrentQuest();

        if (mentalHealthGained > 0)
        {
            MentalHealth.Instance.IncreaseMentalHealth(mentalHealthGained);
        }
        else
        {
            MentalHealth.Instance.ReduceMentalHealth(-mentalHealthGained);
        }

        MentalHealth.Instance.ResumeDrainage();
        enemyAI.Disable(enemyDisabledPosition.position);

        enemyActive = false;
    }

    public void GameWin()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        TimeManager.SetDayDirty(0);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
