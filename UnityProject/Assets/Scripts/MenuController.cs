using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LevelMessages;

public class MenuController : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.anyKey)
		{
			StartGame();
		}
	}

	public void StartGame()
	{
		GameUtilities.Broadcast(
			"LoadNext",
			new LoadNextSceneMsg(SceneName.Level)
		);
	}
}
