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
    [SerializeField] private float killRadius;

    private Vector3 playerPositionSnapshot;
    private bool playerWasInSight;

    public enum EnemyState {Wandering, ChasingPlayer, PlayerHidingSequence, Stunned}
    public EnemyState State { get; private set; }

    [SerializeField] private LayerMask seenLayers;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask doorLayer;

    private void Start()
    {
        playerT = GameManager.Instance.playerTransform;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (agent.isOnNavMesh == false)
        {
            throw new System.Exception("Enemy is not on navmesh " + gameObject);
        }

        if (State == EnemyState.PlayerHidingSequence || State == EnemyState.Stunned) return;

        Collider[] doorsColliders = Physics.OverlapSphere(transform.position, 2f, doorLayer);
        if (doorsColliders.Length > 0)
        {
            foreach (var collider in doorsColliders)
            {
                DoorOpener doorOpener = collider.GetComponent<DoorOpener>();
                if (!doorOpener.isCloset && Physics.OverlapSphere(transform.position, 0.5f, doorLayer).Length > 0) doorOpener.MoveDoor(50000f, 360f, false);
                else if (!doorOpener.isCloset && !doorOpener.moving) doorOpener.MoveDoor(500f, Random.Range(-10f, 10f), true);
            }
        }

        if (PlayerInSight())
        {
            WalkTowardPlayer();
            State = EnemyState.ChasingPlayer;
            if (Physics.OverlapSphere(transform.position, killRadius, playerLayer).Length > 0)
            {
                print("Player In Kill Radius.. KILL PLAYER");
            }
        }
        else if (playerWasInSight == true)
        {
            playerWasInSight = false;
            SnapshotPlayerPosition();
            StartCoroutine(MoveToPlayerSnapshot());
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

    public void PlayerEnteredHidingSpot(Vector3 locationToWalkTo)
    {
        float distanceToPlayer = Vector3.Distance(playerT.position, transform.position);

        if (distanceToPlayer > sightRange) return;

        print("Enemy Knows Player Entered Hiding Spot " + locationToWalkTo);

        // TODO Kill Player Upon Reaching The Hiding Spot
        if (distanceToPlayer <= nearSightRange && State == EnemyState.ChasingPlayer)
        {
            WalkToHidingSpot(locationToWalkTo);
            print("Kill PLayer Upon Reaching The Hiding Spot");
        } 
        else if (distanceToPlayer <= sightRange && State == EnemyState.ChasingPlayer) WalkToHidingSpot(locationToWalkTo);
    }
    private void WalkToHidingSpot(Vector3 locationToWalkTo)
    {
        print("Walking to hiding spot");

        StopAllCoroutines();
        StartCoroutine(WalkToHidingSpotSequence(locationToWalkTo));
    }
    private IEnumerator WalkToHidingSpotSequence(Vector3 locationToWalkTo)
    {
        State = EnemyState.PlayerHidingSequence;

        playerWasInSight = false;
        agent.SetDestination(locationToWalkTo);

        yield return new WaitUntil(() => Vector2XZFromVector3(transform.position) == Vector2XZFromVector3(locationToWalkTo));

        // TODO Stare into closet
        print("Reached Hiding Spot... STARING");
        float timeStaring = 0f;
        float safeTimeBeforeCanKill = 0.5f;

        while (timeStaring < 3f)
        {
            // Stare lolxd
            Vector3 desiredRotation = playerT.position - transform.position;
            desiredRotation.y = 0;
            desiredRotation.Normalize();
            transform.rotation = Quaternion.Euler(Vector3.RotateTowards(transform.rotation.eulerAngles, desiredRotation, 1f, 1f));
            timeStaring += Time.deltaTime;

            if (GameManager.Instance.IsPlayerHoldingBreath() == false && timeStaring >= safeTimeBeforeCanKill)
            {
                // TODO Kill Player RAAAAAH
                print("Player failed to hold breath... Kill Player");
            }
            yield return null;
        }

        // TODO Add chance to come back to stare again
        // TODO Walk Away From Hiding Spot (start by walking to the side out of sight of player)
        print("Finished Staring... Walking Away");

        Vector3 randomLocation = RandomNavmeshLocation(10f);
        agent.SetDestination(randomLocation);

        while (Vector2XZFromVector3(transform.position) != Vector2XZFromVector3(randomLocation))
        {
            if (PlayerInSight())
            {
                print("Enemy spotted player while walking away from hiding spot");
                yield break;
            }
            yield return null;
        }

        print("Reached a location away from hiding spot");
        StartCoroutine(WanderRandomly());
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
        Gizmos.DrawWireSphere(transform.position, killRadius);

        if (!Application.isEditor || !Application.isPlaying) return;

        if (PlayerInSight()) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + (playerT.position - transform.position).normalized * sightRange);

        if (PlayerInSight() && Vector2.Distance(Vector2XZFromVector3(transform.position), Vector2XZFromVector3(playerT.position)) <= nearSightRange) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (playerT.position - transform.position).normalized * nearSightRange);
    }
}
