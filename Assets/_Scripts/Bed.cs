using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField] private float mentalHealthOnDay1;
    [SerializeField] private float mentalHealthOnDay2;
    [SerializeField] private float mentalHealthOnDay3;
    [SerializeField] private float mentalHealthOnDay4;
    [SerializeField] private float mentalHealthOnDay5;
    [SerializeField] private float mentalHealthOnDay6;
    [SerializeField] private float mentalHealthOnDay7;

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

    public override void OnInteract()
    {
        if (active)
        {
            base.OnInteract();
            GoToSleep();
        }
    }

    private void GoToSleep()
    {
        questObjective.OnComplete();
        GameManager.Instance.TakeAwayPlayerControl();
        MentalHealth.Instance.PauseDrainage();
        StartCoroutine(SleepRoutine());
        Reticle.HideReticle_Static();
        HUD.SetActive(false);
    }

    private IEnumerator SleepRoutine()
    {
        // Enable the day change screen
        dayChangeScreen.SetActive(true);

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

        // Fade out the fade image
        float fadeOutDuration = 2f;
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

        WakeUp();
    }

    private void WakeUp()
    {
        Reticle.ShowReticle_Static();
        HUD.SetActive(true);
        GameManager.Instance.GivePlayerControlBack();
        MentalHealth.Instance.ResumeDrainage();
        TimeManager.SetTime(TimeManager.day + 1, 6, 30);
        TimeManager.OnMorningInvoke();

        switch (TimeManager.day)
        {
            case 1:
                MentalHealth.Instance.SetMentalHealth(mentalHealthOnDay1);
                break;
            case 2:
                MentalHealth.Instance.SetMentalHealth(mentalHealthOnDay2);
                break;
            case 3:
                MentalHealth.Instance.SetMentalHealth(mentalHealthOnDay3);
                break;
            case 4:
                MentalHealth.Instance.SetMentalHealth(mentalHealthOnDay4);
                break;
            case 5:
                MentalHealth.Instance.SetMentalHealth(mentalHealthOnDay5);
                break;
            case 6:
                MentalHealth.Instance.SetMentalHealth(mentalHealthOnDay6);
                break;
            case 7:
                MentalHealth.Instance.SetMentalHealth(mentalHealthOnDay7);
                break;
            default:
                break;
        }
    }
}
