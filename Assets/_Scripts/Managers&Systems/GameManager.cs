using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.UI;

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

    [SerializeField] private Cinematic introCinematic;
    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private LightSwitch entranceSwitch;
    [SerializeField] private LightSwitch bedroomSwitch;

    [SerializeField] private AudioClip uiButtonPressSound;

    [SerializeField] private AudioSource staticAudioSource;

    [SerializeField] private Transform enemyGameOverPosition;
    [SerializeField] private LightFlicker bedroomLight;

    [SerializeField] private Slider sensitivitySlider;
    private void OnEnable()
    {
        TimeManager.OnDayChanged += SetMentalHealthGained;
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= SetMentalHealthGained;
    }

    public void WarpEnemyToGameOverPosition()
    {
        enemyAI.SetPosition(enemyGameOverPosition.position);
        enemyAI.SetRotation(enemyGameOverPosition.rotation);
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
        Time.timeScale = 1f;

        HideCursor();

        MentalHealth.Instance.OnMentalHealthReachZero += EnableEnemy;

        enemyActive = true;
        DisableEnemy(true);

        foreach (var hidingSpot in hidingSpots)
        {
            hidingSpot.onPlayerEnter += enemyAI.PlayerEnteredHidingSpot;
        }



        StartGame();
    }

    public void UiButtonPressed()
    {
        AudioManager.Instance.PlayAudioClip(uiButtonPressSound, playerTransform.position, 0.4f);
    }

    public void RestartDay()
    {
        UiButtonPressed();
        PlayerPrefs.SetInt("Retry", 1);
        PlayerPrefs.SetInt("DayToLoad", TimeManager.day);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadMainMenu()
    {
        UiButtonPressed();
        TimeManager.SetDayDirty(0);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void ActivateGameOverScreen()
    {
        WarpEnemyToGameOverPosition();
        staticAudioSource.volume = 0.1f;
        gameOverScreen.SetActive(true);
        gameOverScreen.transform.SetAsLastSibling();
    }

    private void StartGame()
    {
        int dayToLoad = PlayerPrefs.GetInt("DayToLoad");
        print("StartGame, Day: " + dayToLoad);

        //dayToLoad = 3;
        //PlayerPrefs.SetInt("Retry", 1);

        if (dayToLoad != 1)
        {
            TimeManager.SetTime(dayToLoad, 0, 0, true, true);
            
            bedroomSwitch.TurnOnLights();
        }
        else
        {
            PlayIntro();
            entranceSwitch.TurnOnLights();
        }
    }
    private void PlayIntro()
    {
        CinematicManager.Instance.PlayCinematic(introCinematic);
        PlayableDirector director = introCinematic.director;
        director.stopped += OnIntroEnd;
    }
    private void OnIntroEnd(PlayableDirector director)
    {
        director.stopped -= OnIntroEnd;
        TimeManager.SetTime(1, 0, 0, false, true);
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

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            sensitivitySlider.value = 2f;
        }
     
        sensitivitySlider.onValueChanged.Invoke(sensitivitySlider.value);

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
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        playerController.SetSensitivity(sensitivitySlider.value);

        paused = false;
        Time.timeScale = timeScaleBeforePause;

        Cursor.lockState = lockModeBeforePause;
        Cursor.visible = cursorVisibleBeforePause;

        pauseScreen.SetActive(false);
    }

    public void KillPlayer()
    {
        PlayerPrefs.SetInt("Star2", 0);
        PlayerPrefs.SetInt("Star5", 0);

        if (isPlayerDead) return;

        foreach (GameObject GM in disableOnGameOver)
        {
            GM.SetActive(false);
        }

        bedroomLight.TurnOnLight(true);
        isPlayerDead = true;
        TakeAwayPlayerControl();
        AmbienceController.Instance.FadeOutScaryAtmosphere();
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

            lightFlicker.CheckChildLights();
        }
    }

    public void FadeOffAudioSources(AudioSource[] sources, float fadeTime)
    {
        StartCoroutine(FadeOffAudioSourcesRoutine(sources, fadeTime));
    }
    IEnumerator FadeOffAudioSourcesRoutine(AudioSource[] sources, float fadeTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            print("Fading Off AudioSources");
            elapsedTime += Time.deltaTime;
            foreach (var audioSource in sources)
            {
                audioSource.volume = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            }
            yield return null;
        }
    }


    public void EnableEnemyIfNotAppearedYet()
    {
        if (enemyFirstAppearance) MentalHealth.Instance.ReduceMentalHealth(100f, true);
    }
    private void EnableEnemy()
    {
        if (enemyAI == null) throw new System.Exception("EnemyAI is null");

        if (enemyAI.active) return;

        float extraGracePerioid = 0f;

        if (enemyFirstAppearance && PlayerPrefs.GetInt("Hidden") == 0)
        {
            PlayerPrefs.SetInt("Hidden", 1);
            QuestSystem.Instance.PauseCurrentQuest("HIDE!");
            enemyFirstAppearance = false;
            extraGracePerioid = 1f;
        }

        enemyAI.Activate(enemySpawnPosition.position, enemySpawnPosition.rotation, false, extraGracePerioid);

        StartCoroutine(FlickerAllLights());

        AmbienceController.Instance.FadeInScaryAtmosphere();

        MentalHealth.Instance.PauseDrainage();

        enemyActive = true;
    }

    public void EnableEnemyLastEscape()
    {
        if (enemyAI == null) throw new System.Exception("EnemyAI is null");

        if (enemyAI.active) return;

        enemyAI.Activate(enemySpawnPosition.position, enemySpawnPosition.rotation, true, 0.5f);

        MentalHealth.Instance.PauseDrainage();

        enemyActive = true;
    }

    public void DisableEnemy(bool startDisabling = false)
    {
        if (enemyAI == null) throw new System.Exception("EnemyAI is null");
        if (enemyAI.cantDeactivate) return;

        if (QuestSystem.Instance.paused) QuestSystem.Instance.ResumeCurrentQuest();

        MentalHealth.Instance.ResumeDrainage();

        if (!enemyActive)
        {
            Debug.LogWarning("Trying to Disable enemy when it is already disabled");
            return;
        }

        enemyActive = false;

        enemyAI.Disable(enemyDisabledPosition.position);
        if (!startDisabling) AmbienceController.Instance.FadeOutScaryAtmosphere();

        if (mentalHealthGained > 0)
        {
            MentalHealth.Instance.IncreaseMentalHealth(mentalHealthGained);
        }
        else
        {
            MentalHealth.Instance.ReduceMentalHealth(-mentalHealthGained, true);
        }
    }

    public void GameWin()
    {
        if (PlayerPrefs.GetInt("Star1") == 1)
        {
            PlayerPrefs.SetInt("Star1Awarderd", 1);
        }
        if (PlayerPrefs.GetInt("Star2") == 1)
        {
            PlayerPrefs.SetInt("Star2Awarderd", 1);
        }
        if (PlayerPrefs.GetInt("Star3") == 1)
        {
            PlayerPrefs.SetInt("Star3Awarderd", 1);
        }
        if (PlayerPrefs.GetInt("Star4") == 1)
        {
            PlayerPrefs.SetInt("Star4Awarderd", 1);
        }
        if (PlayerPrefs.GetInt("Star5") == 1)
        {
            PlayerPrefs.SetInt("Star5Awarderd", 1);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LoadMainMenu();
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

    Coroutine slowTimeRoutine = null;
    Coroutine returnTimeRoutine = null;

    public void SlowTimeForQTE(float duration)
    {
        print("SlowingDownTime");
        if (slowTimeRoutine != null) StopCoroutine(slowTimeRoutine);
        if (returnTimeRoutine != null) { StopCoroutine(returnTimeRoutine); returnTimeRoutine = null; }
        slowTimeRoutine = StartCoroutine(SlowTime(duration));
    }
    public void ReturnTimeFromQTE()
    {
        print("ReturningTimeFromQTE");
        if (returnTimeRoutine != null) StopCoroutine(returnTimeRoutine);
        if (slowTimeRoutine != null) { StopCoroutine(slowTimeRoutine); slowTimeRoutine = null; }
        returnTimeRoutine = StartCoroutine(ReturnTime());
    }

    private IEnumerator SlowTime(float duration)
    {
        float timeElapsed = 0;

        print("SlowingDownTimeRoutine");

        while (Time.timeScale != 0.33f)
        {
            timeElapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1, 0.2f, timeElapsed / 0.1f);
            if (Time.timeScale < 0.2f) Time.timeScale = 0.2f;
            yield return null;
        }

        timeElapsed = 0;
        while (Time.timeScale != 0f)
        {
            timeElapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1, 0f, timeElapsed / duration - 0.1f);
            if (Time.timeScale < 0f) Time.timeScale = 0f;
            yield return null;
        }

        slowTimeRoutine = null;
    }
    private IEnumerator ReturnTime()
    {
        print("ReturningTIme");

        float timeElapsed = 0;
        float currentTimeScale = Time.timeScale;

        while (Time.timeScale < 1f)
        {
            print("ReturningTImeRoutineLOOOP TimeElapsed: " + timeElapsed);
            timeElapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(currentTimeScale, 1f, timeElapsed / 0.25f);
            if (Time.timeScale > 1f) Time.timeScale = 1f;
            yield return null;
        }

        returnTimeRoutine = null;
    }
}
