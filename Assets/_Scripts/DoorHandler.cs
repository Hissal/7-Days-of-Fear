using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    [SerializeField] Camera cam;
    Transform doorToBeSelected = null;
    Transform selectedDoor;
    HingeJoint joint = null;
    GameObject dragPointGameobject;
    int leftDoor = 0;
    [SerializeField] LayerMask doorLayer;

    void Update()
    {
        //Raycast
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3, doorLayer) && !selectedDoor)
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

        if (Input.GetMouseButtonDown(0) && doorToBeSelected)
        {
            selectedDoor = doorToBeSelected;
            selectedDoor.GetComponent<DoorOpener>().PlayerMovingDoor();
        }

        if (selectedDoor != null)
        {
            //HingeJoint joint = selectedDoor.GetComponent<HingeJoint>();
            JointMotor motor = joint.motor;

            //Create drag point object for reference where players mouse is pointing
            if (dragPointGameobject == null)
            {
                dragPointGameobject = new GameObject("Ray door");
                dragPointGameobject.transform.parent = selectedDoor;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            dragPointGameobject.transform.position = ray.GetPoint(Vector3.Distance(selectedDoor.position, transform.position));
            dragPointGameobject.transform.rotation = selectedDoor.rotation;


            float delta = Mathf.Pow(Vector3.Distance(dragPointGameobject.transform.position, selectedDoor.position), 3);

            //Deciding if it is left or right door
            if (selectedDoor.GetComponentInChildren<MeshRenderer>().localBounds.center.x > selectedDoor.localPosition.x)
            {
                leftDoor = 1;
            }
            else
            {
                leftDoor = -1;
            }

            //Applying velocity to door motor
            float speedMultiplier = 60000;

            print(selectedDoor.parent.forward.z + " leftdoor: " + leftDoor);

            if (selectedDoor.parent.forward.z > 0.5f)
            {
                if (dragPointGameobject.transform.position.x > selectedDoor.position.x)
                {
                    motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
                }
                else
                {
                    motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
                }
            }
            else if (selectedDoor.parent.forward.z < -0.5f)
            {
                if (dragPointGameobject.transform.position.x > selectedDoor.position.x)
                {
                    motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
                }
                else
                {
                    motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
                }
            }
            else if (selectedDoor.parent.transform.forward.x > 0.5f)
            {
                if (dragPointGameobject.transform.position.z > selectedDoor.position.z)
                {
                    motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
                }
                else
                {
                    motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
                }
            }
            else if (selectedDoor.parent.transform.forward.x < -0.5f)
            {
                if (dragPointGameobject.transform.position.z > selectedDoor.position.z)
                {
                    motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
                }
                else
                {
                    motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
                }
            }
            joint.motor = motor;

            if (Input.GetMouseButtonUp(0))
            {
                selectedDoor.GetComponent<DoorOpener>().PlayerNoLongerMovingDoor();

                doorToBeSelected = null;
                selectedDoor = null;
                motor.targetVelocity = 0;
                joint.motor = motor;
                joint = null;
                Destroy(dragPointGameobject);
                Reticle.UnFocus_Static();
            }
        }
    }
}
