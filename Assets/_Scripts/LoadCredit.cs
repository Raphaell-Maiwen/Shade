using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCredit : MonoBehaviour
{
    public void Loadcredit() {

        SceneManager.LoadScene("Credits");
        Debug.Log("Scene loaded");
    }
}
