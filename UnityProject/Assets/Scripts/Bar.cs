using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIMessages;

public class Bar : MonoBehaviour
{
    //used for both health and stamina bars

    [SerializeField]
    private Image content;

    [SerializeField]
    private int BarID;

    [SerializeField]
    private Text valueText;

    // Use this for initialization
    void Start()
    {
        SetBarValue(100f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleBar(UpdateBar msg)
    {
        if (msg.barID == BarID)
        {
            SetBarValue(msg.currentValue);
            content.fillAmount = Map(msg.currentValue, msg.minValue, msg.maxValue);
        }
        
    }

    private void SetBarValue(float value)
    {
        int result = (((int)(value - 50)) + 50);

        string[] tmp = valueText.text.Split(':');
        valueText.text = string.Format("{0}: {1}", tmp[0], (result - (result % 10) ) );

    }
    private float Map(float current, float min, float max)
    {
        return ( current / (max - min) ); 
    }

}
