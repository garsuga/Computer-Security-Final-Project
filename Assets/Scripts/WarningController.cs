using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningController : MonoBehaviour
{
    public TextMeshProUGUI warningMessageText;
    public CanvasGroup warningCanvas;

    private float deathTime = float.MaxValue;
    private float birthTime = 0;

    public WarningController SetWarningText(string message)
    {
        warningMessageText.text = message;
        return this;
    }

    public WarningController FadeIn(float seconds)
    {
        deathTime = Time.time + seconds;
        birthTime = Time.time;
        return this;
    }

    private void Update()
    {
        warningCanvas.alpha = Mathf.Lerp(birthTime, deathTime, Time.time);
        if (Time.time >= deathTime)
            Destroy(this.gameObject);
    }
}
