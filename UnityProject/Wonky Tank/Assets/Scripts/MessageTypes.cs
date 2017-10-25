using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GameUtilities
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

    class MoveTankMsg
    {
        public MoveTankMsg() { TankID = 0; FrameNo = 0; Direction = false; }
        public MoveTankMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

        public bool Direction;
        public byte TankID;
        public int FrameNo;
    };

    class RotateTankMsg
    {
        public RotateTankMsg() { TankID = 0; FrameNo = 0; Direction = false; }
        public RotateTankMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

        public bool Direction;
        public byte TankID;
        public int FrameNo;
    };

    class StrafeTankMsg
    {
        public StrafeTankMsg() { TankID = 0; FrameNo = 0; Direction = false; }
        public StrafeTankMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

        public bool Direction;
        public byte TankID;
        public int FrameNo;
    };

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

    class MoveGunMsg
    {
        public MoveGunMsg() { TankID = 0; FrameNo = 0; }
        public MoveGunMsg(byte tid, int fno) { TankID = tid; FrameNo = fno; }

        public byte TankID;
        public int FrameNo;
    }
}//TankMessages