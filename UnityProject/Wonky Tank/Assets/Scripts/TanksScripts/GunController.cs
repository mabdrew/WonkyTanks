using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;


public class GunController : MonoBehaviour {

    public GameObject OwningTank;
    private GameObject OwningGame;

    public KeyCode Left;
    public KeyCode Right;

    public KeyCode Up;
    public KeyCode Down;

    private float GunRotateSpeed;

	// Use this for initialization
	void Start () {
        GameUtilities.FindGame(ref OwningGame);
        Left = KeyCode.H;
        Right = KeyCode.K;

        Up = KeyCode.U;
        Down = KeyCode.J;

        GunRotateSpeed = 20f;
	}

    void MoveGunVertical(TankComponentMovementMsg msg)
    {   //
        if (msg.TankID == OwningTank.GetComponent<PlayerController>().TankID)
        {
            if (msg.Direction)
            {
                transform.Rotate(Vector3.right, 5 * GunRotateSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(Vector3.right, -5 * GunRotateSpeed * Time.deltaTime);
            }
        }
    }

    void CheckVerticalMove() 
    {
        if (Input.GetKey(Up))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<PlayerController>().TankID,
                                                            Time.frameCount, true);
            OwningGame.SendMessage("MoveGunVertical", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
        if (Input.GetKey(Down))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<PlayerController>().TankID,
                                                Time.frameCount, false);
            OwningGame.SendMessage("MoveGunVertical", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }

    void MoveGunHorizontal(TankComponentMovementMsg msg)
    {
        if(msg.TankID==OwningTank.GetComponent<PlayerController>().TankID)
        {
            if (msg.Direction)
            {
                transform.Rotate(Vector3.forward, 5*GunRotateSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(Vector3.forward, -5 * GunRotateSpeed * Time.deltaTime);
            }
        }
    }

    void CheckHorizontalMove() 
    {
        if(Input.GetKey(Left))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<PlayerController>().TankID,
                                                            Time.frameCount, true);
            OwningGame.SendMessage("MoveGunHorizontal",msg,GameUtilities.DONT_CARE_RECIEVER);
        }
        if (Input.GetKey(Right))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<PlayerController>().TankID,
                                                Time.frameCount, false);
            OwningGame.SendMessage("MoveGunHorizontal", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }

    void MoveGun()
    {
        CheckVerticalMove();
        CheckHorizontalMove();
    }
	
	// Update is called once per frame
	void Update () {
        //always update its position as per its owning tank. The messaging of this is handled by its owning tank
        Vector3 NewPos = OwningTank.transform.position;
        NewPos.y += 0.5f;//MAGIC NUMBER
		transform.position = NewPos;
        MoveGun();
	}
}
