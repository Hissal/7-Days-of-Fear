using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public Camera playerCamera;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 1f;
    private bool moving;

    [Space]

    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float timeBeforeStaminaRecharge = 2f;
    [SerializeField] private float staminaDrainSpeed = 20f;
    [SerializeField] private float staminaRegainSpeed = 33f;

    private float currentStamina;
    private bool releasedSprintKey = true;
    private bool sprinting = false;
    private float staminaRegainTimer = 0f;

    [Space]

    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;

    [Space]

    [SerializeField] private float gravity = 10f;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footStepsAudioSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float FootstepFrequency = 0.5f;
    [SerializeField] private float footstepRandomization = 0.1f;
    [SerializeField] private float footstepVolume = 0.5f;
    [SerializeField] private float footstepPitch = 1f;
    private float footstepTimer;

    [Header("Heartbeat")]
    [SerializeField] private AudioSource heartBeatAudioSource;

    //[SerializeField] private float jumpPower = 7f;

    Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public float rotationX = 0;

    public bool canMove { get; private set; } = true;

    CharacterController characterController;

    private Transform enemyTransform;


    private Vector3 defaultCameraLocalPosition;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        enemyTransform = GameManager.Instance.enemyAI.transform;

        currentStamina = maxStamina;
        characterController = GetComponent<CharacterController>();

        defaultCameraLocalPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        if (GameManager.Instance.paused) return;

        //float distanceToEnemy = Vector3.Distance(transform.position, enemyTransform.position);
        //float maxDistance = 3f;
        //float minDistance = 1f;

        //if (distanceToEnemy < maxDistance)
        //{
        //    float volume = Mathf.Lerp(0f, 0.5f, (distanceToEnemy - minDistance) / (maxDistance - minDistance));
        //    heartBeatAudioSource.volume = volume;
        //}
        //else
        //{
        //    heartBeatAudioSource.volume = 0f;
        //}

        #region Handles Movment

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        HandleSprinting();

        float curSpeedX = canMove ? (sprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (sprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (curSpeedX != 0 || curSpeedY != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        #endregion

        #region Handles Gravity

        moveDirection.y = movementDirectionY;

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = 0;
        }

        #endregion

        #region Handles Rotation
        if (canMove) characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

            //print($"RotationX: {rotationX}, RotationY: {Input.GetAxis("Mouse X") * lookSpeed}");
        }

        #endregion

        #region Handles Sounds

        if (footstepTimer >= FootstepFrequency && moving && characterController.isGrounded)
        {
            footstepTimer = 0f + Random.Range(-footstepRandomization, footstepRandomization);
            PlayRandomFootstep();
        }
        else
        {
            if (sprinting) footstepTimer += Time.deltaTime * (1f + (sprintSpeed - walkSpeed));
            else footstepTimer += Time.deltaTime;
        }

        #endregion
    }

    public void SetCameraaRotationToZero()
    {
        rotationX = 0;
        playerCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public void SetCameraPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        playerCamera.transform.position = position;
        playerCamera.transform.rotation = rotation;
    }
    public void ResetCamera()
    {
        playerCamera.transform.localPosition = defaultCameraLocalPosition;
        SetCameraaRotationToZero();
    }

    private void HandleSprinting()
    {
        // Press Left Shift to run
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            releasedSprintKey = true;
        }

        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && releasedSprintKey)
        {
            DrainStamina(staminaDrainSpeed);
            sprinting = true;
            staminaRegainTimer = timeBeforeStaminaRecharge;

            return;
        }
        else if (staminaRegainTimer <= 0 && currentStamina < maxStamina)
        {
            RestoreStamina(staminaRegainSpeed);
        }
        else if (staminaRegainTimer > 0)
        {
            staminaRegainTimer -= Time.deltaTime;
        }

        sprinting = false;
    }

    private void PlayRandomFootstep()
    {
        AudioClip footstep = null;

        if (footstepSounds.Length > 0)
        {
            footstep = footstepSounds[Random.Range(0, footstepSounds.Length)];
        }

        if (footstep != null)
        {
            footStepsAudioSource.volume = footstepVolume;
            footStepsAudioSource.pitch = footstepPitch;
            footStepsAudioSource.PlayOneShot(footstep);
        }
    }

    private void DrainStamina(float amount)
    {
        currentStamina -= amount * Time.deltaTime;

        if (currentStamina < 0)
        {
            releasedSprintKey = false;
            currentStamina = 0;
        }
    }
    private void RestoreStamina(float amount)
    {
        currentStamina += amount * Time.deltaTime;

        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
    }

    public void Stun()
    {
        canMove = false;
    }
    public void Unstun()
    {
        canMove = true;
    }
}
