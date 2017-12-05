using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LevelMessages;

public class Timer : MonoBehaviour {

    [SerializeField] private float totalTime = 120.0f;

    [SerializeField] private Text TimerText;
    [SerializeField] private Text FailureText;

    private TankBody tankBody;

    private GameObject OwningGame;

    public float TotalTime
    {
        get
        {
            return totalTime;
        }

        set
        {
            totalTime = value;
        }
    }

    // Use this for initialization
    void Start () {
        GameUtilities.FindGame(ref OwningGame);
    }
    
    // Update is called once per frame
    void Update () {

        TotalTime -= Time.deltaTime;

        if(TotalTime <= 0.0f)
        {
            FailLevel();
        }
        else
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        TimerText.text = string.Format("{0}:{1:00.00}", (int)(TotalTime / 60), TotalTime % 60);
    }

    private void FailLevel()
    {
        GameUtilities.Broadcast ("DisableMovement", "");
        TimerText.text = "RIP";
        FailureText.gameObject.SetActive(true);
        Invoke("EndLevel", 5.0f);

    }

    private void EndLevel()
    {
        GameUtilities.Broadcast (
            "LoadNext",
            new LoadNextSceneMsg(SceneName.Title)
        );
    }
}
