using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;


public class GunBody : MonoBehaviour {

    public GameObject OwningTank;
    private GameObject OwningGame;
    public Camera GunCamera;

    public KeyCode Left;
    public KeyCode Right;

    public KeyCode Up;
    public KeyCode Down;

    private float GunRotateSpeed;
    private float DeltaY;
    private int DeviationX;
    private const int UpperDeviationLimit = 10;
    private const int LowerDeviationLimit = -2;
    private KeyCode FireButton;

	// Use this for initialization
	void Start () {
        GameUtilities.FindGame(ref OwningGame);
        Left = KeyCode.H;
        Right = KeyCode.K;

        Up = KeyCode.U;
        Down = KeyCode.J;

        DeviationX = 0;
        DeltaY = 6.0f;
        GunRotateSpeed = 20f;
        FireButton = KeyCode.LeftAlt;
	}

    void MoveGunVertical(TankComponentMovementMsg msg)
    {   //
        if (msg.TankID == OwningTank.GetComponent<TankBody>().TankID)
        {
            if (msg.Direction)
            {
                if (DeviationX < UpperDeviationLimit)
                {
                    transform.Rotate(Vector3.right, -5 * GunRotateSpeed * Time.deltaTime);
                    DeviationX++;
                }
            }
            else
            {
                if (DeviationX > LowerDeviationLimit)
                {
                    transform.Rotate(Vector3.right, 5 * GunRotateSpeed * Time.deltaTime);
                    DeviationX--;
                }
            }
        }
    }

    void CheckVerticalMove() 
    {
        if (Input.GetKey(Up))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<TankBody>().TankID,
                                                            Time.frameCount, true);
            OwningGame.SendMessage("MoveGunVertical", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
        if (Input.GetKey(Down))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<TankBody>().TankID,
                                                Time.frameCount, false);
            OwningGame.SendMessage("MoveGunVertical", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }

    void MoveGunHorizontal(TankComponentMovementMsg msg)
    {
        if(msg.TankID==OwningTank.GetComponent<TankBody>().TankID)
        {
            if (msg.Direction)
            {
                Vector3 EAng = transform.rotation.eulerAngles;
                EAng.y += DeltaY;
                transform.rotation = Quaternion.Euler(EAng);
            }
            else
            {   //madbrew : workaround. If I use the Rotate function with Vector3.forward, I get a Z axis rotation that rotates the gun
                    // in a plane created by the x rotation
                Vector3 EAng = transform.rotation.eulerAngles;
                EAng.y -= DeltaY;
                transform.rotation = Quaternion.Euler(EAng);
            }
        }
    }

    void CheckHorizontalMove() 
    {
        if(Input.GetKey(Left))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<TankBody>().TankID,
                                                            Time.frameCount, true);
            OwningGame.SendMessage("MoveGunHorizontal",msg,GameUtilities.DONT_CARE_RECIEVER);
        }
        if (Input.GetKey(Right))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<TankBody>().TankID,
                                                Time.frameCount, false);
            OwningGame.SendMessage("MoveGunHorizontal", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }

    void MoveGun()
    {
        CheckVerticalMove();
        CheckHorizontalMove();
    }

    void LocalFireSomethingTest()
    {   //prototype function. For testing purposes only
        if(Input.GetKey(FireButton)){
            Vector3 CirclePos = new Vector3();
            CirclePos = transform.position + GunCamera.transform.forward;
            
            GameObject somecircle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            somecircle.transform.position = CirclePos;
        }
    }

    void CheckFireGun()
    {   //for now, it defaults to bouncy. Later should add capability for multiple shot types.
        if(Input.GetKey(FireButton))
        {
            CreateProjectileMsg msg = new CreateProjectileMsg(true, Time.frameCount, ShotType.Bouncy,
                transform.position + GunCamera.transform.forward,transform.position);
            OwningGame.SendMessage("CreateProjectile",msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }
	
	// Update is called once per frame
	void Update () {
        //always update its position as per its owning tank. The messaging of this is handled by its owning tank
        Vector3 NewPos = OwningTank.transform.position;
        NewPos.y += 0.5f;//MAGIC NUMBER
		transform.position = NewPos;
        MoveGun();
        CheckFireGun();
        //LocalFireSomethingTest();
	}
}
