using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathHoldingMinigame : MonoBehaviour
{
    [SerializeField] private KeyCode breathHoldingKey = KeyCode.Space;

    [SerializeField] private Image breathBar;

    [SerializeField] private float maxBreath;
    [SerializeField] private float breathDrainSpeed;
    [SerializeField] private float breathRegainSpeed;
    private float breathLeft;

    public bool holdingBreath { get; private set; }

    [SerializeField] private float timeBeforeBreathRecharge;
    private float breathRechargeTimer;

    private bool releasedBreathKey;

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
        breathLeft -= breathDrainSpeed * Time.deltaTime;

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
    }
}
