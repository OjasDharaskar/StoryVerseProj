
using UnityEngine;
using UnityEngine.AI;

public class CurseEntityRoam : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform player;
    public float chaseSpeed = 3.5f;
    public float stoppingDistance = 1.8f;
    public float updateRate = 0.2f;

    private NavMeshAgent agent;
    private float nextUpdateTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("Alien needs a NavMeshAgent!");
            enabled = false;
            return;
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player == null)
        {
            Debug.LogError(
                "Assign the Main Character to the Player field!"
            );
            enabled = false;
            return;
        }

        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.autoRepath = true;
        agent.isStopped = false;
    }

    void Update()
    {
        if (player == null || agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateRate;

            agent.SetDestination(player.position);
        }
    }
}
