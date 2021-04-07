using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundTimeManager : MonoBehaviour {

    public static float mRoundTimeLeft;

    private int minutes;
    private int seconds;
    private string niceTime;
    private Text mRoundTimeText;

    private void Awake()
    {
        // C.W.: Gets the initial text from the Text component
        mRoundTimeText = GetComponent<Text>();
    }
    
    void Update () {

        // C.W.: converts the mRoundTimeLeft from an exact float to 
        // a nice strins
        minutes = Mathf.FloorToInt(mRoundTimeLeft / 60f);
        seconds = Mathf.FloorToInt(mRoundTimeLeft - minutes * 60);
        niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        // C.W.: updates the text on UI
        mRoundTimeText.text = "Time left: " + niceTime ;

        
	}




}
