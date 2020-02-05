using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI timerText;

    private int prevScore;
    private float timeElapsed = 0f;
    void Update()
    {
    }

    public void UpdateScore()
    {
        score = (int)timeElapsed + (GameController.instance.blocksBroken * 10);
        timeElapsed += Time.deltaTime;

        //set timer UI
        timerText.SetText("Score: " + score.ToString());
    }

    public int ResetScore()
    {
        prevScore = score;
        timeElapsed = 0f;
        score = 0;
        return prevScore;
    }
}
