using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using TMPro;

public class BehaviorTreeAI : MonoBehaviour
{
    private enum AIActionState
    {
        Patrolling,
        Chasing,
        Attacking,
        Searching
    }

    [SerializeField] private AIActionState currentState;

    private NavMeshAgent navMeshAgent;
    private Transform player;
    private Vector3 patrolPoint;
    private float chaseRange = 10f;
    private float attackRange = 5f;
    private float searchDuration = 5f;
    private float searchTimer = 0f;
    private Vector3 lastKnownPlayerPosition;

    [SerializeField] private Transform waypoint1;
    [SerializeField] private Transform waypoint2;
    [SerializeField] private Transform movingObstacle;
    [SerializeField] private float fieldOfViewAngle = 60f; // AI's field of view angle
    [SerializeField] private TextMeshProUGUI detectionText; // Reference to the detection Text element

    private bool delayInProgress = false; // Flag to track delay in progress

    private void Start()
    {
        currentState = AIActionState.Patrolling;
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        detectionText.gameObject.SetActive(false); // Ensure the detection text is initially disabled

        patrolPoint = waypoint1.position;
        navMeshAgent.SetDestination(patrolPoint);
    }

    private void Update()
    {
        switch (currentState)
        {
            case AIActionState.Patrolling:
                PatrollingBehavior();
                break;
            case AIActionState.Chasing:
                ChasingBehavior();
                break;
            case AIActionState.Attacking:
                AttackingBehavior();
                break;
            case AIActionState.Searching:
                SearchingBehavior();
                break;
        }
    }

    private void PatrollingBehavior()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f && !delayInProgress)
        {
            StartCoroutine(WaitBeforeNextWaypoint());
        }

        if (CanSeePlayer())
        {
            currentState = AIActionState.Chasing;
        }
    }

    private IEnumerator WaitBeforeNextWaypoint()
    {
        delayInProgress = true;
        yield return new WaitForSeconds(3f); // 3 second delay
        delayInProgress = false;

        // Switch to the next waypoint
        patrolPoint = (patrolPoint == waypoint1.position) ? waypoint2.position : waypoint1.position;
        navMeshAgent.SetDestination(patrolPoint);
    }

    private void ChasingBehavior()
    {
        if (!IsPlayerBehindObstacle())
        {
            navMeshAgent.SetDestination(player.position);

            if (Vector3.Distance(transform.position, player.position) < attackRange)
            {
                currentState = AIActionState.Attacking;
            }

            if (!CanSeePlayer())
            {
                lastKnownPlayerPosition = player.position;
                currentState = AIActionState.Searching;
                searchTimer = 0f;
            }
        }
    }

    private bool IsPlayerBehindObstacle()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseRange))
        {
            if (hit.collider.CompareTag("MovingObstacle"))
            {
                return true;
            }
        }

        return false;
    }

    private void AttackingBehavior()
    {
        // Attack the player (you can add attack logic here)
    }

    private void SearchingBehavior()
    {
        searchTimer += Time.deltaTime;

        if (searchTimer < searchDuration)
        {
            navMeshAgent.SetDestination(lastKnownPlayerPosition);

            if (CanSeePlayer())
            {
                currentState = AIActionState.Chasing;
                return;
            }
        }
        else
        {
            currentState = AIActionState.Patrolling;
            navMeshAgent.SetDestination(patrolPoint);
        }
    }

    private bool CanSeePlayer()
{
    Vector3 directionToPlayer = player.position - transform.position;

    // Check if player is within chase range
    if (directionToPlayer.magnitude <= chaseRange)
    {
        // Check if player is within field of view angle and in front of the AI
        if (Vector3.Angle(transform.forward, directionToPlayer) <= fieldOfViewAngle * 0.5f)
        {
            // Center raycast
            if (RaycastForPlayer(directionToPlayer))
            {
                return true;
            }

            // Left raycast
            Vector3 leftRayDirection = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward;
            if (RaycastForPlayer(leftRayDirection))
            {
                return true;
            }

            // Right raycast
            Vector3 rightRayDirection = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * transform.forward;
            if (RaycastForPlayer(rightRayDirection))
            {
                return true;
            }
        }
    }

    return false;
}


    private bool RaycastForPlayer(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, chaseRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Raycast hit the player!");
                // Check if there is an obstacle between the AI and the player
                if (!IsPlayerBehindObstacle())
                {
                    SceneManager.LoadScene("DefeatScreen"); // Load defeat screen scene
                    return true;
                }
            }
        }
        return false;
    }
}
