using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar_Update : MonoBehaviour
{
    // Movement

    CharacterController elliot;
    public float horizontal;
    public float vertical;
    public float movementSpeed;
    public float rotateSpeed;
    Vector3 movement;
    Vector3 lastMovementMade;
    private Vector3 moveDirection = Vector3.zero;
    public float speed = 4;
    public float rotSpeed = 200;
    float rot = 0f;
    float gravity = 8;
    Vector3 moveDir = Vector3.zero;

    // Animation
    public bool isWalking;
    Animator anim;

    // Pickups

    private bool readyToRotate = false;
    public GameObject objectToRotate;

    private bool readyToHold = false;
    private bool isHolding = false;
    public GameObject objectToHold;

    private bool readyToPlace = false;
    Vector3 newObjectPos;
    public GameObject objectToBePlacedOn;

    // ------------------------------------------------------------------- //

    void Start()
    {
        elliot = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        ManageMovement();
        ManagePickups();
        CheckWin();
    }

    void CheckWin()
    {
        if (KeyReceived.keyReceived == true)

        {
            print("shade wins?");
            anim.SetTrigger("shade_Wins");
            KeyReceived.keyReceived = false;
        }
    }

    void ManageMovement()
    {
        horizontal = Input.GetAxis("Horizontal"); 
        vertical = Input.GetAxis("Vertical");      

        if (Input.GetAxis("Horizontal") != 0)
        {
            isWalking = true;
            Walk();
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            isWalking = true;
            Walk();
        }

        if ((isWalking == true) && (Input.GetAxis("Horizontal")) == 0 && (Input.GetAxis("Vertical")) == 0)
        {
            isWalking = false;
            anim.SetBool("isWalking", false);
        }
    }

    void ManagePickups()
    {
        if (readyToRotate)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                objectToRotate.GetComponent<Rotate>().RotateClockwise();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (readyToHold)
            {
                if (objectToHold.transform.parent != null)
                {
                    objectToHold.GetComponentInParent<MoveObject>().hasAnObjectOn = false;
                }

                objectToHold.transform.SetParent(this.transform);
                readyToHold = false;
                isHolding = true;

                objectToHold.GetComponent<Rigidbody>().useGravity = false;

                objectToBePlacedOn = null;
                readyToPlace = false;

                anim.SetBool("isHolding", true);

            }
            else
            {

                anim.SetBool("isHolding", false);
                PlaceObject();
            }
        }
    }

    private void PlaceObject()
    {
        if (readyToPlace && objectToHold.GetComponent<MoveObject>().canBeStacked)
        {
            objectToHold.transform.position = newObjectPos;
            objectToHold.transform.SetParent(objectToBePlacedOn.transform);
            readyToPlace = false;

            objectToBePlacedOn.GetComponentInParent<MoveObject>().hasAnObjectOn = true;
        }
        else
        {
            objectToHold.transform.SetParent(null);
        }

        objectToHold.GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherGO = other.gameObject;

        //if (!readyToRotate && other.gameObject.tag == "RealWorld") {
        if (otherGO.GetComponent<Rotate>() != null && otherGO.GetComponent<Rotate>().rotationCycle.Length > 0)
        {
            objectToRotate = otherGO;
            readyToRotate = true;
        }
        //}

        MoveObject moveObjectScript = otherGO.GetComponent<MoveObject>();

        if (moveObjectScript != null)
        {
            if (!isHolding && moveObjectScript.canBeHeld && !moveObjectScript.hasAnObjectOn)
            {
                print("Ready to hold");
                objectToHold = otherGO;
                readyToHold = true;
            }
            else if (moveObjectScript.isASurface)
            {
                objectToBePlacedOn = otherGO;
                readyToPlace = true;
                newObjectPos = other.transform.position;
                newObjectPos.y += 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //print("Not ready to rotate");
        if (other.gameObject == objectToRotate)
        {
            readyToRotate = false;
            objectToRotate = null;
        }
        if (other.gameObject == objectToHold)
        {
            readyToHold = false;
            objectToHold = null;
        }
        if (other.gameObject == objectToBePlacedOn)
        {
            readyToPlace = false;
            objectToBePlacedOn = null;
        }
    }

    void Walk()
    {
        {
            anim.SetBool("isWalking", true);


            if (horizontal > 0 || horizontal < 0 || vertical > 0 || vertical < 0)
            {
                elliot.Move(moveDirection);

                moveDirection = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
                moveDirection *= movementSpeed;

                float moveVertical = Input.GetAxis("Vertical");
                float moveHorizontal = Input.GetAxis("Horizontal");

                Vector3 newPosition = new Vector3(moveHorizontal, 0.0f, moveVertical);
                transform.LookAt(newPosition + transform.position);
                transform.Translate(newPosition * movementSpeed * Time.deltaTime, Space.World);

                moveDirection.y -= gravity * Time.deltaTime;

            }
            else if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
                moveDirection *= 0;
        }

    }


 


}
