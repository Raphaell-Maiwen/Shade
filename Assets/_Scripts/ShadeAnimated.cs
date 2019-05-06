using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeAnimated : MonoBehaviour
{
    public float speed;
    public float jumpVelocity;

    Rigidbody rb;

    public Animator anim;

    public GameObject[] shadeSprite;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isRunning", false);
    }

    void Update()
    {
        ManageMovement();

        
    }

    public void ManageMovement()
    {
        if (Input.GetAxis("Horizontal") !=0)
        {
            Run();
        }

        if (Input.GetAxis("Horizontal") == 0)
        {
            anim.SetBool("isRunning", false);
        }

        print(rb.velocity.y);
        if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y < 0.0001f && rb.velocity.y > -0.0001f)
        {
            Jump();
        }
    }

    public void Run()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
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

    public void Jump()
    {
        rb.velocity = jumpVelocity * Vector3.up;
        anim.SetTrigger("isJumping");
    }
}