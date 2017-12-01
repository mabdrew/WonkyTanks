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
	public byte CreatingPLayer;

	// Use this for initialization
	void Start () {
		GameUtilities.FindGame (ref OwningGame);
		NoCollisions = 0;

	}

	void OnCollisionEnter (Collision collision)
	{
		if (NoCollisions > MaxNoCollisions)
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

				//public EnemyType EType; 
				//public byte EnemyID;
				//public byte TankID;//the tank this damage is from
				//public float Amount;

				DamageEnemyMsg msg = new DamageEnemyMsg ();
				print(collision.transform.gameObject.name);
				msg.Amount = Damage;
				msg.TankID = CreatingPLayer;
				msg.EType = collision.transform.parent.gameObject.GetComponent<Guardian>().GetEnemyType();
				msg.EnemyID = collision.transform.parent.gameObject.GetComponent<Guardian>().EnemyID;
				print(collision.transform.childCount);

				OwningGame.BroadcastMessage ("DamageEnemy", msg, GameUtilities.DONT_CARE_RECIEVER);
				Destroy (gameObject);
			}

		}
		Damage -= 2;//MAGIC NUMBER
	}

	// Update is called once per frame
	void Update () {
		
	}
}
