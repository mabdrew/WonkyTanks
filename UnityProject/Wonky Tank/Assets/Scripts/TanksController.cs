using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void MoveTank(byte[] fno_and_tid) 
    {
        foreach (var child in Children)
                child.SendMessage("MoveTank", fno_and_tid);
    }
    //madison : unity will complain if the function flatout doesn't exist. It seems exessive to have to have an empty implementation for every function that must traverse the tree
	
	// Update is called once per frame
	void Update () {
		
	}
}