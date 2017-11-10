using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyMessages;

public class Enemies : MonoBehaviour {

    GameObject OwningGame;

	// Use this for initialization
	void Start () {
        GameUtilities.FindGame(ref OwningGame);
	}

    void DamageEnemy(DamageEnemyMsg msg)
    {
        BroadcastMessage("DamageEnemy", msg, GameUtilities.DONT_CARE_RECIEVER);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
