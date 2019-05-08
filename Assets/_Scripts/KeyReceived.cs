using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyReceived : MonoBehaviour
{
    public static bool keyReceived = false;
    public Animator keyAnim;

    private void Start()
    {
        keyAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision col)
    {
        print("key collision");
        keyReceived = true;
        keyAnim.SetTrigger("shade_Wins");

    }


}
