using UnityEngine;
using UnityEngine.AI;

public class MovingObstacle : MonoBehaviour
{
    public Transform pointA; // Start point
    public Transform pointB; // End point
    public float speed = 2f; // Movement speed

    private Vector3 targetPosition;
    private Rigidbody rb;

    void Start()
    {
        targetPosition = pointB.position;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        MoveObstacle();
    }

    void MoveObstacle()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NavMeshAgent playerAgent = collision.gameObject.GetComponent<NavMeshAgent>();
            if (playerAgent != null)
            {
                playerAgent.isStopped = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NavMeshAgent playerAgent = collision.gameObject.GetComponent<NavMeshAgent>();
            if (playerAgent != null)
            {
                playerAgent.isStopped = false;
            }
        }
    }
}
