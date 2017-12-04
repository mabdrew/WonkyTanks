using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MapMessages;

public class CollectableParent : MonoBehaviour {

	[SerializeField] private Text collectableText;
	private string displayedText = "Dongles Left: ";

	void Start() //this is driving me insane
	{
	}


	void UpdateCollectableText(UpdateCollectableTextMsg msg)	{

		collectableText.text = displayedText + msg.collectablesLeft;

	}
}
