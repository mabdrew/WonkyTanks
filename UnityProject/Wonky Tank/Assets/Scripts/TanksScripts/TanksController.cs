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

    //madbrew : is there a better way than implementing each of these functions? some sort of auto-forward, maybe?
    void StrafeTank(StrafeTankMsg msg)
    {
        foreach (var child in Children)
                child.SendMessage("StrafeTank", msg, GameUtilities.DONT_CARE_RECIEVER);          
    }

    void MoveTank(MoveTankMsg msg)
    {
        foreach (var child in Children)
            child.SendMessage("MoveTank", msg, GameUtilities.DONT_CARE_RECIEVER);    
    }

    void TurnTank(RotateTankMsg msg)
    {
        foreach (var child in Children)
            child.SendMessage("TurnTank", msg, GameUtilities.DONT_CARE_RECIEVER);
    }
   
	// Update is called once per frame
	void Update () {
		
	}
}