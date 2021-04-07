using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardManager : MonoBehaviour {

    public static int mFrags;
    public static int mDeaths;
    public static int mCurrentHP;

    private static Text mScoreBoardText;

    private void Awake()
    {
        // C.W.: Gets the initial text from the Text component
        mScoreBoardText = GetComponent<Text>();

        // C.W.: initialize the statistic with 0
        mFrags = 0;
        mDeaths = 0;
        mCurrentHP = 100;
    }

    void Update () {
        // C.W.: ScoreBoardUpdates are done by specific setters to reduce computational cost
        // and avoid many updates with the same content.
	}

    private static void updateScoreBoard()
    {
        // C.W.: updates the score board on ui
        mScoreBoardText.text = "CBR Statistic \nCurrentHP: " + mCurrentHP + "\nFrags: " + mFrags + "\nDeaths: " + mDeaths;
    }

    public static void increaseFrags()
    {
        mFrags++;
        updateScoreBoard();

    }

    public static void increaseDeaths()
    {
        mDeaths++;
        updateScoreBoard();

    }

    public static void updateCurrentCBRHP(int healthPoints)
    {
        mCurrentHP = healthPoints;
        updateScoreBoard();

    }


}
