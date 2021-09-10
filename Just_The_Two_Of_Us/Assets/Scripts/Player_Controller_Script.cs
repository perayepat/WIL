using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Controller_Script : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerCam;
    float playerHeight = 2f;

    [Header("Movement")]
    public float moveSpeed = 6f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float airSpeed = 2f;
    [SerializeField] float zero_G_Speed = 2f;
    [SerializeField] float swimSpeed = 2f;
    public float movementMultiplier = 10f;
    [SerializeField] float airMovementMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 15f;

    [Header("Swiming")]
    public float swimUpSpeed = 7f;

    [Header("KeyBinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode grabKey = KeyCode.E;
    [SerializeField] KeyCode pressButtonKey = KeyCode.Mouse0;

    [Header("Drag")]
    public float groundedDrag = 6f;
    public float airDrag = 2f;
    public float swimDrag = 2f;
    public float zero_G_Drag = 0f;

    [Header("Gravity")]
    [Range(1.0f, 9.81f)]
    [SerializeField] float gravityMultiplier = 1f;

    [Header("Grab/Pickup")]
    public Transform grabPoint;
    public Rigidbody grabItem;
    Outline tempHolderG;
    [SerializeField] float grabHoldDist = 1.5f;
    [SerializeField] bool canGrab;
    [SerializeField] bool holding = false;
    [SerializeField] bool grabDelay = false;
    [SerializeField] LayerMask grabItemMask;

    [Header("Keypad")]
    [SerializeField] LayerMask pressButtonMask;
    [SerializeField] bool canPressButton;
    Outline tempHolderK;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI Interact_Grab_UI;
    [SerializeField] TextMeshProUGUI Interact_PressButton_UI;

    bool isGrounded;
    float groundDist = 0.4f;
    [SerializeField] LayerMask groundMask;

    float horiMovement;
    float vertMovement;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    

    public enum PlayerState
    {
        Walk,
        Swim,
        Zero_Gravity
    }
    [SerializeField] PlayerState playerState;



    bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Interact_Grab_UI.enabled = false;
        Interact_PressButton_UI.enabled = false;
    }


    private void Update()
    {
        //Ground Check
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDist, groundMask);



        //Keypad Button Press Logic
        KeypadButtonPress();



        //Pickup/Interact Check
        RaycastHit hit;
        canGrab = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 2.5f, grabItemMask);
        if (canGrab)
        {
            tempHolderG = hit.collider.GetComponent<Outline>();
        }


        PlayerInput(hit);
        ControlDrag();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }
        else if (Input.GetKey(jumpKey) && playerState == PlayerState.Swim)
        {
            SwimUp();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);



        //Update Grab/Pickup UI
        if (canGrab && holding == false)
        {
            Interact_Grab_UI.enabled = true;
            tempHolderG.GetComponent<Outline>().OutlineWidth = 4;
        }
        else
        {
            Interact_Grab_UI.enabled = false;
            if (tempHolderG)
            {
                tempHolderG.GetComponent<Outline>().OutlineWidth = 0;
            }

        }
        //Update Grabbed item pos if holding
        if(holding && grabItem != null)
        {
            //grabItem.MovePosition(grabPoint.position);
            //grabItem.transform.position = grabPoint.position;
            grabItem.velocity = 50 * (grabPoint.position - grabItem.transform.position);
        }
    }


    void KeypadButtonPress()
    {
        RaycastHit hit;
        canPressButton = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 2.5f, pressButtonMask);

        //Update Player UI
        if (canPressButton)
        {
            Interact_PressButton_UI.enabled = true;
            tempHolderK = hit.collider.GetComponent<Outline>();
            tempHolderK.OutlineWidth = 4;
        }
        else
        {
            Interact_PressButton_UI.enabled = false;
            if (tempHolderK)
            {
                tempHolderK.OutlineWidth = 0;
            }

        }

        //Logic
        if (canPressButton && Input.GetKeyDown(pressButtonKey))
        {
            Keypad_Button_Script KB = hit.collider.GetComponent<Keypad_Button_Script>();
            string keyValue = KB.keypad_Num.ToString();
            KB.ReturnKeyPadObj().AppendKeypadInput(keyValue);
        }
    }


    void PlayerInput(RaycastHit hit)
    {
        horiMovement = Input.GetAxisRaw("Horizontal");
        vertMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * vertMovement + orientation.right * horiMovement;


        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
        {
            grabHoldDist += Input.GetAxis("Mouse ScrollWheel");
            grabHoldDist = Mathf.Clamp(grabHoldDist, 0.8f, 1.5f);
        }

        grabPoint.localPosition = new Vector3(0, 0, grabHoldDist);


        //For Grabbing Items
        if (Input.GetKeyDown(grabKey) && canGrab && grabDelay == false && holding == false)
        {
            grabHoldDist = 1.5f;
            grabItem = hit.rigidbody;
            grabItem.useGravity = false;
            //grabItem.GetComponent<BoxCollider>().isTrigger = true;
            holding = true;

            StopCoroutine(GrabDelay());
            StartCoroutine(GrabDelay());
        }
        else if(Input.GetKeyDown(grabKey) && holding && grabDelay == false)
        {
            grabItem.useGravity = true;
            //grabItem.GetComponent<BoxCollider>().isTrigger = false;
            holding = false;
            grabItem = null;

            StopCoroutine(GrabDelay());
            StartCoroutine(GrabDelay());
        }
    }
    void ControlDrag()
    {
        if(playerState == PlayerState.Walk)
        {
            if (isGrounded)
            {
                rb.drag = groundedDrag;
                moveSpeed = walkSpeed;
            }
            else
            {
                rb.drag = airDrag;
                moveSpeed = airSpeed;
                rb.AddForce(-orientation.up * gravityMultiplier, ForceMode.Force);
            }
        }
        else if (playerState == PlayerState.Swim)
        {
            rb.drag = swimDrag;
            moveSpeed = swimSpeed;
        }
        else if (playerState == PlayerState.Zero_Gravity)
        {
            rb.drag = zero_G_Drag;
            moveSpeed = zero_G_Speed;
        }

    }



    private void FixedUpdate()
    {
        MovePlayer();
    }


    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMovementMultiplier, ForceMode.Acceleration);
        }
    }
    void Jump()
    {
        if(playerState == PlayerState.Walk)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
    void SwimUp()
    {
        if (playerState == PlayerState.Swim)
        {
            rb.AddForce(transform.up * swimUpSpeed, ForceMode.Force);
        }
    }


    //Checks if player is under water
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Water_Volume")
        {
            playerState = PlayerState.Swim;
        }
        else
        {
            playerState = PlayerState.Walk;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water_Volume")
        {
            playerState = PlayerState.Walk;
        }
    }

    IEnumerator GrabDelay()
    {
        grabDelay = true;
        yield return new WaitForSeconds(0.2f);
        grabDelay = false;
    }
}
