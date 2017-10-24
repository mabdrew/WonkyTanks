using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;

public class TanksController : MonoBehaviour {

    GameObject[] Children;

	// Use this for initialization
	void Start () {
        int noChildren = gameObject.transform.childCount;
        Children = new GameObject[noChildren];

        print("tanks has " + noChildren.ToString() + " children");

        for (int iChild = 0; iChild < noChildren; iChild++)//gather children for easy access
        {
            Children[iChild] = gameObject.transform.GetChild(iChild).gameObject;
        }
	}

    void MoveTank(MoveTankMsg fno_tid) 
    {
        foreach (var child in Children)
            child.SendMessage("MoveTank", fno_tid, SendMessageOptions.DontRequireReceiver);
    }
   
	// Update is called once per frame
	void Update () {
		
	}
}