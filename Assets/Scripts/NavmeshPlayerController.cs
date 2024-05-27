using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NavmeshPlayerController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveToClickPoint();
        }
    }

    private void MoveToClickPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            agent.SetDestination(hit.point);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Win();
        }
    }

    private void Win()
    {
        SceneManager.LoadScene("VictoryScreen");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Lose();
        }
    }

    private void Lose()
    {
        StartCoroutine(HandleLose());
    }

    private IEnumerator HandleLose()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("DefeatScreen");
    }
}
