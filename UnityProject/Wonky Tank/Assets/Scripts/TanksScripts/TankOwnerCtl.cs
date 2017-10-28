using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;
public class TankOwnerCtl : MonoBehaviour {


    GameObject[] Children;
	// Use this for initialization
	void Start () {
        int noChildren = gameObject.transform.childCount;
        Children = new GameObject[noChildren];

        print("TankOwner has " + noChildren.ToString() + " children");

        for (int iChild = 0; iChild < noChildren; iChild++)//gather children for easy access
        {
            Children[iChild] = gameObject.transform.GetChild(iChild).gameObject;
        }
	}

    void StrafeTank(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
            child.SendMessage("StrafeTank", msg, GameUtilities.DONT_CARE_RECIEVER);
    }

    void MoveTank(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
            child.SendMessage("MoveTank", msg, GameUtilities.DONT_CARE_RECIEVER);
    }

    void TurnTank(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
            child.SendMessage("TurnTank", msg, GameUtilities.DONT_CARE_RECIEVER);
    }

    void MoveGunHorizontal(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
            child.SendMessage("MoveGunHorizontal", msg, GameUtilities.DONT_CARE_RECIEVER);
    }

    void MoveGunVertical(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
            child.SendMessage("MoveGunVertical", msg, GameUtilities.DONT_CARE_RECIEVER);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
