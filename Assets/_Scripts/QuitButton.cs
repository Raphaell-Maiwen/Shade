using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Should Quit Game");

    }
}
