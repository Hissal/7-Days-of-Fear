using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrecisionBar : TaskAction
{
    [SerializeField] int barWidth;
    [SerializeField] private RectTransform pointer;
    [SerializeField] private RectTransform successPoint;
    [SerializeField] private GameObject parent;

    private int pointerDirection;

    private Rect successRect;

    private float referenceResolutionWidth;

    private bool active = false;

    private float pointerSpeed;
    [SerializeField] private float speedOnDay1;
    [SerializeField] private float speedOnDay2;
    [SerializeField] private float speedOnDay3;
    [SerializeField] private float speedOnDay4;
    [SerializeField] private float speedOnDay5;
    [SerializeField] private float speedOnDay6;
    [SerializeField] private float speedOnDay7;

    [Header("Audio")]
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;

    private CanvasScaler canvasScaler;


    private GameManager gameManager;

    private void OnEnable()
    {
        TimeManager.OnDayChanged += SetPointerSpeed;
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= SetPointerSpeed;
    }

    void Start()
    {
        referenceResolutionWidth = transform.parent.parent.GetComponent<CanvasScaler>().referenceResolution.x;
        canvasScaler = transform.parent.parent.GetComponent<CanvasScaler>();
        gameManager = GameManager.Instance;

        DisableTaskAction();
    }

    private void SetPointerSpeed(int day)
    {
        switch (day)
        {
            case 1:
                pointerSpeed = speedOnDay1;
                break;
            case 2:
                pointerSpeed = speedOnDay2;
                break;
            case 3:
                pointerSpeed = speedOnDay3;
                break;
            case 4:
                pointerSpeed = speedOnDay4;
                break;
            case 5:
                pointerSpeed = speedOnDay5;
                break;
            case 6:
                pointerSpeed = speedOnDay6;
                break;
            case 7:
                pointerSpeed = speedOnDay7;
                break;
            default:
                break;
        }
    }

    public override void Init()
    {
        parent.SetActive(true);
        SetRandomPointerPositionAndDirection();
        SetRandomSuccessPosition();
        StartCoroutine(StartTaskRoutine());

        base.Init();
    }

    private IEnumerator StartTaskRoutine()
    {
        yield return null;
        active = true;
    }

    private void SetRandomPointerPositionAndDirection()
    {
        int coinFlip = Random.Range(0, 2);
        if (coinFlip == 0) pointerDirection = 1;
        else pointerDirection = -1;

        float furthestPoint = barWidth / 2;
        float newPositionX = Random.Range(-furthestPoint, furthestPoint);
        Vector2 newPointerPosition = new Vector2(newPositionX, 0f);
        pointer.localPosition = newPointerPosition;

    }
    private void SetRandomSuccessPosition()
    {
        float furthestPoint = barWidth / 3;
        float newSuccessPointX = Random.Range(-furthestPoint, furthestPoint);
        Vector2 newSuccessPointPosition = new Vector2(newSuccessPointX, 0f);
        successPoint.localPosition = newSuccessPointPosition;
        successRect = successPoint.rect;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        MovePointer();

        if (Input.GetKeyDown(KeyCode.E))
        {
            print("Pressed Precision bar key");
            EndTask(OnSuccessPoint());
        }
    }

    protected override void DisableTaskAction()
    {
        parent.gameObject.SetActive(false);
    }

    private void EndTask(bool success)
    {
        if (success)
        {
            AudioManager.Instance.PlayAudioClip(successSound, gameManager.playerTransform.position, 0.05f);
        }
        else
        {
            AudioManager.Instance.PlayAudioClip(failSound, gameManager.playerTransform.position, 0.05f);
        }

        StartCoroutine(EndTaskRoutine(success));
    }
    private IEnumerator EndTaskRoutine(bool success)
    {
        active = false;

        if (success)
        {
            TaskSuccess();
        }
        else
        {
            TaskFail();
        }

        yield return new WaitForSeconds(0.2f);


        DisableTaskAction();
    }

    private bool OnSuccessPoint()
    {
        if (pointer.localPosition.x < successPoint.localPosition.x + successPoint.rect.width * 0.5f && pointer.localPosition.x > successPoint.localPosition.x - successPoint.rect.width * 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MovePointer()
    {
        // Use the scaleFactor to normalize the speed
        float normalizedSpeed = pointerSpeed / canvasScaler.scaleFactor;
        float moveAmount = normalizedSpeed * Time.deltaTime * pointerDirection;

        // Update the pointer position
        pointer.localPosition = new Vector2(pointer.localPosition.x + moveAmount, pointer.localPosition.y);

        // Check if the pointer has reached the edges of the bar and change direction if necessary
        if (pointer.localPosition.x > barWidth * 0.5f)
        {
            pointer.localPosition = new Vector2(barWidth * 0.5f, pointer.localPosition.y);
            ChangePointerDirection();
        }
        else if (pointer.localPosition.x < -barWidth * 0.5f)
        {
            pointer.localPosition = new Vector2(-barWidth * 0.5f, pointer.localPosition.y);
            ChangePointerDirection();
        }
    }

    private void ChangePointerDirection()
    {
        pointerDirection *= -1;
    }

}
