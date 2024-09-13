using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questTMP;
    private Quest currentQuest;
    private List<Quest> nextQuests = new List<Quest>();

    private Vector2 questHelperPosition;
    private Color questHelperColor;

    public bool paused { get; private set; }

    private void Start()
    {
        questHelperColor = questTMP.color;
        questHelperPosition = questTMP.rectTransform.position;
    }

    public void StartQuest(Quest quest)
    {
        if (currentQuest != null && !quest.overrideCurrentQuest)
        {
            print("Adding quest to nextQuests");
            nextQuests.Add(quest);
            return;
        }
        else if (currentQuest != null && quest.overrideCurrentQuest)
        {
            currentQuest.OnObjectiveChanged -= UpdateQuestText;
            currentQuest.OnQuestComplete -= QuestCompleted;
            currentQuest.EndQuest();
            currentQuest = null;
        }

        currentQuest = quest;
        currentQuest.OnObjectiveChanged += UpdateQuestText;
        currentQuest.OnQuestComplete += QuestCompleted;
        currentQuest.StartQuest();
    }

    private void UpdateQuestText(string newText)
    {
        if (paused) return;

        StopAllCoroutines();

        StartCoroutine(FadeOutAndMoveText(() =>
        {
            questTMP.text = newText;
            StartCoroutine(FadeInText());
        }));
    }

    private IEnumerator FadeOutAndMoveText(System.Action onComplete)
    {
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        Vector3 originalPosition = questHelperPosition;
        Color originalColor = questHelperColor;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            questTMP.color = newColor;

            float yOffset = Mathf.Lerp(0f, 20f, elapsedTime / fadeDuration);
            Vector3 newPosition = originalPosition + new Vector3(0f, yOffset, 0f);
            questTMP.rectTransform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        questTMP.color = originalColor;
        questTMP.rectTransform.position = originalPosition;

        onComplete?.Invoke();
    }

    private IEnumerator FadeInText()
    {
        float fadeDuration = 0.3f;
        float elapsedTime = 0f;
        Color originalColor = questHelperColor;
        Vector3 originalPosition = questHelperPosition;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            questTMP.color = newColor;

            float xOffset = Mathf.Lerp(100f, 0, elapsedTime / fadeDuration);
            Vector3 newPosition = originalPosition + new Vector3(xOffset, 0f, 0f);
            questTMP.rectTransform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        questTMP.rectTransform.position = originalPosition;
        questTMP.color = originalColor;
    }

    private void QuestCompleted()
    {
        currentQuest.OnObjectiveChanged -= UpdateQuestText;
        currentQuest.OnQuestComplete -= QuestCompleted;
        currentQuest = null;
        if (nextQuests.Count > 0)
        {
            StartQuest(nextQuests[0]);
            nextQuests.RemoveAt(0);
        }
        else
        {
            UpdateQuestText("");
        }
    }

    public void PauseCurrentQuest(string newQuestText)
    {
        if (newQuestText != "")
        {
            UpdateQuestText(newQuestText);
        }

        if (currentQuest == null) return;

        currentQuest.PauseQuest();
        paused = true;
    }
    public void ResumeCurrentQuest()
    {
        if (currentQuest == null)
        {
            questTMP.text = "";
            return; 
        }

        currentQuest.ResumeQuest();
        paused = false;
        UpdateQuestText(currentQuest.Description);
    }

    public static QuestSystem Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
