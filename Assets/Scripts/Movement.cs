using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    // Global variables used throughout the script. May be changed from the Unity Editor.
    public float moveSpeed = 6.0f;
    public float avoidDistance = 4.0f;
    public float rotationSpeed = 4.0f; 
    public float moveDistance = 3.0f;
    public float moveTime = 2.0f;
    public float wanderWaitTime = 2.0f; // in seconds
    public float wanderTriggerChance = 0.3f; // has to be between 0 and 1
    public float yPos = 0f;

    // Player object, can be changed from Unity Editor
    public Transform player;

    // Pseudo-random number, will be used to generate a Vector3 object.
    private int vectorNum;

    // Animator object to be used for the GameObject's animations.
    private Animator animator;
    
    // Avoid trigger
    private bool isAvoiding = false;

    private Coroutine currentCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        // Check Console to confirm that the script is run
        Debug.Log("Starting Object!");

        // Getting a reference to the GameObject's animator.
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("Moving", false);
            // Debug.Log(animator.GetBool("Moving"));
        }

        currentCoroutine = StartCoroutine(Wander());

        // TODO: Behavior that makes the asset avoid solid objects while moving. Perhaps make it forcefully turn until 
    }

    /// <summary>
    /// Implements wandering behavior onto the asset. 
    /// Once every set amount of time, there is a set 
    /// chance that the asset moves.
    /// </summary>
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
                    // Debug.Log(animator.GetBool("Moving"));
                }

                vectorNum = Random.Range(1, 9);
                yield return StartCoroutine(WanderingMovement(moveTime, vectorNum));
            }
        }
    }

    /// <summary>
    /// Wandering movement of the asset, where it moves in accordance 
    /// to the output from the DirectionVector method.
    /// </summary>
    /// <param name="inputTime">Movement time in seconds.</param>
    /// <param name="x">Input number to be processed by DirectionVector method.</param>
    IEnumerator WanderingMovement(float inputTime, int x)
    {
        float elapsedT = 0f;
        float finalT = inputTime;

        Vector3 directionVector = DirectionVector(x);

        Vector3 initialPos = transform.position;
        Vector3 targetPos = initialPos + directionVector * moveDistance;
        targetPos.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(directionVector);
        targetRotation.x = 0f;

        Vector3 tempPos;

        while (elapsedT < finalT)
        {
            tempPos = transform.position;
            tempPos = Vector3.Lerp(initialPos, targetPos, elapsedT / finalT);
            tempPos.y = 0f;

            transform.position = tempPos;

            if (Vector3.Distance(transform.position, player.position) < avoidDistance)
            {
                break;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedT / finalT);

            elapsedT += Time.deltaTime;
            yield return null;
        }

        if (Vector3.Distance(transform.position, player.position) >= avoidDistance)
        {
            transform.position = targetPos;
            transform.rotation = targetRotation;

            if (animator != null)
            {
                animator.SetBool("Moving", false);
                // Debug.Log(animator.GetBool("Moving"));
            }
        }
    }

    /// <summary>
    /// Avoiding behavior of the GameObject. It moves away from the player.
    /// </summary>
    IEnumerator Avoid()
    {
        isAvoiding = true;
        float elapsedT = 0f;
        float finalT = moveTime;

        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 initialPos = transform.position;
        
        // Fleeing behavior, added a .5x modifier to mimic faster, fleeing motion.
        Vector3 targetPos = initialPos + directionAwayFromPlayer * (1.5f * moveDistance);
        targetPos.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(directionAwayFromPlayer);
        targetRotation.x = 0f;

        Vector3 tempPos = transform.position;

        if (animator != null)
        {
            animator.SetBool("Moving", true);
        }

        while (elapsedT < finalT)
        {
            tempPos = transform.position;
            tempPos = Vector3.Lerp(initialPos, targetPos, elapsedT / finalT);
            tempPos.y = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedT / finalT);

            transform.position = tempPos;

            elapsedT += Time.deltaTime;
            yield return null; 
        }

        transform.position = targetPos;
        transform.rotation = targetRotation;

        if (animator != null)
        {
            animator.SetBool("Moving", false);
        }
        isAvoiding = false;

        currentCoroutine = StartCoroutine(Wander());
    }

    /// <summary>
    /// Generates a Vector3 object based on the four cardinal directions and four ordinal directions.
    /// </summary>
    /// <param name="y"> A pseudorandom integer number between 1 and 8, inclusive, each assigned to a certain vector:
    /// <para> 1 - North, (0, 0, 1) </para> 
    /// 2 - Northeast, (1, 0, 1)
    /// <para> 3 - East, (1, 0, 0) </para>
    /// 4 - Southeast, (1, 0, -1)
    /// <para> 5 - South, (0, 0, -1) </para>
    /// 6 - Southwest, (-1, 0, 1)
    /// <para> 7 - West, (-1, 0, 0) </para>
    /// 8 - Northwest, (-1, 0, 1) </param>
    /// <returns> A Vector3 object oriened towards the desired direction. </returns>
    public static Vector3 DirectionVector(int y)
    {
        int num = y;
        Vector3 output = Vector3.zero;

        switch(num)
        {
            case 1:
                output += Vector3.forward;
                break;
            case 2:
                output += Vector3.forward;
                output += Vector3.right;
                break;
            case 3:
                output += Vector3.right;
                break;
            case 4:
                output += Vector3.back;
                output += Vector3.right;
                break;
            case 5:
                output += Vector3.back;
                break;
            case 6:
                output += Vector3.back;
                output += Vector3.left;
                break;
            case 7:
                output += Vector3.left;
                break;
            case 8:
                output += Vector3.left;
                output += Vector3.forward;
                break;
        }

        return output;
    }


    /// <summary>
    /// Update is called once per frame. It is used to check the distance between the player 
    /// and the GameObject.
    /// </summary>
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
