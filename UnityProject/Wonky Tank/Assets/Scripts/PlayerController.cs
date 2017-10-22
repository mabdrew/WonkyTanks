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

    public KeyCode Forward;
    public KeyCode Backward;
    public KeyCode Left;
    public KeyCode Right;

    GameObject OwningGame;
    int CurrentFrame; //Q : figure out how to fix framerates. Better way to track frames?
    public byte TankID;

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

        //on start up find your parent game
        var RootGMs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (var GObj in RootGMs)
        {
            if (GObj.CompareTag("Game"))
            {
                print("Found game");
                OwningGame = GObj;
                break;
            }
        }
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

    void MoveTank(MoveTankMsg fno_tid)
    {   //Q : what to do with frame no?
        if (fno_tid.TankID != TankID)
            return;//if your tank id doesn't match, then ignore the message to move
        MoveForward();
        Turn();
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