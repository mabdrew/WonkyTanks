using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TankMessages
{

class MoveTankMsg
{
    public MoveTankMsg() { TankID = 0; FrameNo = 0; }
    public MoveTankMsg(byte tid, int fno) { TankID = tid; FrameNo = fno; }

    public byte TankID;
    public int FrameNo;
};

}