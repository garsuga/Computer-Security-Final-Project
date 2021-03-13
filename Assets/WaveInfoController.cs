using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveInfoController : MonoBehaviour
{
    public Text waveNumText;
    public Text waveTimerText;

    private bool nextWaveTimerActive = false;
    private float nextWaveTimerLength;
    private float nextWaveTimerStart;

    public int WaveNum {
        set
        {
            waveNumText.text = "Wave " + value.ToString();
        }
    }

    public void StartTimer(float length)
    {
        nextWaveTimerActive = true;
        nextWaveTimerLength = length;
        nextWaveTimerStart = Time.realtimeSinceStartup;
        waveTimerText.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nextWaveTimerActive)
        {
            if (Time.realtimeSinceStartup > nextWaveTimerStart + nextWaveTimerLength)
            {
                nextWaveTimerActive = false;
                waveTimerText.enabled = false;
            } else
            {
                int wholeSecondsLeft = Mathf.CeilToInt(nextWaveTimerStart + nextWaveTimerLength - Time.realtimeSinceStartup);
                waveTimerText.text = "Next wave in " + wholeSecondsLeft + "s";
            }
        }
    }
}
