using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using TankMessages;


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
			OwningGame.BroadcastMessage ("MoveGunVertical", ReconstructTankComponentMovementMsg (id_data_pair [1]));
			break;
		case MoveGunHorizontalID:
			OwningGame.BroadcastMessage ("MoveGunHorizontal", ReconstructTankComponentMovementMsg (id_data_pair [1]));
			break;
		case MoveTankID:
			OwningGame.BroadcastMessage ("MoveTank", ReconstructTankComponentMovementMsg (id_data_pair [1]));
			break;
		case TurnTankID:
			OwningGame.BroadcastMessage ("TurnTank", ReconstructTankComponentMovementMsg (id_data_pair [1]));
			break;
		case StrafeTankID:
			OwningGame.BroadcastMessage ("StrafeTank", ReconstructTankComponentMovementMsg (id_data_pair [1]));
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
