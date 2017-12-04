using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using TankMessages;
using MapMessages;
using EnemyMessages;
using LevelMessages;

public class WebsockAdaptor : MonoBehaviour {

	[DllImport("__Internal")]
	private static extern void WebsockAdaptorStart();
	[DllImport("__Internal")]
	private static extern void WebsockAdaptorSend(string msg);

	// Use this for initialization
	void Start () {
		WebsockAdaptorStart ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void NewChannel (string name) {
		print ("Added channel:: " + name);
	}

	void RemoveChannel (string name) {
		print ("Removed channel:: " + name);
	}

	void ReceivePacket(string data) {
		GameObject OwningGame = null;
		GameUtilities.FindGame(ref OwningGame);
		string[] id_data_pair = data.Split (new char[]{','}, 2);
		switch (int.Parse(id_data_pair[0])) {
		case MoveGunVerticalID:
			OwningGame.BroadcastMessage ("MoveGunVertical", ReconstructTankComponentMovementMsg (id_data_pair [1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case MoveGunHorizontalID:
			OwningGame.BroadcastMessage ("MoveGunHorizontal", ReconstructTankComponentMovementMsg (id_data_pair [1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case MoveTankID:
			OwningGame.BroadcastMessage ("MoveTank", ReconstructTankComponentMovementMsg (id_data_pair [1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case TurnTankID:
			OwningGame.BroadcastMessage ("TurnTank", ReconstructTankComponentMovementMsg (id_data_pair [1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case StrafeTankID:
			OwningGame.BroadcastMessage ("StrafeTank", ReconstructTankComponentMovementMsg (id_data_pair [1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case CreateProjectileID:
			OwningGame.BroadcastMessage ("CreateProjectile", ReconstructCreateProjectileMsg (id_data_pair [1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case DamageTankID:
			OwningGame.BroadcastMessage ("DamageTank", ReconstructDamageTankMsg (id_data_pair[1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case DamageEnemyID:
			OwningGame.BroadcastMessage ("DamageEnemy", ReconstructDamageEnemyMsg (id_data_pair [1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case DestroyThisEnemyID:
			OwningGame.BroadcastMessage("DestroyThisEnemy", ReconstructEnemyIDMsg (id_data_pair[1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		case SetIsFinishActiveID:
			OwningGame.BroadcastMessage("SetIsFinishActive", ReconstructSetIsFinishActiveMsg(id_data_pair[1]));
			break;
		default:
			// No-op?
			break;
		}
	}

	private const int MoveGunVerticalID = 1;
	void MoveGunVertical(TankComponentMovementMsg msg) {
		if (msg.External) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(MoveGunVerticalID, msg));
	}

	private const int MoveGunHorizontalID = MoveGunVerticalID + 1;
	void MoveGunHorizontal(TankComponentMovementMsg msg) {
		if (msg.External) {
			return;
		}
		print ("Called MoveGunHorizontal on WebsockAdaptor");
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(MoveGunHorizontalID, msg));
	}

	private const int TurnTankID = MoveGunHorizontalID + 1;
	void TurnTank(TankComponentMovementMsg msg) {
		if (msg.External) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(TurnTankID, msg));
	}

	private const int MoveTankID = TurnTankID + 1;
	void MoveTank(TankComponentMovementMsg msg) {
		if (msg.External) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(MoveTankID, msg));
	}

	private const int StrafeTankID = MoveTankID + 1;
	void StrafeTank(TankComponentMovementMsg msg) {
		if (msg.External) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(StrafeTankID, msg));
	}

	static TankComponentMovementMsg ReconstructTankComponentMovementMsg(string message) {
		string[] parts = message.Split (new char[]{','});
		TankComponentMovementMsg msg = new TankComponentMovementMsg ();
		msg.External = true;
		msg.Direction = bool.Parse (parts [0]);
		msg.FrameNo = int.Parse (parts [1]);
		msg.TankID = byte.Parse (parts [2]);
		return msg;
	}

	static string DeconstructTankComponentMovementMsg(int id, TankComponentMovementMsg msg) {
		return
			id
			+ "," + msg.Direction
			+ "," + msg.FrameNo
			+ "," + msg.TankID;
	}

	private const int CreateProjectileID = StrafeTankID + 1;
	void CreateProjectile(CreateProjectileMsg msg)
	{
		if(!msg.External){
			string DeconMsg =
				CreateProjectileID
				+ "," + msg.PlayerFriendly
				+ "," + msg.FrameNo
				+ "," + msg.TankID
				+ "," + ((int)msg.TypeFired)
				+ "," + msg.xPos
				+ "," + msg.yPos
				+ "," + msg.zPos
				+ "," + msg.xQuat
				+ "," + msg.yQuat
				+ "," + msg.zQuat
				+ "," + msg.wQuat;
			WebsockAdaptorSend (DeconMsg);
		}
	}

	static CreateProjectileMsg ReconstructCreateProjectileMsg(string message)
	{
		string[] parts = message.Split (new char[]{','});
		CreateProjectileMsg msg = new CreateProjectileMsg ();
		msg.External = true;
		msg.PlayerFriendly = bool.Parse (parts[0]);
		msg.FrameNo = int.Parse (parts[1]);
		msg.TankID = byte.Parse(parts[2]);
		int TF = int.Parse(parts[3]);
		msg.TypeFired = (ShotType)TF;

		msg.xPos = float.Parse(parts[4]);
		msg.yPos = float.Parse(parts[5]);
		msg.zPos = float.Parse(parts[6]);

		msg.xQuat = float.Parse(parts[7]);
		msg.yQuat = float.Parse(parts[8]);
		msg.zQuat = float.Parse(parts[9]);
		msg.wQuat = float.Parse(parts[10]);

		return msg;
	}

	private const int DamageTankID = CreateProjectileID + 1;
	void DamageTank(DamageTankMsg msg)
	{
		if (!msg.External) {
			string DeconMsg =
				DamageTankID
				+ "," + msg.Amount
				+ "," + msg.FrameNo
				+ "," + msg.TankID;
			WebsockAdaptorSend(DeconMsg);
		}
	}

	static DamageTankMsg ReconstructDamageTankMsg(string message)
	{
		string[] parts = message.Split (new char[]{','});
		DamageTankMsg msg = new DamageTankMsg ();
		msg.External = true;
		msg.Amount = float.Parse (parts[0]);
		msg.FrameNo = int.Parse (parts [1]);
		msg.TankID = byte.Parse (parts [2]);

		return msg;
	}

	private const int DamageEnemyID = DamageTankID + 1;
	void DamageEnemy(DamageEnemyMsg msg)
	{
		if (!msg.External) {
			string DeconMsg =
				DamageEnemyID
				+ "," + ((int)msg.EType)
				+ "," + msg.EnemyID
				+ "," + msg.TankID
				+ "," + msg.Amount;
			WebsockAdaptorSend(DeconMsg);
		}
	}

	static DamageEnemyMsg ReconstructDamageEnemyMsg(string message)
	{
		string[] parts = message.Split (new char[]{','});
		DamageEnemyMsg msg = new DamageEnemyMsg ();
		msg.External = true;
		int ET = int.Parse (parts[0]);
		msg.EType = (EnemyType)ET;
		msg.EnemyID = byte.Parse (parts[1]);
		msg.TankID = byte.Parse (parts [2]);
		msg.Amount = float.Parse (parts [3]);
		return msg;
	}

	private const int DestroyThisEnemyID = DamageEnemyID + 1;
	void DestroyThisEnemy(EnemyIDMsg msg)
	{
		if (!msg.External) {
			string DeconMsg =
				DestroyThisEnemyID
				+ "," + msg.EnemyID
				+ "," + ((int)msg.EType);
			WebsockAdaptorSend(DeconMsg);
		}
	}

	static EnemyIDMsg ReconstructEnemyIDMsg(string message)
	{
		string[] parts = message.Split (new char[]{','});
		EnemyIDMsg msg = new EnemyIDMsg ();
		msg.External = true;

		msg.EnemyID = byte.Parse (parts [0]);
		int ET = int.Parse (parts [1]);

		msg.EType = (EnemyType)ET;
		return msg;
	}

	private const int SetIsFinishActiveID = DestroyThisEnemyID + 1;
	private void SetIsFinishActive(IsFinishActiveMsg msg)
	{
		if (msg.External)
		{
			return;
		}

		string deconstructedMsg =
			SetIsFinishActiveID
			+ "," + msg.isActive;

		WebsockAdaptorSend(deconstructedMsg);
	}

	static IsFinishActiveMsg ReconstructSetIsFinishActiveMsg(string message)
	{

		string[] parts = message.Split(new char[] { ',' });

		IsFinishActiveMsg msg = new IsFinishActiveMsg();
		msg.External = true;
		msg.isActive = bool.Parse( parts[0] );

		return msg;
	}
}
