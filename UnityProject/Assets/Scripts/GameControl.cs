using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;

public class GameControl : MonoBehaviour {

    GameObject[] Children;

	// Use this for initialization
	void Start () {
        int noChildren = gameObject.transform.childCount;
        Children = new GameObject[noChildren];

        print("game has " + noChildren.ToString() + " children");
        
        for(int iChild=0; iChild<noChildren; iChild++)//gather children for easy access
        {
            Children[iChild] = gameObject.transform.GetChild(iChild).gameObject;
        }
	}

    //TANKSTUFF
    //madbrew : is there a better way than implementing each of these functions? some sort of auto-forward, maybe?
    void MoveTank(TankComponentMovementMsg fno_tid)
    { //send move msg's to all child tanks
        foreach (var child in Children)
        {
            if (child.CompareTag("Tanks"))
            {   //madbrew -- general question : should we try and prune message forwarding when necessary?
                child.SendMessage("MoveTank", fno_tid, SendMessageOptions.DontRequireReceiver);
                break;
            }
        }
    }

    void StrafeTank(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
        {
            if (child.CompareTag("Tanks"))
            {   //madbrew -- general question : should we try and prune message forwarding when necessary?
                child.SendMessage("StrafeTank", msg, GameUtilities.DONT_CARE_RECIEVER);
                break;
            }
        }
    }

    void TurnTank(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
        {
            if (child.CompareTag("Tanks"))
            {   //madbrew -- general question : should we try and prune message forwarding when necessary?
                child.SendMessage("TurnTank", msg, GameUtilities.DONT_CARE_RECIEVER);
                break;
            }
        }
    }

    void MoveGunHorizontal(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
        {
            if (child.CompareTag("Tanks"))
            {   //madbrew -- general question : should we try and prune message forwarding when necessary?
                child.SendMessage("MoveGunHorizontal", msg, GameUtilities.DONT_CARE_RECIEVER);
                break;
            }
        }
    }

    void MoveGunVertical(TankComponentMovementMsg msg)
    {
        foreach (var child in Children)
        {
            if (child.CompareTag("Tanks"))
            {   //madbrew -- general question : should we try and prune message forwarding when necessary?
                child.SendMessage("MoveGunVertical", msg, GameUtilities.DONT_CARE_RECIEVER);
                break;
            }
        }
    }

    //ENDTANKSTUFF

	// Update is called once per frame
	void Update () {
		
	}
}