using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

   // private Rigidbody rigBod;

    public float playerSpeed;
    public float rotateSpeed;
 

    // Use this for initialization
    void Start () {	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * playerSpeed;
            turn(); //having a call to turn here enables turning while moving
        }

        //move backward
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * playerSpeed;
            turn(); //enable turning while moving backward
        }

        //rotate left or right while not movin'
        turn(); //I think tanks can do that anyway
    }

    void turn() //rotate left or right
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0.0f, Input.GetAxis("Horizontal") * rotateSpeed, 0.0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Finish"))
        {
            // Loads title screen.
            SceneManager.LoadScene("Title", LoadSceneMode.Single);
        }
    }
}
