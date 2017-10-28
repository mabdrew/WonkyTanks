using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour {

    private GameObject playerObject;
    private GameObject collectableObject;

    private Collider playerCollider;
    private Collider collectableCollider;

    // Use this for initialization
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player"); //find player. hopefully nothing else gets the "Player" tag
        collectableObject = gameObject; //get collectable game object

        if (playerObject != null)
            playerCollider = playerObject.GetComponent<Collider>(); //get collider for intersection detection

        if (collectableObject != null)
            collectableCollider = collectableObject.GetComponent<Collider>(); //get collider for intersection detection
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(new Vector3(360, 180, 0) * Time.deltaTime); //rotate the collectable

        if (collectableCollider.bounds.Intersects(playerCollider.bounds)) //check for intersection with Player
            //player and collectables don't collide with each other
        {
            collectableObject.gameObject.SetActive(false); //make collectable disappear
        }
    }
}
