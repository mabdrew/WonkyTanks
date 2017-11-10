using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyMessages;
using TankMessages;

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

    //ideally, this should just be a map from player references to aggro values.
    SortedDictionary<byte, float> TankAggro;
    //map from TankID's to their associated aggro values
    GameObject[] Players;

    GameObject FindPlayerFromID(byte tid)
    {
        foreach (var Player in Players)
        {
            if (tid == Player.GetComponent<TankBody>().TankID)
            {
                return Player;
            }
        }
        return null;
    }

    bool CanFire() { return (Time.frameCount > FrameFired + FireWaitTime); }

    void DamageEnemy(DamageEnemyMsg msg)
    {   //madbrew : I don't think I need to worry about unity doing any behind the scenes multithreading
            //and creating race conditions. If it does, I need to worry about making this thread safe.
        if(msg.EType == EType && msg.EnemyID == EnemyID)
        {
            Health -= msg.Amount;
            float Aggro=0;
            bool FoundPlayerAggro = TankAggro.TryGetValue(msg.TankID, out Aggro);
            if(FoundPlayerAggro)
            {
                print("Found player " + msg.TankID.ToString() + " adding aggro");
                Aggro += msg.Amount * 10;//MAGIC NUMBER
                TankAggro[msg.TankID] = Aggro;
            }
        }
    }

    void AssertAggroLimits()
    {
        foreach (var entry in TankAggro)
        {
            float Aggro = entry.Value;
            if (Aggro > 1000f)
                TankAggro[entry.Key] = 1000f;
            else if (Aggro < 0f)
                TankAggro[entry.Key] = 0f;
        }
    }
    
    GameObject FindMaxAggroPlayer()
    {   
        AssertAggroLimits();
        const float MinimumViableAggro = 3f;
        float MaxAggro = MinimumViableAggro;
        GameObject WorstPlayer = null;
        
        foreach (var entry in TankAggro)
        {
            if(entry.Value>MaxAggro)
            {
                MaxAggro = entry.Value;
                WorstPlayer = FindPlayerFromID(entry.Key);
            }
        }

        return WorstPlayer;
    }

    bool WithInNoticeDistance(GameObject player)
    {
        if (player != null)
        {
            if (player.transform != null)
            {
                Vector3 DistanceVector;

                DistanceVector.x = transform.position.x - TestPlayer.transform.position.x;
                DistanceVector.y = transform.position.y - TestPlayer.transform.position.y;
                DistanceVector.z = transform.position.z - TestPlayer.transform.position.z;

                float Distance = DistanceVector.magnitude;
                return Distance < NoticeDistance;
            }
        }
        return false;
    }

    void PassiveUpdatePlayerAggro()
    {   //look at each player's position, update their aggro according to whether or
        //not they are in range of the guardian
        foreach (var Player in Players)
        {
            float Aggro;
            byte TankID = Player.GetComponent<TankBody>().TankID;
            TankAggro.TryGetValue(TankID, out Aggro);

            if (WithInNoticeDistance(Player))
                Aggro += 10f;
            else
                Aggro -= 5f;

            TankAggro[TankID] = Aggro;
        }        
    }

    void AttackMaxAggroPlayer()
    {   //Attack the max aggro player, if their aggro is greater than 3 (MAGIC NUMBER)
        GameObject MaxAggroPlayer = FindMaxAggroPlayer();

        if (MaxAggroPlayer == null)
            return;

        transform.LookAt(MaxAggroPlayer.transform);
        if ( CanFire() )
        {
            CreateProjectileMsg msg = new CreateProjectileMsg(false, Time.frameCount, 255, ShotType.Bouncy,
                transform.forward*1.5f,transform.forward);
            OwningGame.SendMessage("CreateProjectile", msg, GameUtilities.DONT_CARE_RECIEVER);
            FrameFired = Time.frameCount;
        }

    }

    bool IsDead() { return Health <= 0f; }

    void DestoryThisEnemy(EnemyIDMsg msg)
    {
        if (msg.EType == EType && msg.EnemyID == EnemyID)
            Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {
        GameUtilities.FindGame(ref OwningGame);
        TankAggro = new SortedDictionary<byte, float>();
        Health = 100f;
        FrameFired = Time.frameCount;
        Players = null;
        GameUtilities.GetAllPlayers(ref Players);
        if(Players!=null)
            print("Guardian : found " + Players.Length.ToString() + " players");
	}
	
	// Update is called once per frame
	void Update () {
        if (IsDead())
        {   
            EnemyIDMsg  msg = new EnemyIDMsg(EnemyID, EType);
            OwningGame.BroadcastMessage("DestroyThisEnemy", msg ,GameUtilities.DONT_CARE_RECIEVER);
        }

        PassiveUpdatePlayerAggro();
        AttackMaxAggroPlayer();

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