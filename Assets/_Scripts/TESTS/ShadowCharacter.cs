using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCharacter : MonoBehaviour{
    public float speed;
    public float jumpVelocity;

    bool facingRight = true;

    Rigidbody rb;

    void Awake() {
        rb=  GetComponent<Rigidbody>();
    }

    void Update(){
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);

        if (horizontalInput > 0 && !facingRight) {
            facingRight = true;
        }
        else if(horizontalInput < 0 && facingRight){
            facingRight = false;
        }
        print(facingRight);


        if (Input.GetButtonDown("Jump") && rb.velocity.y < 0.0001f && rb.velocity.y > -0.0001f){
            rb.velocity = jumpVelocity * Vector3.up;
        }
    }
}