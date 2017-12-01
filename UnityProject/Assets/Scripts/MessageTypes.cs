using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameUtilities : MonoBehaviour//extends mono purely for the benefits of printing to the game console
{   //broad, useful functions and data for the game

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

    public static void GetAllPlayers(ref GameObject[] players_in)
    {
        GameObject Game = null;
        FindGame(ref Game);

        if(Game!=null)
        {
            GameObject Tanks = Game.transform.Find("Tanks").gameObject;

            int NoChildren = Tanks.transform.childCount;
            players_in = new GameObject[NoChildren];
            for(int iChild=0;iChild<NoChildren;iChild++)
            {
                players_in[iChild] = Tanks.transform.GetChild(iChild).gameObject.transform.Find("TankBody").gameObject;
            }
        }
    }

    public const SendMessageOptions DONT_CARE_RECIEVER = SendMessageOptions.DontRequireReceiver;
    public const SendMessageOptions DO_CARE_RECIEVER = SendMessageOptions.RequireReceiver;

    public const byte INVALID_TANK_ID = 255;
    public const byte INVALID_ENEMY_ID = INVALID_TANK_ID;
}

namespace TankMessages
{
    enum ShotType { InvalidShotType = -1, Bouncy = 0 }

    class TankComponentMovementMsg
    {
        public TankComponentMovementMsg() { TankID = GameUtilities.INVALID_TANK_ID; FrameNo = 0; Direction = false; }
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
            TankID = GameUtilities.INVALID_TANK_ID;
            TypeFired = ShotType.InvalidShotType;
			xPos = yPos = zPos = xQuat = yQuat = zQuat = wQuat = 0f;
        }

		public CreateProjectileMsg(bool pf, int fno, byte tid, ShotType tf, Vector3 ip, Quaternion rot)
        {
            PlayerFriendly = pf;
            FrameNo = fno;
            TankID = tid;
            TypeFired = tf;
            
			xPos = ip.x;
			yPos = ip.y;
			zPos = ip.z;

			xQuat = rot.x;
			yQuat = rot.y;
			zQuat = rot.z;
			wQuat = rot.w;
        }

        public bool PlayerFriendly;//whether or not the projectile can harm/affect players
        public int FrameNo;
        public byte TankID;
        public ShotType TypeFired;

		public float xPos;
		public float yPos;
		public float zPos;

		public float xQuat;
		public float yQuat;
		public float zQuat;
		public float wQuat;
    }

    class DamageTankMsg
    {
        public DamageTankMsg() { TankID = GameUtilities.INVALID_TANK_ID; FrameNo = 0; Amount = 0.0f; }
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
	class GetBulletMsg
    {
        public GameObject bulletObj;
        //public int FrameNo;

        public GetBulletMsg(GameObject bullet) { bulletObj = bullet; }
    }
}

namespace LevelMessages
{
    class LoadNextSceneMsg
    {
        public string SceneName;
        public LoadSceneMode SceneModeType;

        public LoadNextSceneMsg()
        {
            SceneName = "Title";
            SceneModeType = LoadSceneMode.Single;
        }

        public LoadNextSceneMsg(string nextScene, LoadSceneMode nextSceneType)
        {
            SceneName = nextScene;
            SceneModeType = nextSceneType;
        }
    }
}

namespace EnemyMessages
{
    public enum EnemyType { InvalidEnemyType = -1, Guardian = 0, Chaser = 1 }

    class DamageEnemyMsg
    {
        public DamageEnemyMsg()
        {
            EType = EnemyType.InvalidEnemyType;
            EnemyID = GameUtilities.INVALID_ENEMY_ID;
            Amount = 0f;
        }

        public DamageEnemyMsg(EnemyType et, byte eid, byte tid, float amt)
        {
            EType = et;
            EnemyID = eid;
            TankID = tid;
            Amount = amt;
        }

        public EnemyType EType; 
        public byte EnemyID;
        public byte TankID;//the tank this damage is from
        public float Amount;
    }

    class EnemyIDMsg
    {
        public EnemyIDMsg()
        {
            EnemyID = GameUtilities.INVALID_ENEMY_ID;
            EType = EnemyType.InvalidEnemyType;
        }

        public EnemyIDMsg(byte eid, EnemyType et)
        {
            EnemyID = eid;
            EType = et;
        }

        public byte EnemyID;
        public EnemyType EType; 
    }
}

namespace UIMessages
{
    class UpdateBar
    {
        public float currentValue;
        public float minValue;
        public float maxValue;
        public int barID;

        public UpdateBar()
        {
            currentValue = 100.0f;
            minValue = 0.0f;
            maxValue = 100.0f;
            barID = 0;
        }

        public UpdateBar(float current, float min, float max, int id)
        {
            currentValue = current;
            minValue = min;
            maxValue = max;
            barID = id;
        }
    }
}