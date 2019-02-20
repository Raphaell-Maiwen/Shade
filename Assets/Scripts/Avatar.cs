using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour {
    public float speed;
    public float jumpVelocity;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().velocity = jumpVelocity * Vector3.up;
        }
    }
}