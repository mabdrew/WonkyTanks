using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapMessages;

public class CollectableController : MonoBehaviour {

    private GameObject playerObject;
    private GameObject collectableObject;

    private Collider playerCollider;
    private Collider collectableCollider;

    private GameObject OwningGame;


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
            GetCollectableMsg msg = new GetCollectableMsg(collectableObject);
            OwningGame.BroadcastMessage("GetCollectable", msg, GameUtilities.DONT_CARE_RECIEVER); //make collectable disappear
        }
    }

    void GetCollectable(GetCollectableMsg msg)
    {
        if (msg.collectableObj == collectableObject)
        {
            collectableObject.gameObject.SetActive(false);
        }
    }
}
