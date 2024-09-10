using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class DoorOpener : MonoBehaviour
{
    // TODO Fix Closing (Not fully closing) (kinda done.. stops moving when 0.1f away from goal)
    // TODO Bool to mark door as closet door
    // TODO door open angle interaction with enemy (if in closet but door is open enemy can see player)

    [SerializeField] private HingeJoint joint;
    [SerializeField] private bool doorSideSwitch;
    [field: SerializeField] public bool isMovableByEnemy { get; private set; }
    [field: SerializeField] public bool isCloset { get; private set; }
    public bool moving { get; private set; }

    private bool playerMovingDoor;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            MoveDoor(1000f, 60f, false);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            MoveDoor(1000f, 20f, false);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            MoveDoor(1000f, 20f, true); ;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            MoveDoor(1000f, -20f, true); ;
        }
    }

    public float GetOpenPrecentage()
    {
        float angleMinAbs = Mathf.Abs(joint.limits.min);
        float angleMaxAbs = Mathf.Abs(joint.limits.max);

        float maxAngleAbs = 0f;

        if (angleMinAbs > angleMaxAbs) maxAngleAbs = angleMinAbs;
        else maxAngleAbs = angleMaxAbs;

        return Mathf.Abs(joint.angle) / maxAngleAbs;
    }

    public void MoveDoor(float speed, float targetAngle, bool addTargetAngleToCurrent)
    {
        bool open;

        if (addTargetAngleToCurrent)
        {
            targetAngle += Mathf.Abs(joint.angle);
        }

        if (Mathf.Abs(joint.angle) < targetAngle)
        {
            open = true;
        }
        else
        {
            open = false;
        }

        StopAllCoroutines();
        StartCoroutine(DoorMovingRoutine(speed, targetAngle, open));
    }

    private IEnumerator DoorMovingRoutine(float speed, float targetAngle, bool open)
    {
        moving = true;

        if (open)
        {
            if (targetAngle > joint.limits.max  && joint.limits.min == 0) targetAngle = joint.limits.max;
            else if (-targetAngle < joint.limits.min && joint.limits.max == 0) targetAngle = -joint.limits.min;
        }
        else
        {
            if (targetAngle <= 0f) targetAngle = 0f;
        }

        //print("Moving Door Target Angle = " + targetAngle);

        JointMotor motor = joint.motor;

        if (open)
        {
            while (Mathf.Abs(joint.angle) < targetAngle - 0.1f && !playerMovingDoor)
            {
                if (doorSideSwitch)
                {
                    motor.targetVelocity = -speed * Time.deltaTime;

                    //print("1 OpeningDoor " + motor.targetVelocity + " Angle = " + joint.angle + " Max = " + joint.limits.max + " Target = " + targetAngle);

                    joint.motor = motor;
                }
                else
                {
                    motor.targetVelocity = speed * Time.deltaTime;

                    //print("2 OpeningDoor " + motor.targetVelocity + " Angle = " + joint.angle + " Max = " + joint.limits.max + " Target = " + targetAngle);

                    joint.motor = motor;
                }

                yield return null;
            }
        }
        else
        {
            while (Mathf.Abs(joint.angle) > targetAngle + 0.1f && !playerMovingDoor)
            {
                if (doorSideSwitch)
                {
                    motor.targetVelocity = speed * Time.deltaTime;

                    //print("1 ClosingDoor " + motor.targetVelocity + " Angle = " + joint.angle + " Min = " + joint.limits.min + " Target = " + targetAngle);

                    joint.motor = motor;
                }
                else
                {
                    motor.targetVelocity = -speed * Time.deltaTime;

                    //print("2 ClosingDoor " + motor.targetVelocity + " Angle = " + joint.angle + " Min = " + joint.limits.min + " Target = " + targetAngle);

                    joint.motor = motor;
                }
                yield return null;
            }
        }

        //print("Door Finished Opening");

        motor.targetVelocity = 0f;
        joint.motor = motor;

        moving = false;
    }

    public void PlayerMovingDoor()
    {
        playerMovingDoor = true;
    }

    public void PlayerNoLongerMovingDoor()
    {
        playerMovingDoor = false;
    }
}
