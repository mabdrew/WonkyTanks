using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.Time;
using TankMessages;
using System;


public class PlayerController : MonoBehaviour {

   // private Rigidbody rigBod;

    public float PlayerSpeed;
    public float RotateSpeed;
    public float StrafeSpeed;

    public KeyCode Forward;
    public KeyCode Backward;
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode StrafeLeft;
    public KeyCode StrafeRight;


    GameObject OwningGame;
    int CurrentFrame; //Q : figure out how to fix framerates. Better way to track frames?
    public byte TankID;

    private float Stamina;
    private float health;

    byte GetTankID()
    {
        return TankID;
    }

    // Use this for initialization
    void Start () 
    {
        Forward = KeyCode.W;
        Backward = KeyCode.S;
        Left = KeyCode.A;
        Right = KeyCode.D;
        StrafeLeft = KeyCode.Z;
        StrafeRight = KeyCode.X;

        //on start up find your parent game
        GameUtilities.FindGame(ref OwningGame);

        StrafeSpeed = 2.0f;
        Stamina = 100.0f;
        health = 100.0f;
    }

    void TurnTank(TankComponentMovementMsg msg) //rotate Left or Right
    {
        if(msg.TankID==TankID)
        {
            if(msg.Direction)
            {
                transform.Rotate(0.0f, Input.GetAxis("Horizontal") * RotateSpeed, 0.0f);
            }
            else
            {
                transform.Rotate(0.0f, Input.GetAxis("Horizontal") * RotateSpeed, 0.0f);
            }
        }
    }

    void MoveTank(TankComponentMovementMsg msg)
    {   
        if(msg.TankID==TankID)
        {
            if (msg.Direction)
            {   //forward
                transform.position += transform.forward * Time.deltaTime * PlayerSpeed;
            }
            else
            {   //backward
                transform.position -= transform.forward * Time.deltaTime * PlayerSpeed;
            }
        }
    }

    void StrafeTank(TankComponentMovementMsg msg)
    {   
        //dir tid, fno
        if(msg.TankID==TankID)
        {
            if(msg.Direction)
            {//strafeleft
                if (Stamina > 0.0f)
                {
                    transform.position -= transform.right * Time.deltaTime * StrafeSpeed;
                    Stamina -= (2.0f + Math.Abs(Stamina / 100f) + .01f);
                }
            }
            else
            {//straferight
                if (Stamina > 0.0f)
                {
                    transform.position += transform.right * Time.deltaTime * StrafeSpeed;
                    Stamina -= (2.0f + Math.Abs(Stamina / 100f) + .01f);
                }
            }
        }
    }


    void CheckMoveTank()
    {
        if(Input.GetKey(Forward))
        {   //forward
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, true);
            OwningGame.SendMessage("MoveTank", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
        if(Input.GetKey(Backward))
        {   //backward
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, false);
            OwningGame.SendMessage("MoveTank", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }

    void CheckTurnTank()
    {
        if (Input.GetKey(Left))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, true);
            OwningGame.SendMessage("TurnTank", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
        if (Input.GetKey(Right))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, true);
            OwningGame.SendMessage("TurnTank", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }

    void CheckStrafeTank()
    {
        if (Input.GetKey(StrafeLeft))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, true);
            OwningGame.SendMessage("StrafeTank",msg,GameUtilities.DONT_CARE_RECIEVER);
        }
        if (Input.GetKey(StrafeRight))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, false);
            OwningGame.SendMessage("StrafeTank", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }

    void MoveTank()
    {   
        CheckMoveTank();
        CheckTurnTank();
        CheckStrafeTank();
    }

    void DamageTank(DamageTankMsg dmsg)
    {
        if(dmsg.TankID==TankID)
        {
            health -= dmsg.Amount;
        }
    }

    void HealStamina()
    {
        if (Stamina > 100.0f)
            Stamina = 100.0f;
        else if (Stamina < 100.0f)
        {
            Stamina += Math.Abs(Stamina / 100f) + .01f;
        }
        if (Stamina < -5.0f)
            Stamina = 0.0f;
        //print(Stamina);
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        CurrentFrame = Time.frameCount;
        //TankComponentMovementMsg FrameNumberAndTankID = new TankComponentMovementMsg(TankID, CurrentFrame);
        //FIXME madbrew : shouldn't send messages each frame, only when move key is pressed. Hack for now.
        //if (Input.GetKey(Left) || Input.GetKey(Right) || Input.GetKey(Forward) 
        //    || Input.GetKey(Backward) || Input.GetKey(StrafeLeft) || Input.GetKey(StrafeRight))
        //    OwningGame.SendMessage("MoveTank", FrameNumberAndTankID, SendMessageOptions.DontRequireReceiver); 
        //MAKE forward and rotate individual events
            //madbrew : what about order on recieving end?
        MoveTank();
        HealStamina();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            // Loads title screen.
            SceneManager.LoadScene("Title", LoadSceneMode.Single);
        }
    }
}