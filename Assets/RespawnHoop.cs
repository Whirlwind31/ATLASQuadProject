using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHoop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject torus_prefab;
    private void OnCollisionEnter(Collision collision) {
        //Another object touches torus
        //collision is the collider that has collided with us
        Debug.Log(collision.gameObject.name + "has collided with the torus");
        if (collision.gameObject.name != "Cube") {
            //Instantiate (torus_prefab, new Vector3 (-33.36f,1.75f,-65.6f), transform.rotation);
            Debug.Log("Respawned");
        }
    }
}
