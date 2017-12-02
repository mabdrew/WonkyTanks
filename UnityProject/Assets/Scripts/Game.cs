using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;

public class Game : MonoBehaviour {

    public Camera GunCamera;
    public Camera TankCamera;
    private KeyCode Switch;
    int FrameSwitched;
    private const int FrameDelta = 15;

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

        Switch = KeyCode.LeftControl;
        if (GunCamera != null && TankCamera != null)
        {
            GunCamera.enabled = false;
            TankCamera.enabled = true;
        }

        FrameSwitched = Time.frameCount;
	}
	/*
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
	*/
    void SwitchCameraLocal()
    {   //on the LOCAL game instance, switch the active camera

        if ((Time.frameCount > FrameSwitched + FrameDelta) && Input.GetKey(Switch))
        {
            if (GunCamera != null && TankCamera != null)
            {
                GunCamera.enabled = !GunCamera.enabled;
                TankCamera.enabled = !TankCamera.enabled;
                FrameSwitched = Time.frameCount;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        SwitchCameraLocal();
	}
}