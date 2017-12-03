﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using TankMessages;
using EnemyMessages;


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
		case EnemyIDMsgID: //Quite possibly, the best name of any variable ever forever in all time and universes
			OwningGame.BroadcastMessage("DestroyThisEnemy", ReconstructEnemyIDMsg (id_data_pair[1]), GameUtilities.DONT_CARE_RECIEVER);
			break;
		default:
			// No-op?
			break;
		}
	}

	private const int MoveGunVerticalID = 1;
	void MoveGunVertical(TankComponentMovementMsg msg) {
		if (msg.external) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(MoveGunVerticalID, msg));
	}

	private const int MoveGunHorizontalID = MoveGunVerticalID + 1;
	void MoveGunHorizontal(TankComponentMovementMsg msg) {
		if (msg.external) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(MoveGunHorizontalID, msg));
	}

	private const int TurnTankID = MoveGunHorizontalID + 1;
	void TurnTank(TankComponentMovementMsg msg) {
		if (msg.external) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(TurnTankID, msg));
	}

	private const int MoveTankID = TurnTankID + 1;
	void MoveTank(TankComponentMovementMsg msg) {
		if (msg.external) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(MoveTankID, msg));
	}

	private const int StrafeTankID = MoveTankID + 1;
	void StrafeTank(TankComponentMovementMsg msg) {
		if (msg.external) {
			return;
		}
		WebsockAdaptorSend (DeconstructTankComponentMovementMsg(StrafeTankID, msg));
	}
//	void CreateProjectile(CreateProjectileMsg msg)
	private const int CreateProjectileID = StrafeTankID + 1;
	void CreateProjectile(CreateProjectileMsg msg)
	{
//		public bool PlayerFriendly;//whether or not the projectile can harm/affect players
//		public int FrameNo;
//		public byte TankID;
//		public ShotType TypeFired;
//
//		public float xPos;
//		public float yPos;
//		public float zPos;
//
//		public float xQuat;
//		public float yQuat;
//		public float zQuat;
//		public float wQuat;
		if(!msg.external){
		string DeconMsg = "";
			DeconMsg += msg.PlayerFriendly.ToString () + "," + msg.FrameNo.ToString () + "," + msg.TankID.ToString () + "," + ((int)msg.TypeFired).ToString ()
			+ "," + msg.xPos.ToString () + "," + msg.yPos.ToString () + "," + msg.zPos.ToString () + "," + msg.xQuat.ToString () + "," + msg.yQuat.ToString ()
				+ "," + msg.zQuat.ToString () + "," + msg.wQuat.ToString();
			WebsockAdaptorSend (DeconMsg);
		}
	}

	private const int DamageTankID = CreateProjectileID + 1;
	void DamageTank(DamageTankMsg msg)
	{
//		public byte TankID;
//		public int FrameNo;
//		public float Amount;

		if (!msg.external) {
			string DeconMsg = "";
			DeconMsg += msg.Amount.ToString() + "," + msg.FrameNo.ToString() + "," + msg.TankID.ToString();
			WebsockAdaptorSend(DeconMsg);
		}
	}

	private const int DamageEnemyID = DamageTankID + 1;
	void DamageEnemy(DamageEnemyMsg msg)
	{
		//	public EnemyType EType; 
		//	public byte EnemyID;
		//	public byte TankID;//the tank this damage is from
		//	public float Amount;
		if (!msg.external) {
			string DeconMsg = "";
			DeconMsg += ((int)msg.EType).ToString() + "," + msg.EnemyID.ToString() + "," + msg.TankID.ToString() + "," + msg.Amount.ToString();
			WebsockAdaptorSend(DeconMsg);
		}
	}

	private const int EnemyIDMsgID = DamageEnemyID + 1;
	void DestroyThisEnemy(EnemyIDMsg msg)
	{
//		public byte EnemyID;
//		public EnemyType EType; 
		if (!msg.external) {
			string DeconMsg = "";
			DeconMsg += msg.EnemyID.ToString () + "," + ((int)msg.EType).ToString ();
			WebsockAdaptorSend(DeconMsg);
		}
	}

	static EnemyIDMsg ReconstructEnemyIDMsg(string message)
	{
		string[] parts = message.Split (new char[]{','});
		EnemyIDMsg msg = new EnemyIDMsg ();
		msg.external = true;

		msg.EnemyID = byte.Parse (parts [0]);
		int ET = int.Parse (parts [1]);
		msg.EType = (EnemyType)ET;

		return msg;
	}

	static DamageEnemyMsg ReconstructDamageEnemyMsg(string message)
	{
		string[] parts = message.Split (new char[]{','});
		DamageEnemyMsg msg = new DamageEnemyMsg ();
		msg.external = true;
		int ET = int.Parse (parts[0]);
		msg.EType = (EnemyType)ET;
		msg.EnemyID = byte.Parse (parts[1]);
		msg.TankID = byte.Parse (parts [2]);
		msg.Amount = float.Parse (parts [3]);

		return msg;
	}


	static DamageTankMsg ReconstructDamageTankMsg(string message)
	{
		string[] parts = message.Split (new char[]{','});
		DamageTankMsg msg = new DamageTankMsg ();
		msg.external = true;
		msg.Amount = float.Parse (parts[0]);
		msg.FrameNo = int.Parse (parts [1]);
		msg.TankID = byte.Parse (parts [2]);

		return msg;
	}

	static CreateProjectileMsg ReconstructCreateProjectileMsg(string message)
	{
		string[] parts = message.Split (new char[]{','});
		CreateProjectileMsg msg = new CreateProjectileMsg ();
		msg.external = true;
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

	static TankComponentMovementMsg ReconstructTankComponentMovementMsg(string message) {
		string[] parts = message.Split (new char[]{','});
		TankComponentMovementMsg msg = new TankComponentMovementMsg ();
		msg.external = true;
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
}
