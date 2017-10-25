using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;


public class GunController : MonoBehaviour {

    public GameObject OwningTank;
    private GameObject OwningGame;

	// Use this for initialization
	void Start () {
        GameUtilities.FindGame(ref OwningGame);
	}

    void MoveGun(MoveGunMsg msg)
    {

    }
	
	// Update is called once per frame
	void Update () {
        //always update its position as per its owning tank. The messaging of this is handled by its owning tank
        Vector3 NewPos = OwningTank.transform.position;
        NewPos.y += 0.5f;//MAGIC NUMBER
		transform.position = NewPos;
	}
}
