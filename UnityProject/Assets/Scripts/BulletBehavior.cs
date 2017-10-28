using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.Time;
using TankMessages;
using System;

public class BulletBehavior : MonoBehaviour {

	public GameObject WonkyBullet;
	public Transform BulletSpawn;
	
	// Use this for initialization
	void Start () {
		
	}
	
	//Fires WonkyBullet from turret
	
	void Fire()
	{
    // Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate (
			WonkyBullet,
			BulletSpawn.position,
			BulletSpawn.rotation);

    // Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

    // Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Fire();
		}
	}
}
