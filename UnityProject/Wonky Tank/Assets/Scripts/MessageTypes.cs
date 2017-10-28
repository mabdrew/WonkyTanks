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
    //madbrew : refactor into a single message type. A lot of these have identical data
    class TankComponentMovementMsg
    {
        public TankComponentMovementMsg() { TankID = 0; FrameNo = 0; Direction = false; }
        public TankComponentMovementMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

        public bool Direction;
        public byte TankID;
        public int FrameNo;
    };

    //class RotateTankMsg
    //{
    //    public RotateTankMsg() { TankID = 0; FrameNo = 0; Direction = false; }
    //    public RotateTankMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

    //    public bool Direction;
    //    public byte TankID;
    //    public int FrameNo;
    //};

    //class StrafeTankMsg
    //{
    //    public StrafeTankMsg() { TankID = 0; FrameNo = 0; Direction = false; }
    //    public StrafeTankMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

    //    public bool Direction;
    //    public byte TankID;
    //    public int FrameNo;
    //};

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

    //class VertGunRotationMsg
    //{
    //    public VertGunRotationMsg() { TankID = 0; FrameNo = 0; Direction = false; }
    //    public VertGunRotationMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

    //    public bool Direction;
    //    public byte TankID;
    //    public int FrameNo;
    //}

    //class HorzGunRotationMsg
    //{
    //    public HorzGunRotationMsg() { TankID = 0; FrameNo = 0; Direction = false; }
    //    public HorzGunRotationMsg(byte tid, int fno, bool direction) { TankID = tid; FrameNo = fno; Direction = direction; }

    //    public bool Direction;
    //    public byte TankID;
    //    public int FrameNo;
    //}
}//TankMessages