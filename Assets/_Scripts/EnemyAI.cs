using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Transform playerT;

    [SerializeField] private float sightRange;
    [SerializeField] private float nearSightRange;
    [SerializeField] private float killRange;

    private Vector3 playerPositionSnapshot;
    private bool playerWasInSight;

    [SerializeField] private Animator animator;

    public enum EnemyState {Wandering, ChasingPlayer, PlayerHidingSequence, Stunned}
    public EnemyState State { get; private set; }

    [SerializeField] private LayerMask seenLayers;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask lightLayer;

    private List<LightFlicker> flickeringLights = new List<LightFlicker>();

    private bool listening;
    private HidingSpot playersCurrentHidingSpot;

    private bool killPlayer;

    private void Start()
    {
        playerT = GameManager.Instance.playerTransform;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }
    public void Init()
    {
        StartCoroutine(WanderRandomly());
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void Update()
    {
        if (agent.isOnNavMesh == false)
        {
            throw new System.Exception("Enemy is not on navmesh " + gameObject);
        }

        if (!AgentReachedDestiantion()) animator.SetBool("Moving", true);
        else animator.SetBool("Moving", false);

        if (State == EnemyState.Stunned) animator.SetBool("Moving", false);

        FlickerNearbyLights();

        if (State == EnemyState.PlayerHidingSequence || State == EnemyState.Stunned) return;

        OpenNearbyDoors();

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
                flickerToRemove.flicker = false;
                flickerToRemove.light.intensity = 0f;
                flickeringLights.Remove(flickerToRemove);
            }
        }
    }

    private void OpenNearbyDoors()
    {
        List<DoorOpener> doorOpeners = GetDoorsInRadius(2f);
        foreach (var door in doorOpeners)
        {
            if (door.isMovableByEnemy && !door.isCloset && Vector2.Distance(Vector2XZFromVector3(transform.position), Vector2XZFromVector3(door.transform.position)) < 0.5f)
            {
                SlamDoorOpen(door);
            }
            else if (door.isMovableByEnemy && !door.isCloset && !door.moving) door.MoveDoor(Random.Range(200f, 500f), Random.Range(-10f, 10f), true);
        }
    }

    private IEnumerator MoveToPlayerSnapshot()
    {
        print("MovingToSnapshot");

        agent.SetDestination(playerPositionSnapshot);

        yield return new WaitUntil(() => AgentReachedDestiantion() || PlayerInSight());

        if (!PlayerInSight())
        {
            print("Arrived At Snapshot Player Not In Sight Walking forward");
            Vector3 newPos = transform.position + transform.forward * 1f;
            agent.SetDestination(newPos);
            yield return null;

            yield return new WaitUntil(() => AgentReachedDestiantion() || PlayerInSight());
            print("Arrived at new position");

            if (!PlayerInSight()) StartCoroutine(WanderRandomly());
        }
    }

    private IEnumerator WanderRandomly()
    {
        State = EnemyState.Wandering;

        print("Wandering");

        agent.SetDestination(RandomNavmeshLocation(5f));

        yield return new WaitUntil(() => AgentReachedDestiantion() || State != EnemyState.Wandering);

        float secondsToWait = Random.Range(0f, 5f);
        float timeElapsed = 0f;

        while (State == EnemyState.Wandering && timeElapsed < secondsToWait)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (State == EnemyState.Wandering)
        {
            StartCoroutine(WanderRandomly());
        }
    }

    private bool PlayerInSight()
    {
        if (Physics.Raycast(transform.position, (playerT.position - transform.position).normalized, out RaycastHit hit, sightRange, seenLayers))
        {
            if (hit.transform.parent == playerT)
            {
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
        agent.isStopped = true;
        State = EnemyState.Stunned;
        if (time < 0) return;
        StartCoroutine(StunRoutine(time));
    }
    private IEnumerator StunRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        UnStun();
    }
    public void UnStun()
    {
        StopAllCoroutines();
        agent.isStopped = false;
        StartCoroutine(WanderRandomly());
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

        // TODO Kill Player Upon Reaching The Hiding Spot
        if (distanceToPlayer <= nearSightRange && State == EnemyState.ChasingPlayer)
        {
            WalkToHidingSpot(closetFront.position, Quaternion.Inverse(closetFront.rotation));
            print("Kill PLayer Upon Reaching The Hiding Spot");
        } 
        else if (distanceToPlayer <= sightRange && State == EnemyState.ChasingPlayer) WalkToHidingSpot(closetFront.position, Quaternion.Inverse(closetFront.rotation));
    }
    private void WalkToHidingSpot(Vector3 locationToWalkTo, Quaternion desiredRotation)
    {
        print("Walking to hiding spot");

        StopAllCoroutines();
        StartCoroutine(WalkToHidingSpotSequence(locationToWalkTo, desiredRotation));
    }
    private IEnumerator WalkToHidingSpotSequence(Vector3 locationToWalkTo, Quaternion desiredRotation)
    {
        State = EnemyState.PlayerHidingSequence;

        playerWasInSight = false;
        agent.SetDestination(locationToWalkTo);

        yield return new WaitUntil(() => Vector2XZFromVector3(transform.position) == Vector2XZFromVector3(locationToWalkTo));

        // TODO Stare into closet
        print("Reached Hiding Spot... STARING");
        animator.SetTrigger("Stare");
        float timeStaring = 0f;

        int coinFlip = Random.Range(0, 2);
        int coinFlip2 = Random.Range(0, 2);
        bool stare2 = coinFlip == 1;
        bool stare3 = coinFlip2 == 1;
        bool doneRotating = false;
        Quaternion startRotation = transform.rotation;

        float timeToMoveDoor = Random.Range(1f, 15f);
        bool movedDoor = false;
        float timeToMoveDoor2 = Random.Range(1f, 15f);
        bool movedDoor2 = false;

        float stareSpeed1 = Random.Range(0.66f, 1f);
        float stareSpeed2 = Random.Range(0.66f, 1f);

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
                print("Rotating to: " + desiredRotation.eulerAngles);
                float precentageDone = timeStaring / 1f;
                transform.rotation = Quaternion.Lerp(startRotation, desiredRotation, precentageDone);
                if (transform.rotation == desiredRotation) doneRotating = true;
            }

            if (GameManager.Instance.IsPlayerHoldingBreath() == false && listening && !killPlayer)
            {
                KillPlayerInCloset();
                print("Player failed to hold breath... Kill Player");
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
                }
            }
            yield return null;
        }

        print("Finished Staring... Walking Away");

        animator.speed = 1f;
        Vector3 randomLocation = RandomNavmeshLocation(10f);
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
        StartCoroutine(WanderRandomly());
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
        door.MoveDoor(50000f, 360f, false);
        animator.SetTrigger("Attack");
    }

    public void RandomChanceOpenHidingSpotDoors(int chance)
    {
        int openDoorChance = chance;
        int openDoorRoll = Random.Range(0, 100);
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
            door.MoveDoor(Random.Range(100f, 300f), Random.Range(5f, 20f), true);
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

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
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

        Gizmos.DrawLine(transform.position, transform.position + (playerT.position - transform.position).normalized * sightRange);

        if (PlayerInSight() && Vector2.Distance(Vector2XZFromVector3(transform.position), Vector2XZFromVector3(playerT.position)) <= nearSightRange) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (playerT.position - transform.position).normalized * nearSightRange);
    }

    private void OnDisable()
    {
        if (playersCurrentHidingSpot != null)
        {
            playersCurrentHidingSpot.onPlayerExit -= PlayerLeftHidingSpot;
        }
    }
}
