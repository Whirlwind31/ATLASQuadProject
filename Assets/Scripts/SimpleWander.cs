using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleWander : MonoBehaviour
{
    public float wanderWaitTime = 2.0f;  // Time between wander actions
    public float wanderRadius = 10.0f;    // Radius within which the object will wander
    public float agentSpeed = 6.0f;
    public float agentAcceleration = 8.0f;
    public float rotationSpeed = 5.0f;    // Speed for smooth rotation

    private NavMeshAgent agent;
    private Animator animator;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        // Get components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            return;
        }

        // Set agent properties
        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;

        // Disable automatic rotation to prevent snapping
        agent.updateRotation = false;

        // Start wandering
        StartCoroutine(Wander());
    }

    /// <summary>
    /// Makes the GameObject wander randomly within a defined radius.
    /// </summary>
    IEnumerator Wander()
    {
        while (true)
        {
            yield return new WaitForSeconds(wanderWaitTime);

            // Get a random position on the NavMesh
            Vector3 wanderTarget = GetRandomNavMeshLocation();
            agent.SetDestination(wanderTarget);

            if (animator != null)
            {
                animator.SetBool("Moving", true);  // Start moving animation
            }

            // Wait until the agent reaches its destination
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            // Stop the agent's velocity to prevent sliding
            agent.velocity = Vector3.zero;

            if (animator != null)
            {
                animator.SetBool("Moving", false);  // Stop moving animation
            }
        }
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
        // If the agent is moving, smoothly rotate towards the movement direction
        if (agent.velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
