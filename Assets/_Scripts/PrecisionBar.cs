using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PrecisionBar : TaskAction
{
    [SerializeField] int barWidth;
    [SerializeField] float pointerSpeed;
    private float currentMove;


    public RectTransform pointer;
    public GameObject green;
    Rect greenRect;
    public bool col;

    // Start is called before the first frame update
    void Start()
    {
        greenRect = green.transform.GetComponent<RectTransform>().rect;
        currentMove = pointerSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        MovePointer();

        if (pointer.localPosition.x < greenRect.x + greenRect.width && pointer.localPosition.x > greenRect.x - greenRect.width)
        {
            col = true;
        }
        else
        {
            col = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (col == true)
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
        pointer.position = new Vector2(pointer.position.x + currentMove, pointer.position.y);
        if (pointer.localPosition.x > barWidth * 0.5f || pointer.localPosition.x < -barWidth * 0.5f)
        {
            ChangePointerDirection();
        }
    }

    private void ChangePointerDirection()
    {
        currentMove *= -1;
    }
}
