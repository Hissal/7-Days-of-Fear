using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float doorMoveSpeed;
    [SerializeField] private float grabDistance = 3f;
    private Transform doorToBeSelected;
    private Transform selectedDoor;
    private HingeJoint joint;
    private GameObject dragPointGameobject;
    private int leftDoor = 0;
    [SerializeField] private LayerMask doorLayer;

    private bool openingDoor = false;
    private bool closingDoor = false;

    private void Start()
    {
        doorToBeSelected = null;
        selectedDoor = null;
        joint = null;
    }

    private void PlayOpenSound()
    {
        if (selectedDoor)
        {
            AudioClip audioClip = selectedDoor.GetComponent<DoorOpener>().doorOpenSound;
            AudioManager.Instance.PlayAudioClip(audioClip, selectedDoor.position, 0.2f);
        }
    }
    private void PlayCloseSound()
    {
        if (selectedDoor)
        {
            AudioClip audioClip = selectedDoor.GetComponent<DoorOpener>().doorCloseSound;
            AudioManager.Instance.PlayAudioClip(audioClip, selectedDoor.position, 0.2f);
        }
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, grabDistance) && !selectedDoor)
        {
            if (((1 << hit.collider.gameObject.layer) & doorLayer.value) != 0)
            {
                if (doorToBeSelected == null || hit.collider.gameObject.GetInstanceID() != doorToBeSelected.gameObject.GetInstanceID())
                {
                    hit.collider.TryGetComponent(out joint);

                    if (joint)
                    {
                        Reticle.Focus_Static();
                        doorToBeSelected = joint.transform;
                    }
                }
            }
            else if (doorToBeSelected && !selectedDoor)
            {
                Reticle.UnFocus_Static();
                doorToBeSelected = null;
                joint = null;
            }
        }
        else if (doorToBeSelected && !selectedDoor)
        {
            Reticle.UnFocus_Static();
            doorToBeSelected = null;
            joint = null;
        }

        if (Input.GetMouseButtonDown(0) && doorToBeSelected)
        {
            Reticle.Grab_Static();
            selectedDoor = doorToBeSelected;
            DoorOpener doorOpener = selectedDoor.GetComponent<DoorOpener>();

            doorOpener.PlayerMovingDoor();

            //print(Mathf.Approximately(doorOpener.GetOpenPrecentage(), 0f));

            if (Mathf.Approximately(doorOpener.GetOpenPrecentage(), 0f))
            {
                openingDoor = true;
            }
            else
            {
                closingDoor = true;
            }
        }

        if (selectedDoor != null)
        {
            JointMotor motor = joint.motor;

            //Create drag point object for reference where players mouse is pointing
            if (dragPointGameobject == null)
            {
                dragPointGameobject = new GameObject("Ray door");
                dragPointGameobject.transform.parent = selectedDoor.parent;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            dragPointGameobject.transform.position = ray.GetPoint(Vector3.Distance(selectedDoor.parent.position, transform.position));

            float delta = Mathf.Pow(Vector3.Distance(dragPointGameobject.transform.position, selectedDoor.position), 3);

            //Deciding if it is left or right door
            MeshRenderer doorRenderer = selectedDoor.GetComponentInChildren<MeshRenderer>();

            if (doorRenderer.localBounds.center.x > selectedDoor.localPosition.x)
            {
                leftDoor = 1;
            }
            else
            {
                leftDoor = -1;
            }

            // Applying velocity to door motor

            Vector3 doorCenter = doorRenderer.bounds.center;
            Vector2 doorFacing = Vector2XZFromVector3(selectedDoor.parent.transform.forward);
            Vector2 playerDirectionFromDoor = (Vector2XZFromVector3(transform.position) - Vector2XZFromVector3(selectedDoor.position)).normalized;
            float dotProductOfPlayerPositionAndDoor = Vector2.Dot(doorFacing, playerDirectionFromDoor);

            float playerDistanceToDoor = Vector2.Distance(Vector2XZFromVector3(transform.position), Vector2XZFromVector3(selectedDoor.parent.transform.position));
            float dragPointDistanceToDoor = Vector2.Distance(Vector2XZFromVector3(dragPointGameobject.transform.position), Vector2XZFromVector3(selectedDoor.parent.transform.position));

            //print($"DistX: {xDist} DistZ: {zDist} forward: {selectedDoor.parent.forward} DotProduct: {Vector2.Dot(doorFacing, playerDirectionFromDoor)}");

            if (playerDistanceToDoor < 0.2f)
            {
                UseXAxis();
            }
            else
            {
                if (Mathf.Abs(dotProductOfPlayerPositionAndDoor) > 0.65f)
                {
                    UseXAxis();
                }
                else
                {
                    UseZAxis();
                }
            }

            DoorOpener doorOpener = selectedDoor.GetComponent<DoorOpener>();
            if (doorOpener != null)
            {
                if (Mathf.Approximately(doorOpener.GetOpenPrecentage(), 0f) && closingDoor)
                {
                    PlayCloseSound();
                    closingDoor = false;
                    openingDoor = true;
                }
                else if (!Mathf.Approximately(doorOpener.GetOpenPrecentage(), 0f) && openingDoor)
                {
                    PlayOpenSound();
                    openingDoor = false;
                    closingDoor = true;
                }
            }

            joint.motor = motor;

            if (Input.GetMouseButtonUp(0) || playerDistanceToDoor > 4f || dragPointDistanceToDoor > 4)
            {
                ReleaseDoor(motor);
            }

            void UseXAxis()
            {
                Vector3 dragPointDoorPosition = selectedDoor.parent.InverseTransformPoint(dragPointGameobject.transform.position);
                Vector3 doorCenterPosition = selectedDoor.parent.InverseTransformPoint(doorCenter);

                float xDist = Mathf.Abs(dragPointDoorPosition.x - doorCenterPosition.x);

                motor.targetVelocity = doorMoveSpeed * Time.deltaTime * leftDoor * xDist;

                if (joint.limits.max > 1f)
                {
                    if (dragPointDoorPosition.x > doorCenterPosition.x)
                    {
                        // Open Door
                        motor.targetVelocity = -doorMoveSpeed * Time.deltaTime * leftDoor * xDist;
                    }
                    else if (dragPointDoorPosition.x < doorCenterPosition.x)
                    {
                        // Close Door
                        motor.targetVelocity = doorMoveSpeed * Time.deltaTime * leftDoor * xDist;
                    }
                    else
                    {
                        motor.targetVelocity = 0f;
                    }
                }
                else
                {
                    if (dragPointDoorPosition.x > doorCenterPosition.x)
                    {
                        // Open Door
                        motor.targetVelocity = doorMoveSpeed * Time.deltaTime * leftDoor * xDist;
                    }
                    else if (dragPointDoorPosition.x < doorCenterPosition.x)
                    {
                        // Close Door
                        motor.targetVelocity = -doorMoveSpeed * Time.deltaTime * leftDoor * xDist;
                    }
                    else
                    {
                        motor.targetVelocity = 0f;
                    }
                }
            }

            void UseZAxis()
            {
                Vector3 dragPointDoorPosition = selectedDoor.parent.InverseTransformPoint(dragPointGameobject.transform.position);
                Vector3 doorCenterPosition = selectedDoor.parent.InverseTransformPoint(doorCenter);

                float zDist = Mathf.Abs(dragPointDoorPosition.z - doorCenterPosition.z);

                if (dragPointDoorPosition.z > doorCenterPosition.z)
                {
                    // Open Door
                    motor.targetVelocity = delta * -doorMoveSpeed * Time.deltaTime * leftDoor * zDist;
                }
                else if (dragPointDoorPosition.z < doorCenterPosition.z)
                {
                    // Close Door
                    motor.targetVelocity = delta * doorMoveSpeed * Time.deltaTime * leftDoor * zDist;
                }
                else
                {
                    motor.targetVelocity = 0f;
                }
            }
        }
    }

    private void ReleaseDoor(JointMotor motor)
    {
        DoorOpener doorOpener = selectedDoor.GetComponent<DoorOpener>();
        doorOpener.PlayerNoLongerMovingDoor();

        doorToBeSelected = null;
        selectedDoor = null;
        motor.targetVelocity = 0;
        joint.motor = motor;
        joint = null;
        Destroy(dragPointGameobject);
        Reticle.UnFocus_Static();
    }

    private Vector2 Vector2XZFromVector3(Vector3 vectorToChange)
    {
        return new Vector2(vectorToChange.x, vectorToChange.z);
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor) return;
        if (dragPointGameobject == null) return;

        Gizmos.DrawSphere(dragPointGameobject.transform.position, 0.1f);
    }
}
