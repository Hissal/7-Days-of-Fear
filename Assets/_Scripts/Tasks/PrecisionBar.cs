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
        active = true;

        base.Init();
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
            EndTask(OnSuccessPoint());
        }
    }

    protected override void DisableTaskAction()
    {
        parent.gameObject.SetActive(false);
    }

    private void EndTask(bool success)
    {
        StartCoroutine(EndTaskRoutine(success));
    }
    private IEnumerator EndTaskRoutine(bool success)
    {
        active = false;

        yield return new WaitForSeconds(0.2f);

        if (success)
        {
            TaskSuccess();
        }
        else
        {
            TaskFail();
        }
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
        float moveAmount = Screen.width / referenceResolutionWidth / (10 / pointerSpeed) * Time.deltaTime;

        pointer.position = new Vector2(pointer.position.x + moveAmount * pointerDirection, pointer.position.y);
        if (pointer.localPosition.x > barWidth * 0.5f || pointer.localPosition.x < -barWidth * 0.5f)
        {
            ChangePointerDirection();
        }
    }

    private void ChangePointerDirection()
    {
        pointerDirection *= -1;
    }

}
