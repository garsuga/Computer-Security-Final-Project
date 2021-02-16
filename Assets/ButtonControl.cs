using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    void Start()
    {
        Button exit_button = GameObject.Find("ExitButton").GetComponent<Button>();
        exit_button.onClick.AddListener(LoadMainMenuScene);

        Button restart_button = GameObject.Find("RestartButton").GetComponent<Button>();
        restart_button.onClick.AddListener(LoadMainScene);
    }

    void LoadMainMenuScene() { SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single); }
    void LoadMainScene() { SceneManager.LoadScene("MainScene", LoadSceneMode.Single); }
}
