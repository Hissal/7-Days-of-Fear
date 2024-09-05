using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    // TODO Cinematic Chaining
    // Play one cinematic at the end of another seamlessly

    [field: SerializeField] public Camera cinematicCamera { get; private set; }

    public void PlayCinematic(Cinematic cinematic)
    {
        GameManager.Instance.StunEnemy(-1);
        GameManager.Instance.TakeAwayPlayerControl();
        if(cinematic.isFirstCinematicOfChain && cinematic.requireStartpositions) MoveAndRotatePlayerToStartOfCinematic(cinematic);
        else
        {
            cinematic.gameObject.SetActive(true);
            cinematic.PlayCinematic(GameManager.Instance.playerController.playerCamera, cinematicCamera);
        }
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

        while (timeElapsed < time)
        {
            float precentageComplete = timeElapsed / time;
            precentageComplete = Mathf.Clamp(precentageComplete, 0f, 100f);

            playerT.position = Vector3.Lerp(startPosition, desiredPosition, precentageComplete);
            playerT.rotation = Quaternion.Lerp(startRotaion, desiredRotation, precentageComplete);
            cameraT.localRotation = Quaternion.Lerp(cameraStartRotation, cameraDesiredRotation, precentageComplete);
    
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        print("DoneMovingPLayer");
        cinematic.gameObject.SetActive(true);
        cinematic.PlayCinematic(GameManager.Instance.playerController.playerCamera, cinematicCamera);
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
