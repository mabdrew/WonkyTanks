using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public GameObject OwningTank;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 NewPos = OwningTank.transform.position;
        NewPos.y += 0.5f;//MAGIC NUMBER
		transform.position = NewPos;
        //Quaternion NewRot = OwningTank.transform.rotation;
        //NewRot.x = transform.rotation.x;
        Quaternion NewRot = transform.rotation;
        transform.rotation = NewRot;
	}
}
