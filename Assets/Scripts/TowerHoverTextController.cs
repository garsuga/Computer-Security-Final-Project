using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerHoverTextController : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI titleText;

    public void SetDescriptionText(string text)
    {
        descriptionText.text = text;
    }

    public void SetTitleText(string text)
    {
        titleText.text = text;
    }
}
