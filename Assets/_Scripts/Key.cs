using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Animator keyAnim;

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == 13) {
            print("key collision");
            keyAnim.SetTrigger("shade_Wins");
            GameObject.Find("GameManager").GetComponent<GameManager>().NextLevel();
        }
    }
}
