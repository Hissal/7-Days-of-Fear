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

    bool drawMirrorGizmo;
    Vector3 mirrorGizmoPos;

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
                dragPointGameobject.transform.parent = selectedDoor.parent;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            //Ray ray = cam.ScreenPointToRay(Vector3.zero);
            dragPointGameobject.transform.position = ray.GetPoint(Vector3.Distance(selectedDoor.parent.position, transform.position));
            //dragPointGameobject.transform.rotation = selectedDoor.rotation;


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

            //Applying velocity to door motor
            float speedMultiplier = 60000;

            //print(selectedDoor.parent.forward.z + " leftdoor: " + leftDoor);

            //float dragPointDistX = dragPointGameobject.transform.localPosition.x - selectedDoor.transform.localPosition.x;
            //float dragpointDistZ = dragPointGameobject.transform.localPosition.z - selectedDoor.transform.localPosition.z;
            //float dragPointObjectDelta = dragPointDistX + dragpointDistZ;

            //print($"DistX: {dragPointDistX} DistZ: {dragpointDistZ} Delta: {dragPointObjectDelta}");

            print(dragPointGameobject.transform.position);

            // if (selectedDoor.parent.forward.z > 0.5f)
            // {
            //     if (dragPointGameobject.transform.position.x > selectedDoor.position.x)
            //     {
            //         motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
            //     }
            //     else
            //     {
            //         motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
            //     }
            // }
            // else if (selectedDoor.parent.forward.z < -0.5f)
            // {
            //     if (dragPointGameobject.transform.position.x > selectedDoor.position.x)
            //     {
            //         motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
            //     }
            //     else
            //     {
            //         motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
            //     }
            // }
            // else if (selectedDoor.parent.transform.forward.x > 0.5f)
            // {
            //     if (dragPointGameobject.transform.position.z > selectedDoor.position.z)
            //     {
            //         motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
            //     }
            //     else
            //     {
            //         motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
            //     }
            // }
            // else if (selectedDoor.parent.transform.forward.x < -0.5f)
            // {
            //     if (dragPointGameobject.transform.position.z > selectedDoor.position.z)
            //     {
            //         motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
            //     }
            //     else
            //     {
            //         motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
            //     }
            // }

            Vector3 doorCenter = doorRenderer.bounds.center;

            float xDist = Mathf.Abs(dragPointGameobject.transform.position.x - doorCenter.x);
            float zDist = Mathf.Abs(dragPointGameobject.transform.position.z - doorCenter.z);
            float distFromDragObjToDoorCenter = Vector2.Distance(Vector2XZFromVector3(dragPointGameobject.transform.position), Vector2XZFromVector3(doorCenter));

            Vector2 doorFacing = Vector2XZFromVector3(selectedDoor.parent.transform.forward);
            Vector2 playerDirectionFromDoor = (Vector2XZFromVector3(transform.position) - Vector2XZFromVector3(selectedDoor.position)).normalized;
            float dotProduct = Vector2.Dot(doorFacing, playerDirectionFromDoor);

            print($"DistX: {xDist} DistZ: {zDist} forward: {selectedDoor.parent.forward} DotProduct: {Vector2.Dot(doorFacing, playerDirectionFromDoor)}");

            if (Mathf.Abs(dotProduct) > 0.5f)
            {
                UseXAxis();
            }
            else
            {
                UseZAxis();
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

            void UseXAxis()
            {
                drawMirrorGizmo = false;

                if (dragPointGameobject.transform.localPosition.x > 0.1f)
                {

                }

                if (xDist > zDist && xDist > 0.1f)
                {
                    if (selectedDoor.parent.forward.z > 0.5f)
                    {
                        if (dragPointGameobject.transform.position.x > doorCenter.x)
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.forward.z < -0.5f)
                    {
                        if (dragPointGameobject.transform.position.x > doorCenter.x)
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.transform.forward.x > 0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.transform.forward.x < -0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                }
                else if (zDist > xDist && zDist > 0.1f)
                {
                    if (selectedDoor.parent.forward.z > 0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.forward.z < -0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.transform.forward.x > 0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.transform.forward.x < -0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                }
                else
                {
                    motor.targetVelocity = 0f;
                }
            }

            void UseZAxis()
            {
                // Mirror X to other side

                drawMirrorGizmo = true;
                print("USINGZAXIS");
                float newX = -dragPointGameobject.transform.localPosition.x;
                mirrorGizmoPos = new Vector3(newX, dragPointGameobject.transform.localPosition.y, dragPointGameobject.transform.localPosition.z);
                mirrorGizmoPos = selectedDoor.parent.TransformPoint(mirrorGizmoPos);

                if (xDist > zDist && xDist > 0.1f)
                {
                    if (selectedDoor.parent.forward.z > 0.5f)
                    {
                        if (newX > doorCenter.x)
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.forward.z < -0.5f)
                    {
                        if (newX > doorCenter.x)
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.transform.forward.x > 0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.transform.forward.x < -0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                }
                else if (zDist > xDist && zDist > 0.1f)
                {
                    if (selectedDoor.parent.forward.z > 0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.forward.z < -0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.transform.forward.x > 0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                    else if (selectedDoor.parent.transform.forward.x < -0.5f)
                    {
                        if (dragPointGameobject.transform.position.z > doorCenter.z)
                        {
                            motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                        else
                        {
                            motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor * distFromDragObjToDoorCenter;
                        }
                    }
                }
                else
                {
                    motor.targetVelocity = 0f;
                }
            }
        }
    }

    private Vector2 Vector2XZFromVector3(Vector3 vectorToChange)
    {
        return new Vector2(vectorToChange.x, vectorToChange.z);
    }

    private void OnDrawGizmos()
    {
        if (dragPointGameobject == null) return;

        Gizmos.DrawSphere(dragPointGameobject.transform.position, 0.1f);

        if (drawMirrorGizmo)
        {
            Gizmos.DrawSphere(mirrorGizmoPos, 0.1f);
            print("DRAWINGMIRRORGIZMO");
        }
    }
}
