using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button newGame;
    public Button credits;
    public Button quit;

    void Awake()
    {
        newGame.onClick.AddListener(StartNewGame);
        credits.onClick.AddListener(ShowCredits);
        quit.onClick.AddListener(QuitGame);
    }

    void StartNewGame() {
        SceneManager.LoadScene("Tutorial_Level_01");
    }

    void ShowCredits() {
        //SceneManager.LoadScene("Credits");
    }

    void QuitGame() {
        print("Quit Game");
        Application.Quit();
    }
}