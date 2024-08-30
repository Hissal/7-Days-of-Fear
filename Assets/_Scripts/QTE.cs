using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTE : MonoBehaviour
{
    public event Action<QTE> OnSuccess = delegate { };
    public event Action<QTE> OnFail = delegate { };

    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI TMPro;

    public void Init(KeyCode keyCode, float time)
    {
        TMPro.text = keyCode.ToString();

        StartCoroutine(TimerRoutine(keyCode, time));
    }

    private IEnumerator TimerRoutine(KeyCode keyCode, float time)
    {
        yield return null;

        float timeLeft = time;

        while (timeLeft > 0)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Success();
                    yield break;
                }
                else
                {
                    Fail();
                    yield break;
                }
            }

            timeLeft -= Time.unscaledDeltaTime;
            timerImage.fillAmount = timeLeft / time;
            yield return null;
        }

        Fail();
    }

    public void Success()
    {
        OnSuccess.Invoke(this);
        RemoveSelf();
    }

    public void Fail()
    {
        OnFail.Invoke(this);
        RemoveSelf();
    }

    private void RemoveSelf()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
