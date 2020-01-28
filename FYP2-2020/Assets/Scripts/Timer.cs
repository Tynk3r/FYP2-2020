using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float timeElapsed = 0f;
    void Update()
    {
    }

    public void UpdateScore()
    {
        timeElapsed += Time.deltaTime;
        // score = (int)(time elapsed) + (int)(number of blocks broken * 100)

        //set timer UI
        timerText.text = "Score: " + ((int)timeElapsed + (GameController.instance.blocksBroken * 10)).ToString();
    }
}
