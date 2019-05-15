using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenus : MonoBehaviour
{
  

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Menus();

        }  
    }

    public void Menus() {

        SceneManager.LoadScene("Main_Menus");
    }
}
