using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.Time;
using TankMessages;
using MapMessages;

public class BulletCollision : MonoBehaviour {

	private GameObject bulletObject;
    private GameObject wallObject;

    private Collider bulletCollider;
    private Collider wallCollider;

    private GameObject OwningGame;
	
	// Use this for initialization
	void Start () 
	{
		wallObject = GameObject.FindGameObjectWithTag("Walls"); //find walls
        bulletObject = gameObject; //get collectable game object

        if (bulletObject != null)
            bulletCollider = bulletObject.GetComponent<Collider>(); //get collider for intersection detection

        if (wallObject != null)
            wallCollider = wallObject.GetComponent<Collider>(); //get collider for intersection detection

        GameUtilities.FindGame(ref OwningGame);
	}
	
	// Update is called once per frame
	void Update () {
		CheckTouchingWall();
	}
	void CheckTouchingWall()
    {
        if (bulletCollider.bounds.Intersects(wallCollider.bounds)) //check for intersection with Walls
        {
            GetBulletMsg msg = new GetBulletMsg(bulletObject);
            OwningGame.BroadcastMessage("GetBullet", msg, GameUtilities.DONT_CARE_RECIEVER); //make bullet disappear
        }
    }
	void GetBullet(GetBulletMsg msg)
    {
        if (msg.bulletObj == bulletObject)
        {
            bulletObject.gameObject.SetActive(false);
        }
    }
}

