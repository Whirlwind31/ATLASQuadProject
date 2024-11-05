using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleWander : MonoBehaviour
{
    public float wanderWaitTime = 2.0f; 
    public float wanderRadius = 10.0f;   
    public float agentSpeed = 6.0f;
    public float agentAcceleration = 8.0f;
    public float rotationSpeed = 5.0f;
    public float modifier = 6.0f;
    public float avoidDistance = 12.0f;

    public float eggLayChance = 0.5f;
    public float eggMultiplier = 2.0f;

    public GameObject[] objectsToSpawn;


    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField]
    private Transform player;

    private Coroutine currentCoroutine;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            return;
        }


        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;
        agent.updateRotation = false;

        currentCoroutine = StartCoroutine(Wander());
    }

    /// <summary>
    /// Makes the GameObject wander randomly within a defined radius.
    /// </summary>
    IEnumerator Wander()
    {
        while (true)
        {
            yield return new WaitForSeconds(wanderWaitTime);

            Vector3 wanderTarget = GetRandomNavMeshLocation();
            agent.SetDestination(wanderTarget);

            if (animator != null)
            {
                animator.SetBool("Moving", true); 
            }

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            agent.velocity = Vector3.zero;

            if (animator != null)
            {
                animator.SetBool("Moving", false); 
            }

            if (Random.value < eggLayChance && objectsToSpawn.Length > 0)
            {
                if (objectsToSpawn != null)
                {
                    int randomIndex = Random.Range(0, objectsToSpawn.Length);
                    Vector3 spawnPos = transform.position - transform.forward * eggMultiplier;
                    Instantiate(objectsToSpawn[randomIndex], spawnPos, Quaternion.identity);
                }
            }
        }
    }

    IEnumerator Avoid()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        agent.speed = agentSpeed * modifier;

        Vector3 avoidTarget = transform.position + directionAwayFromPlayer * avoidDistance;

        NavMeshHit navHit;
        NavMesh.SamplePosition(avoidTarget, out navHit, avoidDistance, NavMesh.AllAreas);

        if (animator != null)
        {
            animator.SetBool("Moving", true);
        }

        agent.SetDestination(navHit.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        agent.velocity = Vector3.zero;

        if (animator != null)
        {
            animator.SetBool("Moving", false);
        }

        currentCoroutine = StartCoroutine(Wander());
    }

    /// <summary>
    /// Generates a random point within a certain radius on the NavMesh.
    /// </summary>
    /// <returns>A random Vector3 position on the NavMesh.</returns>
    Vector3 GetRandomNavMeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas);

        return navHit.position;
    }

    /// <summary>
    /// Update is called once per frame to handle smooth rotation.
    /// </summary>
    void Update()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceFromPlayer <= avoidDistance)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(Avoid());
        }

        if (agent.velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
