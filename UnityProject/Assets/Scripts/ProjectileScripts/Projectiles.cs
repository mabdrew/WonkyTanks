using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;

public class Projectiles : MonoBehaviour {

	public GameObject WonkyBullet;//right now hard coded to wb, should generalize in the future
	private const float BulletLifeSpan = 3f;//later on, these should vary contingent upon bullet type
	private const float VelocityCoeff = 24f;

	void CreateProjectile(CreateProjectileMsg msg)
	{
		Quaternion qt = new Quaternion (msg.xQuat, msg.yQuat, msg.zQuat, msg.wQuat);
		Vector3 pos;
		pos.x = msg.xPos;
		pos.y = msg.yPos;
		pos.z = msg.zPos;

		var wb = (GameObject)Instantiate(WonkyBullet, pos, qt);

		// Add velocity to the bullet
		wb.GetComponent<Rigidbody>().velocity = wb.transform.forward * VelocityCoeff;

		// Destroy the bullet after 2 seconds
		Destroy(wb, BulletLifeSpan);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
