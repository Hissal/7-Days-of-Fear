using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    // TODO Cinematic Chaining

    // Play one cinematic at the end of another seamlessly
    // Return to out of cinematic state only after no more cinematics in chain (Player controls and enemy showing and having control back)
    // nextCinematic variable serialized to unity

    // TODO QTES in cinematics

    // QTEManager
    // public QTE DisplayQTE(Vector2 positionOnScreen) <- Generates and displays a random qte on screen and returns a reference to it

    // display qte at certain point in timeline and subscribe to onfail / onsuccess
    // play next cinematic in chain depending on qte outcome
    // QTEFailedCinematic & QTESuccessCinematic serialized to unity

    public void PlayCinematic(Cinematic cinematic)
    {
        GameManager.Instance.StunEnemy(-1);
        GameManager.Instance.TakeAwayPlayerControl();
        MoveAndRotatePlayerToStartOfCinematic(cinematic);
    }

    private void MoveAndRotatePlayerToStartOfCinematic(Cinematic cinematic)
    {
        StartCoroutine(MoveAndRotatePlayer(cinematic));
    }
    private IEnumerator MoveAndRotatePlayer(Cinematic cinematic)
    {
        print("MovingPlayer");
        Vector3 desiredPosition = cinematic.playerStartPosition;
        Vector3 desiredRotationEuler = cinematic.playerStartRotation;
        Vector3 cameraDesiredRotationEuler = cinematic.cameraStartRotation;
        float time = cinematic.setupTime;

        Transform playerT = GameManager.Instance.playerTransform;
        Transform cameraT = GameManager.Instance.playerController.playerCamera.transform;
        Quaternion desiredRotation = Quaternion.Euler(desiredRotationEuler);
        Quaternion cameraDesiredRotation = Quaternion.Euler(cameraDesiredRotationEuler);

        Vector3 startPosition = playerT.position;
        Quaternion startRotaion = playerT.rotation;
        Quaternion cameraStartRotation = cameraT.localRotation;
        float timeElapsed = 0;

        while (playerT.position != desiredPosition || playerT.rotation != desiredRotation)
        {
            float precentageComplete = timeElapsed / time;

            playerT.position = Vector3.Lerp(startPosition, desiredPosition, precentageComplete);
            playerT.rotation = Quaternion.Lerp(startRotaion, desiredRotation, precentageComplete);
            cameraT.localRotation = Quaternion.Lerp(cameraStartRotation, cameraDesiredRotation, precentageComplete);
    
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        print("DoneMovingPLayer");
        cinematic.gameObject.SetActive(true);
        cinematic.PlayCinematic();
    }

    public static CinematicManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
