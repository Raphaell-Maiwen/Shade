using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCharacter : MonoBehaviour{
    public float speed;
    public float jumpVelocity;

    void Update(){
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)){
            GetComponent<Rigidbody>().velocity = jumpVelocity * Vector3.up;
        }
    }
}