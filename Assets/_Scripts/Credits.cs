using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    private bool canSkip = false;
    void Awake()
    {
        Destroy(GameObject.Find("AudioManager"));
        Invoke("EnableSkipping", 10f);
    }

    void Update() {
        if (Input.anyKeyDown && canSkip)
            SceneManager.LoadScene("Main_Menus");

    }

    void EnableSkipping() {
        canSkip = true;
    }
}