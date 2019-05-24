using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public Transform objectAnchor;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isIdle", true);
    }

    // Update is called once per frame
    void Update() {
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

        bool rotating = false;

        //TODO: Refactor this
        if (Input.GetButtonDown("Rotate") && !isHolding) {
            if (readyToRotate) {
                objectToRotate.GetComponent<Rotate>().RotateClockwise();
                rotating = true;
            }
        }
        if (Input.GetButtonDown("Pickup")) {
            print("Pressing E " + readyToHold);
            if (readyToHold && !rotating) {
                TakeObject();
            }
            else {
                animator.SetBool("isHolding", false);

                if (objectToHold != null) {
                    PlaceObject();
                }
            }
        }
    }

    private void TakeObject() {
        objectToHold.GetComponent<MoveObject>().isMoving(true);

        if (objectToHold.transform.parent != null) {
            objectToHold.GetComponentInParent<MoveObject>().hasAnObjectOn = false;
        }

        objectToHold.transform.SetParent(null);
        objectToHold.transform.position = objectAnchor.position;

        objectToHold.transform.SetParent(this.transform);
        readyToHold = false;
        isHolding = true;

        objectToBePlacedOn = null;
        readyToPlace = false;
        animator.SetBool("isHolding", true);

        objectToHold.GetComponent<MoveObject>().SetUI(false);
    }

    private void PlaceObject() {
        print("before");
        objectToHold.GetComponent<Rigidbody>().isKinematic = false;
        print("after");

        objectToHold.GetComponent<MoveObject>().isMoving(false);
        isHolding = false;

        if (readyToPlace && objectToHold.GetComponent<MoveObject>().canBeStacked) {
            objectToHold.transform.position = newObjectPos;
            objectToHold.transform.SetParent(objectToBePlacedOn.transform);
            readyToPlace = false;

            objectToBePlacedOn.GetComponentInParent<MoveObject>().hasAnObjectOn = true;
        }
        else {
            objectToHold.transform.SetParent(null);
        }

        objectToHold.GetComponent<MoveObject>().isFalling = true;
        objectToHold.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;

        readyToHold = true;
    }

    private void OnTriggerEnter(Collider other) {
        GameObject otherGO = other.gameObject;
        bool interactive = false;

        if (otherGO.GetComponent<Rotate>() != null && otherGO.GetComponent<Rotate>().rotationCycle.Length > 0) {

            otherGO.GetComponent<Rotate>().SetUI(true);
            objectToRotate = otherGO;
            readyToRotate = true;
            interactive = true;
        }

        MoveObject moveObjectScript = otherGO.GetComponent<MoveObject>();

        if (moveObjectScript != null) {
            if (!isHolding && moveObjectScript.canBeHeld && !moveObjectScript.hasAnObjectOn) {
                if (objectToHold != null) {
                    objectToHold.GetComponent<MoveObject>().SetUI(false);
                }

                objectToHold = otherGO;
                readyToHold = true;
                interactive = true;
            }
            else if (moveObjectScript.isASurface) {
                if (objectToBePlacedOn != null) {
                    objectToBePlacedOn.GetComponent<MoveObject>().SetUI(false);
                }

                objectToBePlacedOn = otherGO;
                readyToPlace = true;
                newObjectPos = other.transform.position;
                newObjectPos.y += 1;
                interactive = true;
            }

            moveObjectScript.SetUI(true);
        }

    }

    private void OnTriggerExit(Collider other) {
        GameObject otherGO = other.gameObject;

        //print("Not ready to rotate");
        if (otherGO == objectToRotate) {
            readyToRotate = false;
            objectToRotate = null;
            otherGO.GetComponent<Rotate>().SetUI(false);
        }
        if (otherGO == objectToHold) {
            readyToHold = false;
            objectToHold = null;
            otherGO.GetComponent<MoveObject>().SetUI(false);
        }
        if (otherGO == objectToBePlacedOn) {
            readyToPlace = false;
            objectToBePlacedOn = null;
        }

        /*if (otherGO.GetComponent<ParticleSystem>()) {
            otherGO.GetComponent<ParticleSystem>().Clear();
            otherGO.GetComponent<ParticleSystem>().Stop();
        }*/
    }
}