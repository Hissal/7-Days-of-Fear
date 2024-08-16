using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PrecisionBar : TaskAction
{
    [SerializeField] int barWidth;
    [SerializeField] private RectTransform pointer;
    [SerializeField] float pointerSpeed;
    [SerializeField] private GameObject green;

    private float pointerDirection;

    private Rect greenRect;
    private bool onGreen;

    private float referenceResolutionWidth;

    // Start is called before the first frame update
    void Start()
    {
        referenceResolutionWidth = transform.parent.GetComponent<CanvasScaler>().referenceResolution.x;

        greenRect = green.transform.GetComponent<RectTransform>().rect;
        pointerDirection = 1;
    }
    // Update is called once per frame
    void Update()
    {
        MovePointer();

        if (pointer.localPosition.x < greenRect.x + greenRect.width && pointer.localPosition.x > greenRect.x - greenRect.width)
        {
            onGreen = true;
        }
        else
        {
            onGreen = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (onGreen == true)
            {
                TaskSuccess();
            }
            else
            {
                TaskFail();
            }
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
