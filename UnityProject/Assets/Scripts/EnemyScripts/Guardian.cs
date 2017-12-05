using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyMessages;
using TankMessages;

public class Guardian : MonoBehaviour {

    GameObject OwningGame;
	GameObject BulletSpawnLoc;

    const float NoticeDistance = 7f;
    const float MaxAggro = 10000f;
    const float MinAggro = 0;
    const float DamageToAggroCoefficient = 20f;
    const float MinimumViableAggro = 3f;
    const float PassiveAggroUpTick = 10f;
    const float PassiveAggroDownTick = 5f;

    private const EnemyType EType = EnemyType.Guardian;
    public byte EnemyID;
    float Health;

    int FrameFired;
    const int FireFrameWait = 15;

	public EnemyType GetEnemyType()
	{
		return EType;
	}

    //ideally, this should just be a map from player references to aggro values.
    SortedDictionary<byte, float> TankAggro;
    //map from TankID's to their associated aggro values
    GameObject[] Players;

    GameObject FindPlayerFromID(byte tid)
    {
        foreach (var Player in Players)
            if (tid == Player.GetComponent<TankBody>().TankID)
                return Player;

        return null;
    }

    bool CanFire() { return (Time.frameCount > FrameFired + FireFrameWait); }

    void DamageEnemy(DamageEnemyMsg msg)
    {   
        if(msg.EType == EType && msg.EnemyID == EnemyID)
        {
            Health -= msg.Amount;
            float Aggro = 0;
            bool FoundPlayerAggro = TankAggro.TryGetValue(msg.TankID, out Aggro);
            if(FoundPlayerAggro)
            {
//                print("Found player " + msg.TankID.ToString() + " adding aggro");
                Aggro += msg.Amount * DamageToAggroCoefficient;
                TankAggro[msg.TankID] = Aggro;
            }
        }
    }

    void AssertAggroLimits()
    {
        foreach (var entry in TankAggro)
        {
            float Aggro = entry.Value;
            if (Aggro > MaxAggro)
                TankAggro[entry.Key] = MaxAggro;
            else if (Aggro < MinAggro)
                TankAggro[entry.Key] = MinAggro;
        }
    }
    
    GameObject FindMaxAggroPlayer()
    {   
        AssertAggroLimits();
        float RunningMaxAggro = MinimumViableAggro;
        GameObject WorstPlayer = null;
        
        foreach (var entry in TankAggro)
        {
            if (entry.Value > RunningMaxAggro)
            {
                RunningMaxAggro = entry.Value;
                WorstPlayer = FindPlayerFromID(entry.Key);
//                print(RunningMaxAggro);
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

                DistanceVector.x = transform.position.x - player.transform.position.x;
                DistanceVector.y = transform.position.y - player.transform.position.y;
                DistanceVector.z = transform.position.z - player.transform.position.z;

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
                Aggro += PassiveAggroUpTick;
            else
                Aggro -= PassiveAggroDownTick;

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

            CreateProjectileMsg msg = new CreateProjectileMsg(
                false,
                Time.frameCount,
                GameUtilities.INVALID_TANK_ID,
                ShotType.Bouncy,
                pos,
                qt
            );
            GameUtilities.Broadcast ("CreateProjectile", msg);
            FrameFired = Time.frameCount;
        }

    }

    bool IsDead() 
	{
		//print("Guardian Health = " + Health.ToString()); 
		return Health <= 0f; 
	}

    void DestroyThisEnemy(EnemyIDMsg msg)
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
		BulletSpawnLoc = gameObject.transform.Find ("BulletSpawn").gameObject;
		if (BulletSpawnLoc != null)
			print ("Guardian Found Bullet Spawn");
		else
			print ("Guardian DID NOT Find Bullet Spawn");
	}
	
	// Update is called once per frame
	void Update () {
        if (IsDead())
        {   
            EnemyIDMsg  msg = new EnemyIDMsg(EnemyID, EType);
            GameUtilities.Broadcast ("DestroyThisEnemy", msg);
        }

        PassiveUpdatePlayerAggro();
        AttackMaxAggroPlayer();

		//LookAtTestPlayer();
	}

    //testing functions. Mostly a reference for making message passing functions.
        //Will likely be removed later on.

    public GameObject TestPlayer;//for testing purposes only

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