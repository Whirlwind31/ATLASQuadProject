using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public float wanderWaitTime = 2.0f; // in seconds
    public float wanderTriggerChance = 0.3f; // has to be between 0 and 1
    public float avoidDistance = 4.0f;
    public float cooldownTime = 30f;
    public Transform player;

    public float agentSpeed = 6.0f; // Adjust this value to change speed
    public float agentAcceleration = 8.0f; // Adjust this value to change acceleration

    private Animator animator;
    private NavMeshAgent agent;
    private Coroutine currentCoroutine;
    private bool isAvoiding = false;
    private bool recentlyAvoided = false;
    private int increment = 1;

    void Start()
    {
        Debug.Log("Starting Object!");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (animator != null)
        {
            animator.SetBool("Moving", false);
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            return;
        }

        // Set agent speed and acceleration
        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;

        currentCoroutine = StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        while (true)
        {
            yield return new WaitForSeconds(wanderWaitTime);
            if (Random.value < wanderTriggerChance)
            {
                if (animator != null)
                {
                    animator.SetBool("Moving", true);
                }

                Vector3 wanderTarget = GetRandomNavMeshLocation();
                Debug.Log("Wandering to: " + wanderTarget);
                agent.SetDestination(wanderTarget);

                while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                {
                    yield return null;
                }

                if (animator != null)
                {
                    animator.SetBool("Moving", false);
                }
            }
        }
    }

    Vector3 GetRandomNavMeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * avoidDistance;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, avoidDistance, NavMesh.AllAreas);
        return navHit.position;
    }

    IEnumerator Avoid()
    {
        isAvoiding = true;
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        float modifier = Mathf.Pow(1.5f, increment);
        Vector3 avoidTarget = transform.position + directionAwayFromPlayer * (modifier * avoidDistance);

        NavMeshHit navHit;
        NavMesh.SamplePosition(avoidTarget, out navHit, avoidDistance, NavMesh.AllAreas);

        if (animator != null)
        {
            animator.SetBool("Moving", true);
        }

        Debug.Log("Avoiding to: " + navHit.position);
        agent.SetDestination(navHit.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("Moving", false);
        }

        if (!recentlyAvoided)
        {
            recentlyAvoided = true;
        }

        isAvoiding = false;

        currentCoroutine = StartCoroutine(Wander());

        StartCoroutine(AvoidCooldown());
    }

    IEnumerator AvoidCooldown()
    {
        float elapsedT = 0f;
        while (elapsedT < cooldownTime)
        {
            if (Vector3.Distance(player.position, transform.position) <= avoidDistance)
            {
                yield break;
            }
            elapsedT += Time.deltaTime;
            yield return null;
        }
        increment = 0;  // Reset increment after cooldown period
        recentlyAvoided = false;
    }

    void Update()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceFromPlayer < avoidDistance && !isAvoiding)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(Avoid());
        }
    }
}
