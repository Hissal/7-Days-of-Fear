using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathHoldingMinigame : MonoBehaviour
{
    [SerializeField] private KeyCode breathHoldingKey = KeyCode.Space;

    [SerializeField] private Image breathBar;
    [SerializeField] private Image backGround;

    [SerializeField] private float maxBreath;
    [SerializeField] private float breathDrainSpeed;
    [SerializeField] private float breathRegainSpeed;
    private float breathLeft;

    public bool holdingBreath { get; private set; }

    [SerializeField] private float timeBeforeBreathRecharge;
    private float breathRechargeTimer;

    private bool releasedBreathKey;

    [SerializeField] private Color highBreathColor;
    [SerializeField] private Color lowBreathColor;

    private void OnEnable()
    {
        breathLeft = maxBreath;
        releasedBreathKey = true;
        holdingBreath = false;
        UpdateVisual();
    }

    private void Update()
    {
        if (Input.GetKeyUp(breathHoldingKey))
        {
            releasedBreathKey = true;
        }

        if (Input.GetKey(breathHoldingKey) && breathLeft > 0 && releasedBreathKey)
        {
            DrainBreath();
            holdingBreath = true;
            breathRechargeTimer = timeBeforeBreathRecharge;

            return;
        }
        else if (breathRechargeTimer <= 0 && breathLeft < maxBreath)
        {
            RegainBreath();
        }
        else if (breathRechargeTimer > 0)
        {
            breathRechargeTimer -= Time.deltaTime;
        }

        holdingBreath = false;
    }

    private void DrainBreath()
    {
        float breathPrecentage = breathLeft / maxBreath;

        if (breathPrecentage > 0.66f)
        {
            breathLeft -= breathDrainSpeed * 1.2f * Time.deltaTime;
        }
        else if (breathPrecentage > 0.33f)
        {
            breathLeft -= breathDrainSpeed * Time.deltaTime;
        }
        else
        {
            breathLeft -= breathDrainSpeed * 0.8f * Time.deltaTime;
        }

        if (breathLeft < 0)
        {
            releasedBreathKey = false;
            breathLeft = 0;
        }

        UpdateVisual();
    }

    private void RegainBreath()
    {
        breathLeft += breathRegainSpeed * Time.deltaTime;

        if (breathLeft > maxBreath)
        {
            breathLeft = maxBreath;
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        breathBar.fillAmount = breathLeft / maxBreath;

        Color highBreathTransparent = highBreathColor;
        highBreathTransparent.a = 0.2f;
        Color lowBreathTransparent = lowBreathColor;
        lowBreathTransparent.a = 0.2f;

        breathBar.color = Color.Lerp(lowBreathColor, highBreathColor, breathBar.fillAmount);
        backGround.color = Color.Lerp(lowBreathTransparent, highBreathTransparent, breathBar.fillAmount);
    }
}
