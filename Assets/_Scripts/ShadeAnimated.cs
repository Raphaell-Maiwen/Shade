using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeAnimated : MonoBehaviour {
    public float speed;
    public float jumpVelocity;

    Rigidbody rb;

    public Animator anim;

    public GameObject[] shadeSprite;

    //public float velocity;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        anim = GetComponent<Animator>();
        anim.SetBool("isRunning", false);
    }

    void Update() {
        //velocity = rb.velocity.y;

        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);

        if (Input.GetAxis("Horizontal") != 0) {
            Run(horizontalInput);
        }
        else {
            anim.SetBool("isRunning", false);
        }

        if (Input.GetButtonDown("Jump") && rb.velocity.y < 0.005f && rb.velocity.y > -0.005f) {
            rb.velocity = jumpVelocity * Vector3.up;
            anim.SetTrigger("isJumping");
        }
    }

    public void Run(float horizontalInput) {
        transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
        anim.SetBool("isRunning", true);

        Quaternion rotation = Quaternion.identity;

        if (horizontalInput > 0) {
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else {
            rotation = Quaternion.Euler(0, 180, 0);
        }

        for (int i = 0; i < shadeSprite.Length; i++) {
            shadeSprite[i].transform.rotation = rotation;
        }
    }
}