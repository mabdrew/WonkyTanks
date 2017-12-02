using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;

public class Projectiles : MonoBehaviour {

	public GameObject WonkyBullet;//right now hard coded to wb, should generalize in the future
	private const float BulletLifeSpanScalar = 2f;//later on, these should vary contingent upon bullet type
	private const float VelocityCoeff = 18f;

	void CreateProjectile(CreateProjectileMsg msg)
	{
		Quaternion qt = new Quaternion (msg.xQuat, msg.yQuat, msg.zQuat, msg.wQuat);
		Vector3 pos;
		pos.x = msg.xPos;
		pos.y = msg.yPos;
		pos.z = msg.zPos;

		var wb = (GameObject)Instantiate(WonkyBullet, pos, qt);
		wb.GetComponent<WB> ().PlayerFriendly = msg.PlayerFriendly;

		Color color = new Color ();

		if(!msg.PlayerFriendly){
			wb.GetComponent<WB> ().Damage = 10f;//MAGIC NUMBER
			color.r = 255;
			color.g = 0;
			color.b = 0;
		}
		else{
			wb.GetComponent<WB> ().Damage = 10f;//MAGIC NUMBER
			color.r = 255;
			color.g = 150;
			color.b = 0;
		}

		wb.GetComponent<MeshRenderer> ().material.color = color;
		wb.GetComponent<WB> ().NoCollisions = 0;
		wb.GetComponent<WB> ().CreatingPLayer = msg.TankID;

		// Add velocity to the bullet
		wb.GetComponent<Rigidbody>().velocity = wb.transform.forward * VelocityCoeff;

		// Destroy the bullet after 2 seconds
		if(!msg.PlayerFriendly)
			Destroy(wb, BulletLifeSpanScalar*1);
		else
			Destroy(wb, BulletLifeSpanScalar*0.75f);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
