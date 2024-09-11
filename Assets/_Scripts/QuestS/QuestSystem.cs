using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questTMP;
    private Quest currentQuest;
    private List<Quest> nextQuests = new List<Quest>();

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

    private void UpdateQuestText()
    {
        StartCoroutine(FadeOutAndMoveText(() =>
        {
            questTMP.text = currentQuest.Description;
            StartCoroutine(FadeInText());
        }));
    }

    private IEnumerator FadeOutAndMoveText(System.Action onComplete)
    {
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        Vector3 originalPosition = questTMP.rectTransform.position;
        Color originalColor = questTMP.color;

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
        Color originalColor = questTMP.color;
        Vector3 originalPosition = questTMP.rectTransform.position;

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
            questTMP.text = "";
        }
    }

    public void PauseCurrentQuest(string newQuestText)
    {
        if (newQuestText != "")
        {
            questTMP.text = newQuestText;
        }
        currentQuest.PauseQuest();
    }
    public void ResumeCurrentQuest()
    {
        currentQuest.ResumeQuest();
        questTMP.text += currentQuest.Description;
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
