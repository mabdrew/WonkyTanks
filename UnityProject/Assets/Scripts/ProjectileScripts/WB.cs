using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;
using EnemyMessages;

public class WB : MonoBehaviour {

	GameObject OwningGame;
	public bool PlayerFriendly;
	public float Damage;
	public int NoCollisions;
	const int MaxNoCollisions = 5;
	const int PlayerCollisionPenalty = 3;//reduce number of max collisions for player projectiles
	public byte CreatingPLayer;

	// Use this for initialization
	void Start () {
		GameUtilities.FindGame (ref OwningGame);
		NoCollisions = 0;

	}

	void OnCollisionEnter (Collision collision)
	{
		if ( (NoCollisions > MaxNoCollisions && !PlayerFriendly) || ( (NoCollisions > MaxNoCollisions - PlayerCollisionPenalty) && PlayerFriendly ) )
			Destroy (gameObject);
		NoCollisions++;
		if (collision.transform.CompareTag ("Player")) {
			print ("Player Hit");
			if (!PlayerFriendly) {
				DamageTankMsg msg = new DamageTankMsg ();
				msg.Amount = Damage;
				msg.FrameNo = Time.frameCount;
				msg.TankID = collision.transform.gameObject.GetComponent<TankBody>().GetTankID();

				OwningGame.BroadcastMessage ("DamageTank", msg, GameUtilities.DONT_CARE_RECIEVER);
				Destroy (gameObject);
			}
		} 
		else if (collision.transform.CompareTag ("Enemies")) {
			print ("Enemy Hit");
			if (PlayerFriendly) {
				DamageEnemyMsg msg = new DamageEnemyMsg ();
				msg.Amount = Damage;
				msg.TankID = CreatingPLayer;
				msg.EType = collision.transform.parent.gameObject.GetComponent<Guardian>().GetEnemyType();
				msg.EnemyID = collision.transform.parent.gameObject.GetComponent<Guardian>().EnemyID;

				OwningGame.BroadcastMessage ("DamageEnemy", msg, GameUtilities.DONT_CARE_RECIEVER);
				Destroy (gameObject);
			}

		}

		if (!PlayerFriendly){
			Damage -= 2;//MAGIC NUMBER guardians are stupid so they have reduced penalty \
			gameObject.GetComponent<MeshRenderer>().material.color /= 8;

		}
		else{
			Damage /= 2;//MAGIC NUMBER so players can exploit bouncing shots too badly
			gameObject.GetComponent<MeshRenderer>().material.color /= 8;

		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
