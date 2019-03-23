using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour {
    public float speed;
    public float jumpVelocity;

    private bool readyToRotate = false;
    public GameObject objectToRotate;

    private bool readyToHold = false;
    private bool isHolding = false;
    public GameObject objectToHold;

    private bool readyToPlace = false;
    Vector3 newObjectPos;
    public GameObject objectToBePlacedOn;

    Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y == 0f) {
            rb.velocity = jumpVelocity * Vector3.up;
        }
        else if (readyToRotate) {
            if (Input.GetKeyDown(KeyCode.R)) {
                objectToRotate.GetComponent<Rotate>().RotateClockwise();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            if (readyToHold) {
                objectToHold.transform.SetParent(this.transform);
                readyToHold = false;
                isHolding = true;
            }
            else {
                isHolding = false;
                PlaceObject();
            }
        }
    }

    private void PlaceObject() {
        if (readyToPlace && objectToHold.GetComponent<MoveObject>().canBeStacked) {
            objectToHold.transform.position = newObjectPos;
            objectToHold.transform.SetParent(objectToBePlacedOn.transform);
        }
        else {
            objectToHold.transform.SetParent(null);
        }
    }

    private void OnTriggerEnter(Collider other){
        GameObject otherGO = other.gameObject;
        
        //if (!readyToRotate && other.gameObject.tag == "RealWorld") {
        if (otherGO.GetComponent<Rotate>() != null && otherGO.GetComponent<Rotate>().rotationCycle.Length > 0) {
            print("Ready to rotate");
            objectToRotate = otherGO;
            readyToRotate = true;
        }
        //}

        if (otherGO.GetComponent<MoveObject>() != null) {
            if (!isHolding && otherGO.GetComponent<MoveObject>().canBeHeld) {
                print("Ready to hold");
                objectToHold = otherGO;
                readyToHold = true;
            }
            else if (otherGO.GetComponent<MoveObject>().isASurface) {
                objectToBePlacedOn = otherGO;
                readyToPlace = true;
                newObjectPos = other.transform.position;
                newObjectPos.y += 1;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (!readyToRotate && other.gameObject.tag == "RealWorld") {
            objectToRotate = other.gameObject;
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        //print("Not ready to rotate");
        if (other.gameObject == objectToRotate) {
            readyToRotate = false;
            objectToRotate = null;
        }
        if (other.gameObject == objectToHold) {
            readyToHold = false;
            objectToHold = null;
        }
        if (other.gameObject == objectToBePlacedOn) {
            readyToPlace = false;
            objectToBePlacedOn = null;
        }
    }
}