using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GameUtilities
{   //broad, useful functions for the game

    public static void FindGame(ref GameObject game_in)
    {   //finds the main game object
        var RootGMs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var GObj in RootGMs)
        {
            if (GObj.CompareTag("Game"))
            {
                game_in = GObj;
                break;
            }
        }
    }

    public const SendMessageOptions DONT_CARE_RECIEVER = SendMessageOptions.DontRequireReceiver;
    public const SendMessageOptions DO_CARE_RECIEVER = SendMessageOptions.RequireReceiver;
}

namespace TankMessages
{
    enum ShotType { InvalidShotType = -1, Bouncy = 0 }

    class TankComponentMovementMsg
    {
        public TankComponentMovementMsg() { TankID = 0; FrameNo = 0; Direction = false; }
        public TankComponentMovementMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

        public bool Direction;
        public byte TankID;
        public int FrameNo;
    };

    class CreateProjectileMsg
    {
        public CreateProjectileMsg()
        {
            PlayerFriendly = false;
            FrameNo = 0;
            TypeFired = ShotType.InvalidShotType;
            InitialPosition = Vector3.zero;
            DirectionReference = Vector3.zero;
        }

        public CreateProjectileMsg(bool pf, int fno, ShotType tf, Vector3 ip, Vector3 dr)
        {
            PlayerFriendly = pf;
            FrameNo = fno;
            TypeFired = tf;
            InitialPosition = ip;
            DirectionReference = dr;
        }

        public bool PlayerFriendly;//whether or not the projectile can harm/affect players
        public int FrameNo;
        public ShotType TypeFired;
        public Vector3 InitialPosition;//starting position of the projectile
        public Vector3 DirectionReference;//A vector to use in conjuction with InitialPosition to create direction of projectile
    }

    class DamageTankMsg
    {
        public DamageTankMsg() { TankID = 0; FrameNo = 0; Amount = 0.0f; }
        public DamageTankMsg(byte tid, int fno, float amt)
        {
            TankID = tid;
            FrameNo = fno;
            Amount = amt;
        }

        public byte TankID;
        public int FrameNo;
        public float Amount; 
    }
}//TankMessages

namespace MapMessages
{
    class GetCollectableMsg
    {
        public GameObject collectableObj;
        //public int FrameNo;

        public GetCollectableMsg(GameObject collectable) { collectableObj = collectable; }
    }
}
