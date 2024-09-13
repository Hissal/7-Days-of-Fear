using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BreathHoldingMinigame : MonoBehaviour
{
    [SerializeField] private GameObject spaceBarIndicator;
    [SerializeField] private KeyCode breathHoldingKey = KeyCode.Space;

    [SerializeField] private Image breathBar;
    [SerializeField] private Image backGround;

    [SerializeField] private float maxBreath;
    private float breathDrainSpeed;
    [SerializeField] private float breathRegainSpeed;
    [SerializeField] private float breathDrainSpeedDay1 = 12;
    [SerializeField] private float breathDrainSpeedDay2 = 12;
    [SerializeField] private float breathDrainSpeedDay3 = 12;
    [SerializeField] private float breathDrainSpeedDay4 = 14;
    [SerializeField] private float breathDrainSpeedDay5 = 15;
    [SerializeField] private float breathDrainSpeedDay6 = 17;
    [SerializeField] private float breathDrainSpeedDay7 = 18;

    private float breathLeft;

    public bool holdingBreath { get; private set; }

    [SerializeField] private float timeBeforeBreathRecharge;
    private float breathRechargeTimer;

    private bool releasedBreathKey;

    [SerializeField] private Color highBreathColor;
    [SerializeField] private Color lowBreathColor;

    [Header("Audio")]
    [SerializeField] private AudioClip inhaleSound;
    [SerializeField] private AudioClip exhaleSound;

    private Transform playerTransform;

    public bool active { get; private set; }

    private void Awake()
    {
        TimeManager.OnDayChanged += SetBreathDrainSpeed;
    }
    private void OnDestroy()
    {
        TimeManager.OnDayChanged -= SetBreathDrainSpeed;
    }

    private void Start()
    {
        playerTransform = GameManager.Instance.playerTransform;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        active = true;
        breathLeft = maxBreath;
        releasedBreathKey = true;
        holdingBreath = false;
        UpdateVisual();
    }

    private void OnDisable()
    {
        active = false;
    }

    private void SetBreathDrainSpeed(int day)
    {
        switch (day)
        {
            case 1:
                breathDrainSpeed = breathDrainSpeedDay1;
                break;
            case 2:
                breathDrainSpeed = breathDrainSpeedDay2;
                break;
            case 3:
                breathDrainSpeed = breathDrainSpeedDay3;
                break;
            case 4:
                breathDrainSpeed = breathDrainSpeedDay4;
                break;
            case 5:
                breathDrainSpeed = breathDrainSpeedDay5;
                break;
            case 6:
                breathDrainSpeed = breathDrainSpeedDay6;
                break;
            case 7:
                breathDrainSpeed = breathDrainSpeedDay7;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (!active) return;

        if (Input.GetKeyUp(breathHoldingKey))
        {
            releasedBreathKey = true;
        }

        if (Input.GetKey(breathHoldingKey) && breathLeft > 0 && releasedBreathKey)
        {
            if (holdingBreath == false) AudioManager.Instance.PlayAudioClip(inhaleSound, playerTransform.position, 0.2f);

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

        if (holdingBreath)
        {
            AudioManager.Instance.PlayAudioClip(exhaleSound, playerTransform.position, 0.2f);
            holdingBreath = false;
        }

        holdingBreath = false;
    }

    private void DrainBreath()
    {
        if (spaceBarIndicator.activeInHierarchy) spaceBarIndicator.SetActive(false);

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
