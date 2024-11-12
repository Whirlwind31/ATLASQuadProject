using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScore : MonoBehaviour
{
    public float timeUntilDestroyed = 3.0f;
    public LayerMask targetLayer;

    private void OnCollisionEnter(Collision collision)
    {

        if ((targetLayer.value & (1 << collision.gameObject.layer)) != 0)
        {

            Invoke("DestroyObject", timeUntilDestroyed);
        }
    }


    private void DestroyObject()
    {
        Destroy(gameObject);
        ScoreBoard.scoreCount += 1;
    }
}
