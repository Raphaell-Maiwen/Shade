using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour {
    public float speed;
    public float jumpVelocity;

    private bool readyToRotate = false;
    public GameObject objectToRotate;

    [SerializeField]
    private bool readyToHold = false;

    private bool isHolding = false;
    public GameObject objectToHold;

    private bool readyToPlace = false;
    Vector3 newObjectPos;
    public GameObject objectToBePlacedOn;

    private Vector3 inputVector;

    Rigidbody rb;

    public Animator animator;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isIdle", true);
    }

    // Update is called once per frame
    void Update(){
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.position += inputVector.normalized * speed * Time.deltaTime;


        //Todo: Change GetAxis to inputVector
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector), 100 * Time.deltaTime);
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
        else {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }

        if (Input.GetButtonDown("Rotate")) {
            if (readyToRotate) {
                objectToRotate.GetComponent<Rotate>().RotateClockwise();
            }
        }
        else if (Input.GetButtonDown("Pickup")) {
            print("Pressing E " + readyToHold);
            if (readyToHold) {
                if (objectToHold.transform.parent != null) {
                    objectToHold.GetComponentInParent<MoveObject>().hasAnObjectOn = false;
                }

                print("Starting to hold");
                objectToHold.transform.SetParent(this.transform);
                readyToHold = false;
                isHolding = true;

                objectToHold.GetComponent<Rigidbody>().useGravity = false;

                objectToBePlacedOn = null;
                readyToPlace = false;
                animator.SetBool("isHolding", true);
            }
            else {
                animator.SetBool("isHolding", false);
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
        bool interactive = false;
        
        //if (!readyToRotate && other.gameObject.tag == "RealWorld") {
        if (otherGO.GetComponent<Rotate>() != null && otherGO.GetComponent<Rotate>().rotationCycle.Length > 0) {
            objectToRotate = otherGO;
            readyToRotate = true;
            interactive = true;
        }
        //}

        MoveObject moveObjectScript = otherGO.GetComponent<MoveObject>();

        if (moveObjectScript != null) {
            if (!isHolding && moveObjectScript.canBeHeld && !moveObjectScript.hasAnObjectOn) {
                print("Ready to hold");
                objectToHold = otherGO;
                readyToHold = true;
                interactive = true;
            }
            else if (moveObjectScript.isASurface) {
                objectToBePlacedOn = otherGO;
                readyToPlace = true;
                newObjectPos = other.transform.position;
                newObjectPos.y += 1;
                interactive = true;
            }
        }

        if (interactive && otherGO.GetComponent("Halo")) {
            Behaviour halo = (Behaviour)otherGO.GetComponent("Halo");
            halo.enabled = true;
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
        GameObject otherGO = other.gameObject;

        //print("Not ready to rotate");
        if (otherGO == objectToRotate) {
            readyToRotate = false;
            objectToRotate = null;
        }
        if (otherGO == objectToHold) {
            readyToHold = false;
            objectToHold = null;
            print("Not ready to hold");
        }
        if (otherGO == objectToBePlacedOn) {
            readyToPlace = false;
            objectToBePlacedOn = null;
        }

        if (otherGO.GetComponent("Halo")) {
            Behaviour halo = (Behaviour)otherGO.GetComponent("Halo");
            halo.enabled = false;
        }
    }
}