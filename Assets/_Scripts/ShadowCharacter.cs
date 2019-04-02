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

        print(rb.velocity.y);
        if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y < 0.0001f && rb.velocity.y > -0.0001f){
            rb.velocity = jumpVelocity * Vector3.up;
        }
    }
}