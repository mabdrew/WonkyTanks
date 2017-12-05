using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using TankMessages;
using MapMessages;
using EnemyMessages;
using LevelMessages;
using UIMessages;

public class WebsockAdaptor : MonoBehaviour {
	public static WebsockAdaptor Instance { get; private set; }

	#if UNITY_WEBGL && !UNITY_EDITOR

	[DllImport("__Internal")]
	private static extern void WebsockAdaptorStart();
	[DllImport("__Internal")]
	private static extern void WebsockAdaptorSend(string msg);

	#else

	private static void WebsockAdaptorStart() {
		// No-op
	}

	private static void WebsockAdaptorSend(string msg) {
		// No-op
	}

	#endif

	void Awake() {
		if (Instance == this) {
			// This should never trigger
			return;
		}
		if (Instance != null) {
			Destroy (this.gameObject);
			return;
		}
		WebsockAdaptorStart ();
		Instance = this;
		DontDestroyOnLoad (gameObject);
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
		string[] data_pair = data.Split (new char[]{','}, 2);
		string descriminator = data_pair[0];
		string payload = data_pair[1];
		switch (int.Parse(descriminator)) {
		case LoadNextID:
			GameUtilities.Broadcast ("LoadNext", ReconstructLoadNextSceneMsg (payload));
			break;
		case MoveGunVerticalID:
			GameUtilities.Broadcast ("MoveGunVertical", ReconstructTankComponentMovementMsg (payload));
			break;
		case MoveGunHorizontalID:
			GameUtilities.Broadcast ("MoveGunHorizontal", ReconstructTankComponentMovementMsg (payload));
			break;
		case MoveTankID:
			GameUtilities.Broadcast ("MoveTank", ReconstructTankComponentMovementMsg (payload));
			break;
		case TurnTankID:
			GameUtilities.Broadcast ("TurnTank", ReconstructTankComponentMovementMsg (payload));
			break;
		case StrafeTankID:
			GameUtilities.Broadcast ("StrafeTank", ReconstructTankComponentMovementMsg (payload));
			break;
		case CreateProjectileID:
			GameUtilities.Broadcast ("CreateProjectile", ReconstructCreateProjectileMsg (payload));
			break;
		case DamageTankID:
			GameUtilities.Broadcast ("DamageTank", ReconstructDamageTankMsg (payload));
			break;
		case DamageEnemyID:
			GameUtilities.Broadcast ("DamageEnemy", ReconstructDamageEnemyMsg (payload));
			break;
		case DestroyThisEnemyID:
			GameUtilities.Broadcast ("DestroyThisEnemy", ReconstructEnemyIDMsg (payload));
			break;
		case SetIsFinishActiveID:
			GameUtilities.Broadcast ("SetIsFinishActive", ReconstructSetIsFinishActiveMsg(payload));
			break;
		case UpdateCollectableTextID:
			GameUtilities.Broadcast ("UpdateCollectableText", ReconstructUpdateCollectableTextMsg (payload));
			break;
		default:
			// No-op?
			break;
		}
	}

	private const int LoadNextID = 1;
	void LoadNext(LoadNextSceneMsg msg)
	{
		if (!msg.External) {
			// We act on this self-fired event!
			// But we don't want to echo it back.
			WebsockAdaptorSend(LoadNextID + "," + ((int) msg.SceneName));
		}
		string sceneName;
		switch (msg.SceneName) {
		case SceneName.Level:
			sceneName = "Level";
			break;
		case SceneName.Title:
		default:
			sceneName = "Title";
			break;
		}
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	LoadNextSceneMsg ReconstructLoadNextSceneMsg(string msg) {
		LoadNextSceneMsg message = new LoadNextSceneMsg ((SceneName)int.Parse (msg));
		message.External = true;
		return message;
	}

	private const int MoveGunVerticalID = LoadNextID + 1;
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

	private const int UpdateCollectableTextID = SetIsFinishActiveID + 1;
	void UpdateCollectableText(UpdateCollectableTextMsg msg)
	{
		if (msg.External)
		{
			return;
		}

		string deconstructedMsg =
			UpdateCollectableTextID
			+ "," + msg.collectablesLeft;

		WebsockAdaptorSend(deconstructedMsg);
	}

	static UpdateCollectableTextMsg ReconstructUpdateCollectableTextMsg(string message)
	{

		string[] parts = message.Split(new char[] { ',' });

		UpdateCollectableTextMsg msg = new UpdateCollectableTextMsg();
		msg.External = true;
		msg.collectablesLeft = int.Parse( parts[0] );

		return msg;
	}

	// ================
	// IGNORED MESSAGES
	// ================
	void HandleBar (UpdateBar msg) {
		// No-op
	}

	void DisableMovement (string msg) {
		// No-op?
	}
}
