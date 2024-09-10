using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrecisionBar : TaskAction
{
    [SerializeField] int barWidth;
    [SerializeField] private RectTransform pointer;
    [SerializeField] float pointerSpeed;
    [SerializeField] private RectTransform successPoint;

    private int pointerDirection;

    private Rect successRect;

    private float referenceResolutionWidth;

    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        referenceResolutionWidth = transform.parent.GetComponent<CanvasScaler>().referenceResolution.x;

        DisableTaskAction();
    }

    public override void Init()
    {
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
        float moveAmount = referenceResolutionWidth / (10 / pointerSpeed) * Time.deltaTime;

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
