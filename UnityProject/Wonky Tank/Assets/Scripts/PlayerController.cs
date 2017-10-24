using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.Time;
using TankMessages;


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

    void Turn() //rotate Left or Right
    {
        if (Input.GetKey(Left) || Input.GetKey(Right))
        {
            transform.Rotate(0.0f, Input.GetAxis("Horizontal") * RotateSpeed, 0.0f);
        }
    }

    void MoveForward()
    {   
        //move Forward
        if (Input.GetKey(Forward))
        {
            transform.position += transform.forward * Time.deltaTime * PlayerSpeed;
            Turn(); //having a call to turn here enables turning while moving
        }

        //move Backward
        else if (Input.GetKey(Backward))
        {
            transform.position -= transform.forward * Time.deltaTime * PlayerSpeed;
            Turn(); //enable turning while moving Backward
        }
    }

    void Strafe()
    {   
        if(Input.GetKey(StrafeRight) || Input.GetKey(StrafeLeft))
            if(Stamina>0.0f)
                Stamina -= 3.0f;
        if(Input.GetKey(StrafeLeft))
        {
            if(Stamina>0.0f)
                transform.position -= transform.right * Time.deltaTime * StrafeSpeed;
        }
        if(Input.GetKey(StrafeRight))
        {
            if(Stamina>0.0f)
                transform.position += transform.right * Time.deltaTime * StrafeSpeed;
        }
        if(!Input.GetKey(StrafeRight) && !Input.GetKey(StrafeRight))
        {
            if (Stamina > 100.0f)
                Stamina = 100.0f;
            else if (Stamina<100.0f)
            {
                Stamina += .125f;
            }
        }
    }

    void MoveTank(MoveTankMsg fno_tid)
    {   //Q : what to do with frame no?
        if (fno_tid.TankID != TankID)
            return;//if your tank id doesn't match, then ignore the message to move
        MoveForward();
        Turn();
        Strafe();
    }

    void DamageTank(DamageTankMsg dmsg)
    {
        if(dmsg.TankID==TankID)
        {
            health -= dmsg.Amount;
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        CurrentFrame = Time.frameCount;
        MoveTankMsg FrameNumberAndTankID = new MoveTankMsg(TankID, CurrentFrame);
        OwningGame.SendMessage("MoveTank", FrameNumberAndTankID, SendMessageOptions.DontRequireReceiver); 
        //MAKE forward and rotate individual events
            //madbrew : what about order on recieving end?
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