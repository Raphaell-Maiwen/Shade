using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCharacter : MonoBehaviour{
    public float speed;
    public float jumpVelocity;

    Rigidbody rb;

    void Awake() {
        rb=  GetComponent<Rigidbody>();
    }

    void Update(){
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y == 0f){
            rb.velocity = jumpVelocity * Vector3.up;
        }
    }
}