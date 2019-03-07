using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour {
    public float speed;
    public float jumpVelocity;

    private bool readyToRotate = false;
    public GameObject objectToRotate;

    // Update is called once per frame
    void Update(){
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)){
            GetComponent<Rigidbody>().velocity = jumpVelocity * Vector3.up;
        }
        else if (readyToRotate) {
            if (Input.GetKeyDown(KeyCode.R)){
                objectToRotate.GetComponent<Rotate>().RotateClockwise();
            }
            else if (Input.GetKeyDown(KeyCode.E)){
                objectToRotate.GetComponent<Rotate>().RotateAntiClockwise();
            }
        }
    }

    private void OnTriggerEnter(Collider other){
        //if (!readyToRotate && other.gameObject.tag == "RealWorld") {
        if (other.gameObject.layer == 9) {
            print("Ready to rotate");
            objectToRotate = other.gameObject;
        }
        //}

        readyToRotate = true;
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (!readyToRotate && other.gameObject.tag == "RealWorld") {
            objectToRotate = other.gameObject;
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        print("Not ready to rotate");
        if (other.gameObject == objectToRotate) {
            readyToRotate = false;
            objectToRotate = null;
        }
    }
}