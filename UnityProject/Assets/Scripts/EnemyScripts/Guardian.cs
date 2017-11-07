using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : MonoBehaviour {

    GameObject OwningGame;

    public GameObject TestPlayer;//for testing purposes only
    const float NoticeDistance = 5f;


	// Use this for initialization
	void Start () {
        GameUtilities.FindGame(ref OwningGame);
	}
	

    void LookAtTestPlayer()
    {
        if(TestPlayer!=null)
        {
            transform.LookAt(TestPlayer.transform);
            print("looky!");
        }
    }


	// Update is called once per frame
	void Update () {
		LookAtTestPlayer();
	}
}
