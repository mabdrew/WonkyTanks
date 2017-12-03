using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapMessages;
using UnityEngine.UI;

public class FinishPortal : MonoBehaviour {

    //private GameObject[] Collectables;
    //private int amountOfCollectables;

    public Text collectableText;

    private int collectablesLeft;

    private GameObject OwningGame;

    // private bool isActive;

    // Use this for initialization
    void Start () {
        CollectablesLeft = GameObject.FindGameObjectsWithTag("Collectable").Length; //total and remaining collectables set to # collectables in game
        GameUtilities.FindGame(ref OwningGame);
    }



    void SetCollectablesLeft()
    {
        CollectablesLeft--;

        if (CollectablesLeft <= 0)
        {
            IsFinishActiveMsg msg = new IsFinishActiveMsg(true);
            OwningGame.BroadcastMessage("SetIsFinishActive", msg, GameUtilities.DONT_CARE_RECIEVER);
        }
    }

    private int CollectablesLeft
    {
        get
        {
            return collectablesLeft;
        }

        set
        {
            collectablesLeft = value;
            collectableText.text = "Dongles Left: " + CollectablesLeft.ToString();
        }
    }
}