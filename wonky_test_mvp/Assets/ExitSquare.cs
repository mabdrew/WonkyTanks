using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSquare : MonoBehaviour {

    public MenuManager MenMan;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("Player"))
        {
            print("Collided");
            if (MenMan != null)
                MenMan.TransitionScene();
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
