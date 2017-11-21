using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    private float totalTime = 120.0f;

    [SerializeField] private Text TimerText;

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
	}
	
	// Update is called once per frame
	void Update () {

        TotalTime -= Time.deltaTime;

        if(TotalTime <= 0.0f)
        {
            //fail
        }
        else
        {
            TimerText.text = string.Format("{0}:{1:0.00}", (int)(TotalTime / 60), TotalTime % 60);
        }
	}

    private void FailLevel()
    {

    }
}
