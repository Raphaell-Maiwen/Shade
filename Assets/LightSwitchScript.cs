using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchScript : MonoBehaviour
{

    public GameObject light01;
    public GameObject light02;

    // Start is called before the first frame update
    void Start()
    {
        light01.gameObject.SetActive(false);
        light02.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            light01.gameObject.SetActive(true);
            light02.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            light01.gameObject.SetActive(false);
            light02.gameObject.SetActive(true);
        }
    }
}
