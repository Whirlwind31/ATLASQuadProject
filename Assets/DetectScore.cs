using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.name == "AlmaPointDetector") {
            Debug.Log("Trigger works");
            Score.currentScore +=1;
            GetComponent<Collider>().enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        //Another object touches torus
        //collision is the collider that has collided with us
        Debug.Log(collision.gameObject.name + "has collided with the torus");

        if (collision.gameObject.name == "AlmaPointDetector") {
            Debug.Log("Point scored");
        }
    }
}
