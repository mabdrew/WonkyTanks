using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankMessages;
using EnemyMessages;

public class GunBody : MonoBehaviour {

    public GameObject OwningTank;
    private GameObject OwningGame;
	private GameObject BulletSpawnLoc;
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
    int FrameFired;
    const int FireFrameWait = 20;

	// Use this for initialization

	void Start () {
        GameUtilities.FindGame(ref OwningGame);
		BulletSpawnLoc = gameObject.transform.Find("BulletSpawn").gameObject;

		if (BulletSpawnLoc != null)
			print ("Found Bullet spawn for player");
		else
			print ("Found Bullet spawn NOT for player");

        Left = KeyCode.H;
        Right = KeyCode.K;

        Up = KeyCode.U;
        Down = KeyCode.J;

        DeviationX = 0;
        DeltaY = 4.0f;
        GunRotateSpeed = 12f;
		FireButton = KeyCode.Space;
        FrameFired = Time.frameCount;
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
        if(Input.GetKey(Right))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(OwningTank.GetComponent<TankBody>().TankID,
                                                            Time.frameCount, true);
            OwningGame.SendMessage("MoveGunHorizontal",msg,GameUtilities.DONT_CARE_RECIEVER);
        }
        if (Input.GetKey(Left))
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

    bool CanFire() { return (Time.frameCount > FrameFired + FireFrameWait); }

    void CheckFireGun()
    {   //for now, it defaults to bouncy. Later should add capability for multiple shot types.
        if (Input.GetKey(FireButton) && CanFire())
        {
			Quaternion qt;
			Vector3 pos;

			if (BulletSpawnLoc != null) {
				qt = BulletSpawnLoc.transform.rotation;
				pos = BulletSpawnLoc.transform.position;
			} 
			else {
				qt = new Quaternion ();
				pos = Vector3.zero;
			}

			CreateProjectileMsg msg = new CreateProjectileMsg(true, Time.frameCount, OwningTank.GetComponent<TankBody>().GetTankID(),
				ShotType.Bouncy,
				pos,qt);
			OwningGame.BroadcastMessage("CreateProjectile", msg, GameUtilities.DONT_CARE_RECIEVER);
			FrameFired = Time.frameCount;
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
        DamageTestEnemyGuardian();
	}

    //functions for testing
    void LocalFireSomethingTest()
    {   //prototype function. For testing purposes only
        if (Input.GetKey(FireButton) && CanFire())
        {
            Vector3 CirclePos = new Vector3();
            CirclePos = transform.position + GunCamera.transform.forward;

            GameObject somecircle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            somecircle.transform.position = CirclePos;
            FrameFired = Time.frameCount;
        }
    }

    public GameObject TestEnemyGuardian;

    public void DamageTestEnemyGuardian()
    {
        
        if(TestEnemyGuardian!=null && Input.GetKey(KeyCode.P) && CanFire())
        {
            print("Attacken thha enermy!");
        //public EnemyType EType; 
        //public byte EnemyID;
        //public byte TankID;//the tank this damage is from
        //public float Amount;
            DamageEnemyMsg msg= new DamageEnemyMsg();
            msg.EnemyID = TestEnemyGuardian.GetComponent<Guardian>().EnemyID;
            msg.TankID = OwningTank.GetComponent<TankBody>().TankID;
            msg.EType = EnemyType.Guardian;
            msg.Amount = 10f;
            OwningGame.BroadcastMessage("DamageEnemy", msg, GameUtilities.DONT_CARE_RECIEVER);
            FrameFired = Time.frameCount;
        }
    }
}
