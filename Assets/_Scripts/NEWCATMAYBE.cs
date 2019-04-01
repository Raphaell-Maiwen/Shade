using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEWCATMAYBE : MonoBehaviour{

    // -- VARIABLES
    public Animator anim;
    public CharacterController cat;

    public float horizontal;
    public float vertical;
    public float movementSpeed;
    public float rotateSpeed;
    Vector3 movement;
    Vector3 lastMovementMade;
    private Vector3 moveDirection = Vector3.zero;

    public bool isIdle;
    public bool isWalking;
    public bool isPouncing;
    public bool isFlopping;
    public bool isTickled; // on human interaction
    public bool isBatting;
    public bool isEating;
    public bool isGrabbed; // on human interaction
    public bool isJumping;




    void Start ()
    {
        anim = GetComponent<Animator>();
        cat = GetComponent<CharacterController>();
    }

	void Update ()
    {
        ManageMovement();
    }

    private void LateUpdate()
    {


    }

    // -- ACTIONS

    void ManageMovement()
    {
        // ----------------------------------------------- WALK CONDITIONS

        horizontal = Input.GetAxis("Horizontal");// * movementSpeed;
        vertical = Input.GetAxis("Vertical");// * movementSpeed;      

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


        // ----------------------------------------------- FLOP CONDITIONS

        if ((isFlopping == true) && (Input.GetButton("X")))
        {
            // -- Currently getting a bellyrub is set to X to test the controls
            // -- But eventually this should be triggered by the Human giving the cat a bellyrub
            // -- Cat can choose to flop but only makes the tickled animation when Human interacts while flopped
            isTickled = true;
            anim.SetBool("isTickled", true);
        }

        if ((isFlopping == true) && (Input.GetButtonUp("X")))
        {
            isTickled = false;
            anim.SetBool("isTickled", false);
        }

        if (Input.GetButtonDown("Y"))
        {
            isFlopping = true;
            Flop();
        }

        if (Input.GetButtonUp("Y"))
        {
            anim.SetBool("isFlopping", false);
        }

        // -------------------------------------------------- POUNCE CONDITIONS

        if (Input.GetAxis("Trigger_L") != 0)
        {
            //isPouncing = true;
            Debug.Log(Input.GetAxis("Trigger_L"));
            //Pounce();
        }

        if ((isPouncing == true) && (Input.GetAxis("Trigger_L") == 0))
        {
            //isPouncing = false;
            //anim.SetBool("isPouncing", false);
        }

        if (Input.GetAxis("Trigger_R") != 0)
        {
            isPouncing = true;
            Debug.Log(Input.GetAxis("Trigger_R"));
            Pounce();
        }

        if ((isPouncing == true) && (Input.GetAxis("Trigger_R") == 0)) 
        {
            isPouncing = false;
            anim.SetBool("isPouncing", false);
        }

        // ----------------------------------------------------- BATTING CONDITIONS

        if (Input.GetButtonDown("R_Bumper"))
        {
            //Debug.Log("R_Bumper");
            Bat();
        }

        if ((isBatting == true) && (Input.GetButtonUp("R_Bumper")))
        {
            isBatting = false;
            anim.SetBool("isBatting", false);
        }

        // ----------------------------------------------------- EATING CONDITIONS
        if (Input.GetButtonDown("L_Bumper"))
        {
            //Debug.Log("L_Bumper");
            Eat();
        }

        if ((isEating == true) && (Input.GetButtonUp("L_Bumper")))
        {
            isEating = false;
            anim.SetBool("isEating", false);
        }
        // ----------------------------------------------------- JUMPING CONDITIONS

        if (Input.GetButtonDown("A"))
        {
            Debug.Log("A");
            Jump();
        }

        // ------------------------------------------------------ EXTRA BUTTONS


        if (Input.GetButtonDown("B"))
        {
            //Debug.Log("B");
        }

        // ------------------------------------------------------ CAMERA CONTROLS?

        if (Input.GetAxis("RightStickX") != 0)
        {
            //Debug.Log("Right Stick X");
        }

        if (Input.GetAxis("RightStickY") != 0)
        {
            //Debug.Log("Right Stick Y");
        }

    }

    // -------------------------------------------------------- MOVEMENT FUNCTIONS

    void Bat()
    {
        anim.SetBool("isBatting", true);
    }

    void Jump()
    {
        anim.SetBool("isJumping", true);
    }

    void Eat()
    {
        anim.SetBool("isEating", true);
    }

    void Flop()
    {
        anim.SetBool("isFlopping", true);
    }

    void Pounce()
    {
        anim.SetBool("isPouncing", true);
    }

    void Walk()
    {
        anim.SetBool("isWalking", true);

        // ----- this is all bullshit and you probably shouldn't keep any of it ------- //

        if (horizontal > 0 || horizontal < 0 || vertical > 0 || vertical < 0)
        {
            cat.Move(moveDirection);
        
            moveDirection = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            moveDirection *= movementSpeed;
        
            float moveVertical = Input.GetAxis("Vertical");
            float moveHorizontal = Input.GetAxis("Horizontal");
        
           Vector3 newPosition = new Vector3(moveHorizontal, 0.0f, moveVertical);
            transform.LookAt(newPosition + transform.position);
            transform.Translate(newPosition * movementSpeed * Time.deltaTime, Space.World);
        
        }
        else if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
            moveDirection *= 0;
    }


}





// -- CONTROLS
// Axis 9 = L TRIGGER
// Axis 10 = R TRIGGER
// Bumper 4         = L BUMPER
// Bumper 5         = R BUMPER
// X AXIS / Y AXIS  = L JOYSTICK
// 7th / 8th AXIS   = D-PAD
// 4th/5th AXIS /10 = R JOYSTICK
// 6                = BACK
// 7                = START
// 0                = A
// 1                = B
// 2                = X
// 3                = Y
