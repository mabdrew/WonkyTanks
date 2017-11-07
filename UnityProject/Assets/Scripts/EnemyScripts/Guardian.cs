using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyMessages;

public class Guardian : MonoBehaviour {

    GameObject OwningGame;

    public GameObject TestPlayer;//for testing purposes only
    const float NoticeDistance = 5f;
    const float MaxAggro = 1000f;
    const float MinAggro = 0;
    private const EnemyType EType = EnemyType.Guardian;
    public byte EnemyID;
    float Health;

    int FrameFired;
    const int FireWaitTime = 5;

    SortedDictionary<int, float> TankAggro;
    //map from TankID's to their associated aggro values
    GameObject[] players;

    bool CanFire() { return (Time.frameCount > FrameFired + FireWaitTime); }

    void DamageEnemy(DamageEnemyMsg msg)
    {   //madbrew : I don't think I need to worry about unity doing any behind the scenes multithreading
            //and creating race conditions. If it does, I need to worry about making this thread safe.
        if(msg.EType == EType && msg.EnemyID == EnemyID)
        {
            Health -= msg.Amount;
            //use the TankID from the message to increase associated player's aggro
        }
    }

    void AssertAggroLimits()
    {
        //for each player, assert aggro limits
    }
    
    void AddAggro(float amt, byte tid)
    {

    }

    void ReduceAggro(float amt, byte tid)
    {

    }

    void FindMaxAggroPlayer()
    {   //needs to send a message for projectile only
        //find max aggro player. If their aggro > 1, look at then fire a projectile in their direction 
    
        
    //example code from player gun *firing* a message off    
    //    void CheckFireGun()
    //{   //for now, it defaults to bouncy. Later should add capability for multiple shot types.
    //    if(Input.GetKey(FireButton))
    //    {
    //        CreateProjectileMsg msg = new CreateProjectileMsg(true, Time.frameCount,
    //            OwningTank.GetComponent<TankBody>().TankID,
    //            ShotType.Bouncy,
    //            transform.position + GunCamera.transform.forward,transform.position);
    //        OwningGame.SendMessage("CreateProjectile",msg, GameUtilities.DONT_CARE_RECIEVER);
    //    }
    //}
    
    }

    bool IsDead()
    {
        return Health <= 0f;
    }

    void DestoryThisEnemy(EnemyIDMsg msg)
    {
        if (msg.EType == EType && msg.EnemyID == EnemyID)
            Destroy(gameObject);
    }



	// Use this for initialization
	void Start () {
        GameUtilities.FindGame(ref OwningGame);
        TankAggro = new SortedDictionary<int, float>();
        Health = 100f;
        FrameFired = Time.frameCount;
	}
	
	// Update is called once per frame
	void Update () {
        if (IsDead())
        {   
            EnemyIDMsg  msg = new EnemyIDMsg(EnemyID, EType);
            OwningGame.BroadcastMessage("DestroyThisEnemy", msg ,GameUtilities.DONT_CARE_RECIEVER);
        }

		//LookAtTestPlayer();
	}

    //testing functions. Mostly a reference for making message passing functions.
        //Will likely be removed later on.
    void LookAtTestPlayer()
    {
        if (TestPlayer != null)
        {
            if (TestPlayer.transform != null)
            {
                Vector3 DistanceVector;

                DistanceVector.x = Mathf.Abs(transform.position.x - TestPlayer.transform.position.x);
                DistanceVector.y = Mathf.Abs(transform.position.y - TestPlayer.transform.position.y);
                DistanceVector.z = Mathf.Abs(transform.position.z - TestPlayer.transform.position.z);

                float Distance = DistanceVector.magnitude;
                print("Distance = " + Distance.ToString());
                print("DistanceVector = " + DistanceVector.ToString());
                if (Distance < NoticeDistance)
                {
                    transform.LookAt(TestPlayer.transform);
                    print("looky!");
                }
            }
        }
    }
}