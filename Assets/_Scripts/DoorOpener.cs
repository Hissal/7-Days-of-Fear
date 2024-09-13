using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class DoorOpener : MonoBehaviour
{
    [SerializeField] private HingeJoint joint;
    [SerializeField] private bool doorSideSwitch;
    [field: SerializeField] public bool isMovableByEnemy { get; private set; }
    [field: SerializeField] public bool isCloset { get; private set; }
    public bool moving { get; private set; }

    private bool playerMovingDoor;

    [field: SerializeField] public AudioClip doorOpenSound { get; private set; }
    [field: SerializeField] public AudioClip doorCloseSound { get; private set; }
    [SerializeField] private AudioClip[] creakSounds;
    [SerializeField] private AudioClip[] slamSounds;

    float creakedTimer = 0f;
    float creakCooldown = 10f;
    float creakCooldownRandom = 1f;

    float slamCooldown = 1f;
    float slamTimer = 0f;

    private void FixedUpdate()
    {
        if (creakedTimer > 0f) creakedTimer -= Time.fixedDeltaTime;
        if (slamTimer > 0f) slamTimer -= Time.fixedDeltaTime;
    }

    public float GetOpenPrecentage()
    {
        float angleMinAbs = Mathf.Abs(joint.limits.min);
        float angleMaxAbs = Mathf.Abs(joint.limits.max);

        float maxAngleAbs = 0f;

        if (angleMinAbs > angleMaxAbs) maxAngleAbs = angleMinAbs;
        else maxAngleAbs = angleMaxAbs;

        float precentage = Mathf.Clamp(joint.angle / maxAngleAbs, 0f, 1f);

        if (precentage < 0.02f)
        {
            precentage = 0f;
        }

        if (precentage > 0.98f)
        {
            precentage = 1f;
        }
        return precentage;
    }

    public void MoveDoor(float speed, float targetAngle, bool addTargetAngleToCurrent)
    {
        if (playerMovingDoor && speed < 10000f) return;

        if (creakedTimer <= 0f && speed < 500f)
        {
            AudioClip randomCreak = creakSounds[Random.Range(0, creakSounds.Length)];
            if (randomCreak) AudioManager.Instance.PlayAudioClip(randomCreak, transform.position, 0.2f);
            creakedTimer = creakCooldown + Random.Range(-creakCooldownRandom, creakCooldownRandom);
        }
        if (slamTimer <= 0f && speed > 10000f)
        {
            AudioClip randomSlam = slamSounds[Random.Range(0, slamSounds.Length)];
            if (randomSlam) AudioManager.Instance.PlayAudioClip(randomSlam, transform.position, 0.4f);
            slamTimer = slamCooldown;
        }

        playerMovingDoor = false;

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
