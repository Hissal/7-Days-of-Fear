using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Transform playerT;

    [SerializeField] private float sightRange;

    private Vector3 playerPositionSnapshot;
    private bool playerWasInSight;
    private bool wandering;
    private bool goingToSnapshot;

    private void Start()
    {
        playerT = GameManager.Instance.playerTransform;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // TODO Slam non closet doors open when walking through

        if (PlayerInSight())
        {
            WalkTowardPlayer();
        }
        else if (playerWasInSight == true)
        {
            playerWasInSight = false;
            SnapshotPlayerPosition();
            StartCoroutine(MoveToPlayerSnapshot());
        }
        else if (!wandering && !goingToSnapshot)
        {
            StartCoroutine(WanderRandomly());
        }
    }

    private IEnumerator MoveToPlayerSnapshot()
    {
        //print("MovingToSnapshot");
        goingToSnapshot = true;

        agent.SetDestination(playerPositionSnapshot);

        yield return new WaitUntil(() => Vector3.Distance(transform.position, playerPositionSnapshot) < 0.1f || PlayerInSight());

        //print("ArrivedAtSnapshot");

        if (!PlayerInSight())
        {
            Vector3 newPos = transform.position + transform.forward * 1f;
            agent.SetDestination(newPos);

            yield return new WaitUntil(() => Vector3.Distance(transform.position, newPos) < 0.1f || PlayerInSight());

            goingToSnapshot = false;

            if (!PlayerInSight()) StartCoroutine(WanderRandomly());
        }

        goingToSnapshot = false;
    }

    private bool PlayerInSight()
    {

        if (Physics.Raycast(transform.position, (playerT.position - transform.position).normalized, out RaycastHit hit, sightRange))
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

    private IEnumerator WanderRandomly()
    {
        if (goingToSnapshot == true) yield break;

        //print("Wandering");
        wandering = true;

        agent.SetDestination(RandomNavmeshLocation(2f));

        yield return new WaitForSeconds(Random.Range(1f, 6f));

        if (!PlayerInSight())
        {
            StartCoroutine(WanderRandomly());
        }
        else
        {
            wandering = false;
        }
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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (PlayerInSight()) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + (playerT.position - transform.position).normalized * sightRange);
    }
}
