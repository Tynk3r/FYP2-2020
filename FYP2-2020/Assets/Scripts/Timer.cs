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

    public void UpdateTimerText()
    {
        //set timer UI
        timeElapsed += Time.deltaTime;
        timerText.text = timeElapsed.ToString();
    }
}
