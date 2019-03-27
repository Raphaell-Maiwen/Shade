﻿using System.Collections;
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

    public Animator animator;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("Elliot_IDLE", true);
    }

    // Update is called once per frame
    void Update(){
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            //Jonna : This is where the walking animation plays
            animator.SetBool("Elliot_IDLE", false);
            animator.SetBool("Elliot_WALK", true);
            print("Bouge");
        }
        else {
            //Jonna : this is where the walking animation stops / idle starts
        }

        if (readyToRotate) {
            if (Input.GetKeyDown(KeyCode.R)) {
                objectToRotate.GetComponent<Rotate>().RotateClockwise();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            if (readyToHold) {
                if (objectToHold.transform.parent != null) {
                    objectToHold.GetComponentInParent<MoveObject>().hasAnObjectOn = false;
                }

                objectToHold.transform.SetParent(this.transform);
                readyToHold = false;
                isHolding = true;

                objectToHold.GetComponent<Rigidbody>().useGravity = false;

                objectToBePlacedOn = null;
                readyToPlace = false;
                animator.SetBool("Elliot_CARRY", true);
                //Jonna : this is where the carrying animation starts
            }
            else {
                //Jonna : this is where the carrying animation stops
                isHolding = false;
                PlaceObject();
            }
        }
    }

    private void PlaceObject() {
        if (readyToPlace && objectToHold.GetComponent<MoveObject>().canBeStacked) {
            objectToHold.transform.position = newObjectPos;
            objectToHold.transform.SetParent(objectToBePlacedOn.transform);
            readyToPlace = false;

            objectToBePlacedOn.GetComponentInParent<MoveObject>().hasAnObjectOn = true;
        }
        else {
            objectToHold.transform.SetParent(null);
        }

        objectToHold.GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnTriggerEnter(Collider other){
        GameObject otherGO = other.gameObject;
        
        //if (!readyToRotate && other.gameObject.tag == "RealWorld") {
        if (otherGO.GetComponent<Rotate>() != null && otherGO.GetComponent<Rotate>().rotationCycle.Length > 0) {
            objectToRotate = otherGO;
            readyToRotate = true;
        }
        //}

        MoveObject moveObjectScript = otherGO.GetComponent<MoveObject>();

        if (moveObjectScript != null) {
            if (!isHolding && moveObjectScript.canBeHeld && !moveObjectScript.hasAnObjectOn) {
                print("Ready to hold");
                objectToHold = otherGO;
                readyToHold = true;
            }
            else if (moveObjectScript.isASurface) {
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