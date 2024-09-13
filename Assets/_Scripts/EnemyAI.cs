using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform RaycastPosition;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private Transform playerT;

    [Header("Sight")]
    [SerializeField] private float sightRange;
    [SerializeField] private float nearSightRange;
    [SerializeField] private float killRange;

    private Vector3 playerPositionSnapshot;
    private bool playerWasInSight;

    public enum EnemyState {Wandering, ChasingPlayer, PlayerHidingSequence, Stunned}
    public EnemyState State { get; private set; }

    [Header("Mental Health")]
    [SerializeField] private float[] mentalHealthToGoAwayArray;
    private float mentalHealthToGoAway = 25f;
    [SerializeField] private float mentalHealthIncreaseOnSuccesfulHide = 25f;
    [SerializeField] private float mentalHealthIncreasePerSecondWhenPlayerNotInSight = 0.1f;

    [Header("Layers")]
    [SerializeField] private LayerMask seenLayers;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask lightLayer;

    private List<LightFlicker> flickeringLights = new List<LightFlicker>();

    private bool listening;
    private HidingSpot playersCurrentHidingSpot;

    private bool killPlayer;

    public bool active { get; private set; }

    [Header("Footsteps")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float footstepFrequency = 0.5f;
    [SerializeField] private float footstepRandomization = 0.1f;
    [SerializeField] private float footstepVolume = 0.5f;
    [SerializeField] private float footstepPitch = 1f;
    private float footstepTimer;

    private bool moving;

    public bool cantDeactivate { get; private set; } = false;

    private void OnEnable()
    {
        MentalHealth.Instance.OnMentalHealthIncrease += MentalHealthIncreased;
        TimeManager.OnDayChanged += SetMentalHealthRequiredToGoAway;
    }

    private void SetMentalHealthRequiredToGoAway(int day)
    {
        switch (day)
        {
            case 1:
                mentalHealthToGoAway = mentalHealthToGoAwayArray[0];
                break;
            case 2:
                mentalHealthToGoAway = mentalHealthToGoAwayArray[1];
                break;
            case 3:
                mentalHealthToGoAway = mentalHealthToGoAwayArray[2];
                break;
            case 4:
                mentalHealthToGoAway = mentalHealthToGoAwayArray[3];
                break;
            case 5:
                mentalHealthToGoAway = mentalHealthToGoAwayArray[4];
                break;
            case 6:
                mentalHealthToGoAway = mentalHealthToGoAwayArray[5];
                break;
            case 7:
                mentalHealthToGoAway = mentalHealthToGoAwayArray[6];
                break;
            default:
                break;
        }
    }

    private void MentalHealthIncreased(float currentMentalHealth)
    {
        if (!active) return;
        if (cantDeactivate) return;
        if (!GameManager.Instance.enemyActive) return;

        if (currentMentalHealth >= mentalHealthToGoAway)
        {
            GameManager.Instance.DisableEnemy();
        }
    }

    private void Start()
    {
        active = false;
        playerT = GameManager.Instance.playerTransform;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }
    public void Activate(Vector3 position, Quaternion rotation, bool cantDeactivate)
    {
        this.cantDeactivate = cantDeactivate;
        if (active) return;
        agent.enabled = true;

        StopAllCoroutines();

        SetPosition(position);

        StartCoroutine(ActivateDelay());
    }
    IEnumerator ActivateDelay()
    {
        yield return new WaitForSeconds(0.1f);
        UnStun(false);
        SnapshotPlayerPosition();
        State = EnemyState.ChasingPlayer;
        StartCoroutine(MoveToPlayerSnapshot());
        active = true;
    }

    public void Disable(Vector3 position)
    {
        if (cantDeactivate) return;

        StopAllCoroutines();
        State = EnemyState.Stunned;
        SetPosition(position);
        StartCoroutine(Deactivate());
    }
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(0.1f);
        Stun(-1);
        agent.enabled = false;
        active = false;

        foreach (LightFlicker lightFlicker in flickeringLights)
        {
            UnFlickerLight(lightFlicker);
        }
    }

    public void SetPosition(Vector3 position)
    {
        agent.Warp(position);
    }

    private void Update()
    {
        if (!active) return;

        if (agent.isOnNavMesh == false)
        {
            throw new System.Exception("Enemy is not on navmesh " + gameObject);
        }

        if (!AgentReachedDestiantion()) moving = true;
        else moving = false;

        if (State == EnemyState.Stunned) moving = false;
        animator.SetBool("Moving", moving);

        HandleFootsteps();

        FlickerNearbyLights();

        OpenNearbyDoors();

        if (State != EnemyState.ChasingPlayer) MentalHealth.Instance.IncreaseMentalHealth(mentalHealthIncreasePerSecondWhenPlayerNotInSight * Time.deltaTime);

        if (State == EnemyState.PlayerHidingSequence || State == EnemyState.Stunned) return;

        if (PlayerInSight())
        {
            WalkTowardPlayer();
            State = EnemyState.ChasingPlayer;
            if (Vector2.Distance(Vector2XZFromVector3(transform.position), Vector2XZFromVector3(playerT.position)) < killRange && !killPlayer)
            {
                //print("Player In Kill Radius.. KILL PLAYER");
                KillPlayer();
            }
        }
        else if (playerWasInSight == true)
        {
            playerWasInSight = false;
            SnapshotPlayerPosition();
            StartCoroutine(MoveToPlayerSnapshot());
        }
    }

    private void HandleFootsteps()
    {
        if (footstepTimer >= footstepFrequency && moving)
        {
            footstepTimer = 0f + UnityEngine.Random.Range(-footstepRandomization, footstepRandomization);
            PlayRandomFootstep();
        }
        else
        {
            footstepTimer += Time.deltaTime;
        }
    }

    private void FlickerNearbyLights()
    {
        Collider[] lightColliders = Physics.OverlapSphere(transform.position, 2f, lightLayer);
        if (lightColliders.Length > 0)
        {
            foreach (var collider in lightColliders)
            {
                LightFlicker lightFlicker = collider.GetComponent<LightFlicker>();
                if (!flickeringLights.Contains(lightFlicker) && lightFlicker.light.intensity > 0f)
                {
                    flickeringLights.Add(lightFlicker);
                    lightFlicker.flicker = true;
                }
            }
        }

        if (flickeringLights.Count != 0)
        {
            LightFlicker flickerToRemove = null;

            foreach (var lightFlicker in flickeringLights)
            {
                bool contains = false;

                Collider col = lightFlicker.GetComponent<Collider>();

                foreach (var collider in lightColliders)
                {
                    if (collider == col)
                    {
                        contains = true;
                    }
                }

                if (!contains)
                {
                    flickerToRemove = lightFlicker;
                }
            }

            if (flickerToRemove != null)
            {
                UnFlickerLight(flickerToRemove);
            }
        }
    }

    private void UnFlickerLight(LightFlicker lightFlicker)
    {
        lightFlicker.flicker = false;
        lightFlicker.light.intensity = 0f;
        flickeringLights.Remove(lightFlicker);
    }

    private void OpenNearbyDoors()
    {
        List<DoorOpener> doorOpeners = GetDoorsInRadius(2f);
        foreach (var door in doorOpeners)
        {
            if (door.isMovableByEnemy && !door.isCloset && Vector2.Distance(Vector2XZFromVector3(transform.position), Vector2XZFromVector3(door.transform.position)) < 0.75f)
            {
                SlamDoorOpen(door);
            }
            else if (door.isMovableByEnemy && !door.isCloset && !door.moving) door.MoveDoor(UnityEngine.Random.Range(200f, 500f), UnityEngine.Random.Range(-10f, 10f), true);
        }
    }

    private IEnumerator MoveToPlayerSnapshot()
    {
        print("MovingToSnapshot");

        agent.SetDestination(playerPositionSnapshot);

        yield return null;
        yield return new WaitUntil(() => AgentReachedDestiantion() || PlayerInSight());

        if (!PlayerInSight())
        {
            print("Arrived At Snapshot Player Not In Sight Walking forward");
            Vector3 newPos = transform.position + transform.forward * 1f;
            agent.SetDestination(newPos);
            yield return null;

            yield return new WaitUntil(() => AgentReachedDestiantion() || PlayerInSight());
            print("Arrived at new position");

            if (!PlayerInSight()) StartCoroutine(WanderRandomly(1f));
        }
    }

    private IEnumerator WanderRandomly(float minDistance)
    {
        if (agent.enabled == false) yield break;

        State = EnemyState.Wandering;

        print("Wandering");

        agent.SetDestination(RandomNavmeshLocation(10f, minDistance, 5));

        yield return new WaitUntil(() => AgentReachedDestiantion() || State != EnemyState.Wandering);

        float secondsToWait = UnityEngine.Random.Range(0f, 5f);
        float timeElapsed = 0f;

        while (State == EnemyState.Wandering && timeElapsed < secondsToWait)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (State == EnemyState.Wandering)
        {
            StartCoroutine(WanderRandomly(1f));
        }
    }

    private bool PlayerInSight()
    {
        if (Physics.Raycast(RaycastPosition.position, (playerT.position - RaycastPosition.position).normalized, out RaycastHit hit, sightRange, seenLayers))
        {
            if (hit.collider.gameObject.layer == playerLayer)
            {
                //print("Player In Sight");
                playerWasInSight = true;
                return true;
            }else if (hit.transform == playerT)
            {
                //print("Player In Sight");
                playerWasInSight = true;
                return true;
            }
        }

        return false;
    }

    private void SnapshotPlayerPosition()
    {
        playerPositionSnapshot = playerT.position;
    }

    private void WalkTowardPlayer()
    {
        agent.SetDestination(playerT.position);
    }

    /// <summary>
    /// Stuns the nemy for given time, negative values stun until manually unstunned
    /// </summary>
    /// <param name="time"></param>
    public void Stun(float time)
    {
        StopAllCoroutines();
        if (agent.enabled == true) agent.isStopped = true;
        State = EnemyState.Stunned;
        if (time < 0) return;
        StartCoroutine(StunRoutine(time));
    }
    private IEnumerator StunRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        UnStun(true);
    }
    public void UnStun(bool wander)
    {
        if (agent.enabled == true) agent.isStopped = false;

        if (!wander) return;

        StopAllCoroutines();
        StartCoroutine(WanderRandomly(1f));
    }

    private void PlayerLeftHidingSpot(HidingSpot hidingSpot)
    {
        if (playersCurrentHidingSpot == hidingSpot)
        {
            playersCurrentHidingSpot = null;
        }
        hidingSpot.onPlayerExit -= PlayerLeftHidingSpot;
    }

    public void PlayerEnteredHidingSpot(Transform closetFront, HidingSpot hidingSpot)
    {
        playersCurrentHidingSpot = hidingSpot;

        float distanceToPlayer = Vector3.Distance(playerT.position, transform.position);

        if (distanceToPlayer > sightRange) return;

        print("Enemy Knows Player Entered Hiding Spot " + closetFront);

        hidingSpot.onPlayerExit += PlayerLeftHidingSpot;

        float range = hidingSpot.enemyRange;
        Vector3 locationToWalkTo = closetFront.position + closetFront.right * UnityEngine.Random.Range(-range, range);

        // Calculates a rotation to face between straight at closet and playerposition randomly
        //Vector3 targetPoint = playerT.position;
        //Vector3 directionToTarget = Vector2XZFromVector3(targetPoint) - Vector2XZFromVector3(locationToWalkTo);
        //directionToTarget.y = 0;
        //Quaternion lookAtTarget = Quaternion.LookRotation(directionToTarget);
        //Quaternion inverseRotation = Quaternion.Inverse(closetFront.rotation);
        //Quaternion desiredRotation = Quaternion.Lerp(lookAtTarget, inverseRotation, Random.Range(0f, 1f));

        Quaternion desiredRotation = Quaternion.Inverse(closetFront.rotation);

        // TODO Kill Player Upon Reaching The Hiding Spot
        if (distanceToPlayer <= nearSightRange && State == EnemyState.ChasingPlayer)
        {
            WalkToHidingSpot(locationToWalkTo, desiredRotation, true);
            print("Kill PLayer Upon Reaching The Hiding Spot");
        } 
        else if (distanceToPlayer <= sightRange && State == EnemyState.ChasingPlayer) WalkToHidingSpot(locationToWalkTo, desiredRotation, false);
    }
    private void WalkToHidingSpot(Vector3 locationToWalkTo, Quaternion desiredRotation, bool killPlayer)
    {
        print("Walking to hiding spot");

        StopAllCoroutines();
        StartCoroutine(WalkToHidingSpotSequence(locationToWalkTo, desiredRotation, killPlayer));
    }
    private IEnumerator WalkToHidingSpotSequence(Vector3 locationToWalkTo, Quaternion desiredRotation, bool killPlayer)
    {
        State = EnemyState.PlayerHidingSequence;

        playerWasInSight = false;
        agent.SetDestination(locationToWalkTo);

        yield return new WaitUntil(() => Vector2XZFromVector3(transform.position) == Vector2XZFromVector3(locationToWalkTo) || playersCurrentHidingSpot == null);

        if (playersCurrentHidingSpot == null)
        {
            print("HidingSpot is null");
            StartCoroutine(WanderRandomly(1f));
            yield break;
        }

        // TODO Stare into closet

        if (killPlayer)
        {
            KillPlayerInCloset();
            yield break;
        }

        print("Reached Hiding Spot... STARING");
        animator.SetTrigger("Stare");
        float timeStaring = 0f;

        int coinFlip = UnityEngine.Random.Range(0, 2);
        int coinFlip2 = UnityEngine.Random.Range(0, 2);
        bool stare2 = coinFlip == 1;
        bool stare3 = coinFlip2 == 1;
        bool doneRotating = false;
        Quaternion startRotation = transform.rotation;

        float timeToMoveDoor = UnityEngine.Random.Range(1f, 15f);
        bool movedDoor = false;
        float timeToMoveDoor2 = UnityEngine.Random.Range(1f, 15f);
        bool movedDoor2 = false;

        float stareSpeed1 = UnityEngine.Random.Range(0.66f, 1f);
        float stareSpeed2 = UnityEngine.Random.Range(0.66f, 1f);

        stareSpeed1 = 0.66f;

        animator.SetBool("Stare2", stare2);
        animator.SetBool("Stare3", stare3);

        yield return new WaitUntil( () => animator.GetCurrentAnimatorStateInfo(0).IsName("Stare"));

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stare"))
            {
                animator.speed = stareSpeed1;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stare2"))
            {
                animator.speed = stareSpeed2;
            }
            else
            {
                animator.speed = 1f;
            }

            // Stare lolxd     
            timeStaring += Time.deltaTime;

            if (!movedDoor && timeStaring > timeToMoveDoor)
            {
                RandomChanceOpenHidingSpotDoors(50);
                movedDoor = true;
            }
            if (!movedDoor2 && timeStaring > timeToMoveDoor2)
            {
                RandomChanceOpenHidingSpotDoors(25);
                movedDoor2 = true;
            }

            if (!doneRotating)
            {
                // Rotate to toward closet
                //print("Rotating to: " + desiredRotation.eulerAngles);
                float precentageDone = Mathf.Clamp(timeStaring, 0f, 1f);
                
                transform.rotation = Quaternion.Lerp(startRotation, desiredRotation, precentageDone);
                if (transform.rotation == desiredRotation) doneRotating = true;
            }

            if (GameManager.Instance.IsPlayerHoldingBreath() == false && listening && !killPlayer)
            {
                KillPlayerInCloset();
                print("Player failed to hold breath... Kill Player");
                yield break;
            }

            if (playersCurrentHidingSpot == null)
            {
                print("HidingSpot is null");
                continue;
            }
            foreach (var door in playersCurrentHidingSpot.hidingSpotDoors)
            {
                if (door.GetOpenPrecentage() > 0.4f && !killPlayer)
                {
                    KillPlayerInCloset();
                    print("DOOR WAY TOO OPEN KILL PLAYER");
                    yield break;
                }
            }
            yield return null;
        }

        print("Finished Staring... Walking Away");

        animator.speed = 1f;
        Vector3 randomLocation = RandomNavmeshLocation(10f, 5f, 10);
        agent.SetDestination(randomLocation);

        while (Vector2XZFromVector3(transform.position) != Vector2XZFromVector3(randomLocation))
        {
            if (PlayerInSight())
            {
                print("Enemy spotted player while walking away from hiding spot");
                State = EnemyState.ChasingPlayer;
                yield break;
            }
            yield return null;
        }

        print("Reached a location away from hiding spot");

        if (MentalHealth.Instance.IncreaseMentalHealth(mentalHealthIncreaseOnSuccesfulHide) > mentalHealthToGoAway)
        yield break;

        StartCoroutine(WanderRandomly(1f));
    }

    private void KillPlayerInCloset()
    {
        if (playersCurrentHidingSpot == null)
        {
            print("HidingSpot is null");
            return;
        }
        foreach (var door in playersCurrentHidingSpot.hidingSpotDoors)
        {
            SlamDoorOpen(door);
        }

        KillPlayer();
    }

    private void KillPlayer()
    {
        killPlayer = true;
        GameManager.Instance.KillPlayer();

        // TODO Play Death Sequence
    }

    private void SlamDoorOpen(DoorOpener door)
    {
        if (door.GetOpenPrecentage() > 0.95f) return;
        door.MoveDoor(50000f, 180f, false);
        //animator.SetTrigger("Attack");
    }

    public void RandomChanceOpenHidingSpotDoors(int chance)
    {
        int openDoorChance = chance;
        int openDoorRoll = UnityEngine.Random.Range(0, 100);
        bool openDoorSligtly = openDoorRoll < openDoorChance;

        if (!openDoorSligtly) return;

        print("Moving Current Hiding Spot Doors");

        if (playersCurrentHidingSpot == null)
        {
            print("HidingSpot is null");
            return;
        }

        foreach (var door in playersCurrentHidingSpot.hidingSpotDoors)
        {
            door.MoveDoor(UnityEngine.Random.Range(100f, 300f), UnityEngine.Random.Range(5f, 20f), true);
        }
    }

    private void PlayRandomFootstep()
    {
        AudioClip footstep = null;

        if (footstepSounds.Length > 0)
        {
            footstep = footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Length)];
        }

        if (footstep != null)
        {
            audioSource.volume = footstepVolume;
            audioSource.pitch = footstepPitch;
            audioSource.PlayOneShot(footstep);
        }
    }

    public void Listen()
    {
        listening = true;
    }
    public void StopListen()
    {
        listening = false;
    }

    private List<DoorOpener> GetDoorsInRadius(float radius)
    {
        List<DoorOpener> doorOpeners = new List<DoorOpener>();
        Collider[] doorsColliders = Physics.OverlapSphere(transform.position, radius, doorLayer);
        foreach (var collider in doorsColliders)
        {
            if (collider.TryGetComponent(out DoorOpener opener))
            doorOpeners.Add(opener);
        }

        return doorOpeners;
    }

    public Vector3 RandomNavmeshLocation(float radius, float minDistance, int maxAttempts)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        float highestDistance = 0f;

        for (int i = 0; i < maxAttempts; i++)
        {
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                float distance = Vector3.Distance(transform.position, hit.position);
                if (distance >= minDistance)
                {
                    finalPosition = hit.position;
                    break;
                }
                else if (distance > highestDistance)
                {
                    finalPosition = hit.position;
                    highestDistance = distance;
                }
            }
            randomDirection = UnityEngine.Random.insideUnitSphere * radius;
            randomDirection += transform.position;
        }

        return finalPosition;
    }

    private bool AgentReachedDestiantion()
    {
        float dist = agent.remainingDistance;
        if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        return true;

        return false;
    }

    private Vector2 Vector2XZFromVector3(Vector3 vectorToChange)
    {
        return new Vector2(vectorToChange.x, vectorToChange.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, killRange);

        if (!Application.isEditor || !Application.isPlaying) return;

        if (PlayerInSight()) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;

        Gizmos.DrawLine(RaycastPosition.position, RaycastPosition.position + (playerT.position - RaycastPosition.position).normalized * sightRange);

        if (PlayerInSight() && Vector2.Distance(Vector2XZFromVector3(RaycastPosition.position), Vector2XZFromVector3(playerT.position)) <= nearSightRange) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;
        Gizmos.DrawLine(RaycastPosition.position, RaycastPosition.position + (playerT.position - RaycastPosition.position).normalized * nearSightRange);
    }

    private void OnDisable()
    {
        if (playersCurrentHidingSpot != null)
        {
            playersCurrentHidingSpot.onPlayerExit -= PlayerLeftHidingSpot;
        }
        MentalHealth.Instance.OnMentalHealthIncrease -= MentalHealthIncreased;
        TimeManager.OnDayChanged -= SetMentalHealthRequiredToGoAway;
    }
}
