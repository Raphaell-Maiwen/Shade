using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControls : MonoBehaviour {

    void Update()
    {
        if (Input.GetButtonDown("A"))
        {
            Debug.Log("A");
        }

        if (Input.GetButtonDown("B"))
        {
            Debug.Log("B");
        }

        if (Input.GetButtonDown("X"))
        {
            Debug.Log("X");
        }

        if (Input.GetButtonDown("Y"))
        {
            Debug.Log("Y");
        }

        if (Input.GetAxis("Trigger_L") != 0)
        {
            Debug.Log("L Trigger");
        }

        if (Input.GetAxis("Trigger_R") != 0)
        {
            Debug.Log("R Trigger");
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            Debug.Log("Horizontal");
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            Debug.Log("Vertical");
        }

        if (Input.GetAxis("RightStickX") != 0)
        {
            Debug.Log("Right Stick X");
        }

        if (Input.GetAxis("RightStickY") != 0)
        {
            Debug.Log("Right Stick Y");
        }
    }
}
