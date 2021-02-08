using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Start_Page_UI : MonoBehaviour
{
    Button startButton, exitButton;


    public void Awake()
    {
        startButton = GameObject.Find("Start").GetComponent<Button>();
        startButton.onClick.AddListener(SceneSwitch);
    }

    private void SceneSwitch() { SceneManager.LoadScene("MainScene", LoadSceneMode.Single); }

}
