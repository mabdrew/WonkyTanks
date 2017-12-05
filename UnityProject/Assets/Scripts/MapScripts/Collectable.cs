using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapMessages;
using UnityEngine.UI;

public class Collectable : MonoBehaviour {

    private GameObject playerObject;
    private GameObject collectableObject;

    private Collider playerCollider;
    private Collider collectableCollider;

    private static GameObject OwningGame;

	static int collectablesLeft;

    // Use this for initialization
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player"); //find player. hopefully nothing else gets the "Player" tag
        collectableObject = gameObject; //get collectable game object

        if (playerObject != null)
            playerCollider = playerObject.GetComponent<Collider>(); //get collider for intersection detection

        if (collectableObject != null)
            collectableCollider = collectableObject.GetComponent<Collider>(); //get collider for intersection detection

        GameUtilities.FindGame(ref OwningGame);

		CollectablesLeft = GameObject.FindGameObjectsWithTag("Collectable").Length; //total and remaining collectables set to # collectables in game
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(new Vector3(360, 180, 0) * Time.deltaTime); //rotate the collectable

        CheckTouchingPlayer();
    }

    void CheckTouchingPlayer()
    {
        if (collectableCollider.bounds.Intersects(playerCollider.bounds)) //check for intersection with Player
                                                                          //player and collectables don't collide with each other
        {
            // GetCollectable();
			collectableObject.SetActive(false);
			CollectablesLeft--;


			if (CollectablesLeft <= 0)
			{
				IsFinishActiveMsg msg = new IsFinishActiveMsg(true);
				GameUtilities.Broadcast ("SetIsFinishActive", msg);
			}
        }
    }

	private static int CollectablesLeft
	{
		get
		{
			return collectablesLeft;
		}

		set
		{
			collectablesLeft = value;

			UpdateCollectableTextMsg msg = new UpdateCollectableTextMsg (CollectablesLeft);
			GameUtilities.Broadcast ("UpdateCollectableText", msg);
		}
	}
}
