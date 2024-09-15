using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Assets._Scripts.Managers_Systems;

public class Bed : Interactable
{
    [SerializeField] private QuestObjective questObjective;

    [SerializeField] private GameObject dayChangeScreen;
    [SerializeField] private RectTransform dayNumbersParent;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI[] dayNumberTexts;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject HUD;

    [SerializeField] Transform playerSpawnPoint;

    [SerializeField] private Transform playerOnBedTransform;
    [SerializeField] private Image blackFader;
    [SerializeField] private LightFlicker[] lightFlickers;

    [SerializeField] private LightSwitch lightSwitch;

    [SerializeField] private AudioClip cannotSleepSound;

    public UnityEvent OnSleepDay7;

    bool active = false;

    private void OnEnable()
    {
        questObjective.OnObjectiveActivated += ObjectiveActivated;
        questObjective.OnObjectiveHighlight += Highlight;
    }
    private void OnDisable()
    {
        questObjective.OnObjectiveActivated -= ObjectiveActivated;
        questObjective.OnObjectiveHighlight -= Highlight;
    }

    private void ObjectiveActivated(bool active)
    {
        this.active = active;
    }
    private void Highlight()
    {
        OnFocus();
    }

    public override void OnFocus()
    {
        if (active)
            base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        if (active)
            base.OnLoseFocus();
    }

    public override void OnInteract()
    {
        if (active && !GameManager.Instance.enemyActive)
        {
            base.OnInteract();
            GoToSleep();
            base.OnLoseFocus();
        }
        else if (GameManager.Instance.enemyActive)
        {
            AudioManager.Instance.PlayAudioClip(cannotSleepSound, transform.position, 0.1f);
        }
    }

    private void GoToSleep()
    {
        GameManager gameManager = GameManager.Instance;

        if (TimeManager.day == 7)
        {
            OnSleepDay7.Invoke();
            StartCoroutine(SpawnEnemyRoutine());
        }
        else
        {
            float newDaynumberParentPositionY = 450f - (TimeManager.day - 1) * 150f;
            dayNumbersParent.anchoredPosition = new Vector2(dayNumbersParent.anchoredPosition.x, newDaynumberParentPositionY);

            questObjective.OnComplete();
            gameManager.TakeAwayPlayerControl();
            MentalHealth.Instance.mentalHealthDrainagePauseManual = true;
            MentalHealth.Instance.PauseDrainage();
            StartCoroutine(SleepRoutine());
            Reticle.HideReticle_Static();
            HUD.SetActive(false);
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.TakeAwayPlayerControl();

        // Set the initial alpha value of the fade image and text to 0
        blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, 0f);

        Transform playerTransform = gameManager.playerTransform;

        // Fade in & out the fade image
        float fadeDuration = 2f;
        float fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
            blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, alpha);
            yield return null;
        }

        HUD.SetActive(false);

        //playerTransform.position = playerOnBedTransform.position;
        //playerTransform.rotation = playerOnBedTransform.rotation;
        //gameManager.playerController.SetCameraaRotationToZero();
        gameManager.playerController.SetCameraPositionAndRotation(playerOnBedTransform.position, playerOnBedTransform.rotation);

        yield return new WaitForSeconds(3f);

        foreach (var lightFlicker in lightFlickers)
        {
            lightFlicker.flicker = true;
        }

        fadeDuration = 0.25f;
        fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, alpha);
            yield return null;
        }
        blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, 0f);

        yield return new WaitForSeconds(0.25f);

        // Fade in & out the fade image
        fadeDuration = 2f;
        fadeTimer = 0f;
        bool flickerSet = false;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
            blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, alpha);

            if (flickerSet == false && fadeTimer > 1.5f)
            {
                flickerSet = true;
                foreach (var lightFlicker in lightFlickers)
                {
                    lightFlicker.flicker = false;
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        foreach (var lightFlicker in lightFlickers)
        {
            lightFlicker.flicker = true;
        }

        fadeDuration = 0.25f;
        fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, alpha);
            yield return null;
        }
        blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, 0f);

        yield return new WaitForSeconds(0.25f);

        fadeDuration = 1f;
        fadeTimer = 0f;
        flickerSet = false;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
            blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, alpha);

            if (flickerSet == false && fadeTimer > 0.75f)
            {
                flickerSet = true;
                foreach (var lightFlicker in lightFlickers)
                {
                    lightFlicker.flicker = false;
                }
            }

            yield return null;
        }

        HUD.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        //playerTransform.position = playerSpawnPoint.position;
        //playerTransform.rotation = playerSpawnPoint.rotation;
        gameManager.playerController.ResetCamera();
        gameManager.GivePlayerControlBack();

        gameManager.EnableEnemyLastEscape();

        questObjective.OnComplete();

        foreach (var lightFlicker in lightFlickers)
        {
            lightFlicker.flicker = true;
        }

        fadeDuration = 0.5f;
        fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, alpha);
            yield return null;
        }
        blackFader.color = new Color(blackFader.color.r, blackFader.color.g, blackFader.color.b, 0f);
    }

    private IEnumerator SleepRoutine()
    {
        // Enable the day change screen
        dayChangeScreen.SetActive(true);

        AmbienceController.Instance.FadeOutAmbience(3f);
        Radio.FadeOutRadio_Static(3f);

        // Set the initial alpha value of the fade image and text to 0
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
        dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, 0f);
        foreach (TextMeshProUGUI dayNumberText in dayNumberTexts)
        {
            dayNumberText.color = new Color(dayNumberText.color.r, dayNumberText.color.g, dayNumberText.color.b, 0f);
        }

        // Fade in the fade image
        float fadeDuration = 2f;
        float fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);

        yield return new WaitForSeconds(0.5f);

        // Fade in the day text and day number texts
        float textFadeDuration = 1f;
        float textFadeTimer = 0f;
        while (textFadeTimer < textFadeDuration)
        {
            textFadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, textFadeTimer / textFadeDuration);
            dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
            foreach (TextMeshProUGUI dayNumberText in dayNumberTexts)
            {
                dayNumberText.color = new Color(dayNumberText.color.r, dayNumberText.color.g, dayNumberText.color.b, alpha);
            }
            yield return null;
        }
        dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, 1f);
        foreach (TextMeshProUGUI dayNumberText in dayNumberTexts)
        {
            dayNumberText.color = new Color(dayNumberText.color.r, dayNumberText.color.g, dayNumberText.color.b, 1f);
        }

        // Slide down the day numbers parent
        Vector2 startPos = dayNumbersParent.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, startPos.y - 150f);
        float slideDuration = 1f;
        float slideTimer = 0f;
        while (slideTimer < slideDuration)
        {
            slideTimer += Time.deltaTime;
            float t = slideTimer / slideDuration;
            dayNumbersParent.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        dayNumbersParent.anchoredPosition = endPos;

        GameManager.Instance.playerTransform.position = playerSpawnPoint.position;
        GameManager.Instance.playerTransform.rotation = playerSpawnPoint.rotation;
        GameManager.Instance.playerController.SetCameraaRotationToZero();

        yield return new WaitForSeconds(1f);

        AmbienceController.Instance.SwitchAmbience(TimeManager.day + 1);
        AmbienceController.Instance.FadeInAmbience(3f);
        Radio.FadeInRadio_Static(3f);

        // Fade out the day text and day number texts
        float textFadeOutDuration = 1f;
        float textFadeOutTimer = 0f;
        while (textFadeOutTimer < textFadeOutDuration)
        {
            textFadeOutTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, textFadeOutTimer / textFadeOutDuration);
            dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
            foreach (TextMeshProUGUI dayNumberText in dayNumberTexts)
            {
                dayNumberText.color = new Color(dayNumberText.color.r, dayNumberText.color.g, dayNumberText.color.b, alpha);
            }
            yield return null;
        }
        dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, 0f);
        foreach (TextMeshProUGUI dayNumberText in dayNumberTexts)
        {
            dayNumberText.color = new Color(dayNumberText.color.r, dayNumberText.color.g, dayNumberText.color.b, 0f);
        }

        yield return new WaitForSeconds(0.5f);

        WakeUp();
        lightSwitch.TurnOnLights();

        // Fade out the fade image
        float fadeOutDuration = 0.5f;
        float fadeOutTimer = 0f;
        while (fadeOutTimer < fadeOutDuration)
        {
            fadeOutTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeOutTimer / fadeOutDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);

        // Disable the day change screen
        dayChangeScreen.SetActive(false);

        yield return null;
    }

    private void WakeUp()
    {
        HUD.SetActive(true);
        GameManager.Instance.GivePlayerControlBack();
        Reticle.ShowReticle_Static();
        MentalHealth.Instance.ResumeDrainage();
        MentalHealth.Instance.mentalHealthDrainagePauseManual = false;
        TimeManager.SetTime(TimeManager.day + 1, 6, 30, false, false);
        TimeManager.OnMorningInvoke();
    }
}
