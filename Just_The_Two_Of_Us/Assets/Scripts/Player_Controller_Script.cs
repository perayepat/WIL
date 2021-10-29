using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Controller_Script : MonoBehaviour
{
    Event_Manager event_Manager;

    [SerializeField] Transform orientation;
    [SerializeField] Transform playerCam;
    float playerHeight = 2f;

    [Header("Health")]
    [SerializeField] float health = 100;

    [Header("Movement")]
    public float moveSpeed = 6f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float airSpeed = 2f;
    [SerializeField] float zero_G_Speed = 2f;
    [SerializeField] float swimSpeed = 2f;
    public float movementMultiplier = 10f;
    [SerializeField] float airMovementMultiplier = 0.4f;
    bool falling = false;
    public bool sitting_Elevator = false;

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
    public float fallingDrag = 0f;

    [Header("Gravity")]
    [Range(1.0f, 9.81f)]
    [SerializeField] float gravityMultiplier = 1f;

    [Header("Grab/Pickup")]
    public Transform grabPoint;
    public Rigidbody grabItem;
    Outline tempHolderG;
    [SerializeField] float grabHoldDist = 1.5f;
    [SerializeField] float Min_HoldDist = 1;
    [SerializeField] float Max_HoldDist = 1.5f;
    [SerializeField] bool canGrab;
    [SerializeField] bool holding = false;
    [SerializeField] bool grabDelay = false;
    [SerializeField] LayerMask grabItemMask;

    [Header("Keypad")]
    [SerializeField] LayerMask pressButtonMask;
    [SerializeField] LayerMask pressElevatorTravelButtonMask;
    [SerializeField] LayerMask pressElevatorICEDOWE_EXITButtonMask;
    [SerializeField] LayerMask pressLaser_ButtonMask;
    [SerializeField] LayerMask pressTeleport_ButtonMask;
    [SerializeField] LayerMask pressColor_ButtonMask;
    [SerializeField] LayerMask pressCore_ButtonMask;
    [SerializeField] bool canPressButton;
    [SerializeField] bool canPressElevatorTravelButton;
    [SerializeField] bool canPressICEDOWE_EXITButton;
    [SerializeField] bool canPressLaserActivateButton;
    [SerializeField] bool canPressTeleportButton;
    [SerializeField] bool canPressColorButton;
    [SerializeField] bool canPressCoreButton;
    Outline tempHolder_Kp;
    Outline tempHolder_ETB;
    Outline tempHolderK_IDEB;
    Outline tempHolderK_LBP;
    Outline tempHolderK_TPB;
    Outline tempHolderK_CB;
    Outline tempHolderK_CoreB;

    [Header("BlackHole_Puzzle")]
    [SerializeField] LayerMask blackHoleRingMask;
    [SerializeField] bool canInteractWithRing;
    BlackHole_Ring_Script temp_Blackhole_Script;
    Outline temp_Blackhole_RingOutline;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI Interact_Grab_UI;
    [SerializeField] TextMeshProUGUI Interact_PressButton_UI;

    [Header("Elevator_Desert_Dome")]
    [SerializeField] Transform elevatorSeat;


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


    public enum PlayerEnvironmentState
    {
        InDesert,
        Not_InDesert
    }
    public PlayerEnvironmentState playerEnvironmentState;



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

        event_Manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Event_Manager>();
    }


    private void Update()
    {
        //Ground Check
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDist, groundMask);


        //UI Check
        if(!canPressButton && !canPressElevatorTravelButton && !canPressICEDOWE_EXITButton && !canInteractWithRing && !canPressLaserActivateButton && !canPressTeleportButton && !canPressColorButton)
        {
            Interact_PressButton_UI.enabled = false;
        }


        //Keypad Button Press Logic
        KeypadButtonPress();

        //Core Lever Press
        CoreButtonPress();

        //Color Button Press Logic (Recharge Station Button)
        ColorButtonPress();

        //Laser Activate Press Logic
        PressLaserButton();

        //Teleport Button Press Logic
        PressTeleportButton();

        //ICE DOME Elevator Exit Keypad Logic
        InteractICEDOME_Exit_Button();

        //BlackHole Ring Interaction
        InteractWithBlackHoleRing();


        //Elevator Button Press Logic
        ElevatorTravelKeypadButton_Press();
        if(sitting_Elevator == true)
        {
            Sit_In_Elevator();
        }



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



    public void DamagePlayerHealth(float healthEffect)
    {
        health -= healthEffect;
    }
    public float RetrievePlayerHealth()
    {
        return health;
    }



    void InteractICEDOME_Exit_Button()
    {
        RaycastHit hit;
        canPressICEDOWE_EXITButton = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 1f, pressElevatorICEDOWE_EXITButtonMask);

        //Update Player UI
        if (canPressICEDOWE_EXITButton)
        {
            Interact_PressButton_UI.enabled = true;
            tempHolderK_IDEB = hit.collider.GetComponent<Outline>();
            tempHolderK_IDEB.OutlineWidth = 4;
        }
        else
        {
            //Interact_PressButton_UI.enabled = false;
            if (tempHolderK_IDEB)
            {
                tempHolderK_IDEB.OutlineWidth = 0;
            }

        }

        //Logic
        if (canPressICEDOWE_EXITButton && Input.GetKeyDown(pressButtonKey))
        {
            Elevator_Travel_Button_Script _Travel_Button_Script = hit.collider.GetComponent<Elevator_Travel_Button_Script>();

            if (_Travel_Button_Script.travelValue == "Down")
            {
                event_Manager.Exit_ICE_DOME();
            }

        }
    }


    void InteractWithBlackHoleRing()
    {
        RaycastHit hit;
        canInteractWithRing = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 2.5f, blackHoleRingMask);

        //Update Player UI
        if (canInteractWithRing)
        {
            Interact_PressButton_UI.enabled = true;
            if (hit.collider.GetComponent<Outline>())
            {
                temp_Blackhole_RingOutline = hit.collider.GetComponent<Outline>();
                temp_Blackhole_RingOutline.OutlineWidth = 4;
            }

        }
        else
        {
            //Interact_PressButton_UI.enabled = false;
            if (temp_Blackhole_RingOutline)
            {
                temp_Blackhole_RingOutline.OutlineWidth = 0;
            }
        }

        //Logic
        if(canInteractWithRing && Input.GetKeyDown(pressButtonKey))
        {
            temp_Blackhole_Script = hit.collider.GetComponentInParent<BlackHole_Ring_Script>();

            if (!temp_Blackhole_Script.RotationState())
            {
                temp_Blackhole_Script.LockRingRotation();
            }
        }
    }



    //Recharge Station Color Button
    void ColorButtonPress()
    {
        RaycastHit hit;
        canPressColorButton = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 2.5f, pressColor_ButtonMask);

        //Update Player UI
        if (canPressColorButton)
        {
            Interact_PressButton_UI.enabled = true;
            tempHolderK_CB = hit.collider.GetComponent<Outline>();
            tempHolderK_CB.OutlineWidth = 4;
        }
        else
        {
            //Interact_PressButton_UI.enabled = false;
            if (tempHolderK_CB)
            {
                tempHolderK_CB.OutlineWidth = 0;
            }

        }

        //Logic
        if (canPressColorButton && Input.GetKeyDown(pressButtonKey))
        {
            Recharge_Station_Button_Script _RSBS = hit.collider.GetComponent<Recharge_Station_Button_Script>();

            if(_RSBS.button_Type == Recharge_Station_Button_Script.Button_Type.Enter)
            {
                _RSBS.PressEnterButton();
            }
            else if(_RSBS.button_Type == Recharge_Station_Button_Script.Button_Type.Reset)
            {
                _RSBS.PressResetButton();
            }
            else if(_RSBS.button_Type == Recharge_Station_Button_Script.Button_Type.Color)
            {
                _RSBS.PressColorButton();
            }
        }
    }




    void CoreButtonPress()
    {
        RaycastHit hit;
        canPressCoreButton = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 2.5f, pressCore_ButtonMask);

        //Update Player UI
        if (canPressCoreButton)
        {
            Interact_PressButton_UI.enabled = true;
            tempHolderK_CoreB = hit.collider.GetComponent<Outline>();
            tempHolderK_CoreB.OutlineWidth = 4;
        }
        else
        {
            //Interact_PressButton_UI.enabled = false;
            if (tempHolderK_CoreB)
            {
                tempHolderK_CoreB.OutlineWidth = 0;
            }

        }

        //Logic
        if (canPressCoreButton && Input.GetKeyDown(pressButtonKey))
        {
            Core_Button_Press_Script coreButtonPress = hit.collider.GetComponent<Core_Button_Press_Script>();
            coreButtonPress.CoreButtonPressed();
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
            tempHolder_Kp = hit.collider.GetComponent<Outline>();
            tempHolder_Kp.OutlineWidth = 4;
        }
        else
        {
            //Interact_PressButton_UI.enabled = false;
            if (tempHolder_Kp)
            {
                tempHolder_Kp.OutlineWidth = 0;
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


    void ElevatorTravelKeypadButton_Press()
    {
        RaycastHit hit;
        canPressElevatorTravelButton = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 2f, pressElevatorTravelButtonMask) ;

        //Update Player UI
        if (canPressElevatorTravelButton)
        {
            Interact_PressButton_UI.enabled = true;
            tempHolder_ETB = hit.collider.GetComponent<Outline>();
            tempHolder_ETB.OutlineWidth = 4;
        }
        else
        {
            //Interact_PressButton_UI.enabled = false;
            if (tempHolder_ETB)
            {
                tempHolder_ETB.OutlineWidth = 0;
            }
        }

        //Logic
        if(canPressElevatorTravelButton && Input.GetKeyDown(pressButtonKey))
        {
            Elevator_Travel_Button_Script _Travel_Button_Script = hit.collider.GetComponent<Elevator_Travel_Button_Script>();

            if (event_Manager.ElevatorTravelCommand_DESERT_DOME(_Travel_Button_Script.travelValue))
            {
                sitting_Elevator = true;
            }
        }
    }



    void PressLaserButton()
    {
        RaycastHit hit;
        canPressLaserActivateButton = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 2.5f, pressLaser_ButtonMask);

        //Update Player UI
        if (canPressLaserActivateButton)
        {
            Interact_PressButton_UI.enabled = true;
            tempHolderK_LBP = hit.collider.GetComponent<Outline>();
            tempHolderK_LBP.OutlineWidth = 4;
        }
        else
        {
            //Interact_PressButton_UI.enabled = false;
            if (tempHolderK_LBP)
            {
                tempHolderK_LBP.OutlineWidth = 0;
            }

        }

        //Logic
        if (canPressLaserActivateButton && Input.GetKeyDown(pressButtonKey))
        {
            event_Manager.ActivateLaserFire();
        }
    }



    void PressTeleportButton()
    {
        RaycastHit hit;
        canPressTeleportButton = Physics.Raycast(playerCam.position, playerCam.forward, out hit, 2.5f, pressTeleport_ButtonMask);

        //Update Player UI
        if (canPressTeleportButton)
        {
            Interact_PressButton_UI.enabled = true;
            tempHolderK_TPB = hit.collider.GetComponent<Outline>();
            tempHolderK_TPB.OutlineWidth = 4;
        }
        else
        {
            //Interact_PressButton_UI.enabled = false;
            if (tempHolderK_TPB)
            {
                tempHolderK_TPB.OutlineWidth = 0;
            }

        }

        //Logic
        if (canPressTeleportButton && Input.GetKeyDown(pressButtonKey))
        {
            Teleport_Button_ID teleport_Button_ = hit.collider.GetComponent<Teleport_Button_ID>();
            
            if(teleport_Button_.teleporter_ID == "Sender")
            {
                event_Manager.ActivateTeleport_LaserToMainHUB();
            }
            else if (teleport_Button_.teleporter_ID == "Receiver")
            {
                event_Manager.ActivateTeleport_ReceiveLaser();
            }
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
            grabHoldDist = Mathf.Clamp(grabHoldDist, Min_HoldDist, Max_HoldDist);
        }




        //Drop Item if moved to far from center of screen
        if(!canGrab && holding)
        {
            //Only for Energy Cell (Check if holding item)
            if (grabItem.GetComponent<Energy_Cell_Script>())
            {
                grabItem.GetComponent<Energy_Cell_Script>().inHand = false;
            }



            grabItem.useGravity = true;
            //grabItem.GetComponent<BoxCollider>().isTrigger = false;
            holding = false;
            grabItem = null;


            StopCoroutine(GrabDelay());
            StartCoroutine(GrabDelay());
        }

  




        //Check if Holding a Big Item
        if (grabItem != null && holding)
        {
            if (hit.rigidbody.tag == "Bomb")
            {
                grabPoint.localPosition = new Vector3(0, 0, 2.5f);
            }
            else
            {
                grabPoint.localPosition = new Vector3(0, 0, grabHoldDist);
            }
        }




        //For Grabbing Items
        if (Input.GetKeyDown(grabKey) && canGrab && grabDelay == false && holding == false)
        {
            if(hit.collider.tag == "Power_Cell" && hit.collider.GetComponent<Energy_Cell_Script>())
            {
                if(hit.collider.GetComponent<Energy_Cell_Script>().inChamber == false)
                {
                    grabHoldDist = 1.5f;
                    grabItem = hit.rigidbody;
                    grabItem.useGravity = false;
                    //grabItem.GetComponent<BoxCollider>().isTrigger = true;
                    holding = true;

                    grabItem.GetComponent<Energy_Cell_Script>().inHand = true;
                }
            }
            else
            {
                grabHoldDist = 1.5f;
                grabItem = hit.rigidbody;
                grabItem.useGravity = false;
                //grabItem.GetComponent<BoxCollider>().isTrigger = true;
                holding = true;
            }



            ////Only for Energy Cell (Check if holding item)
            //if (grabItem.GetComponent<Energy_Cell_Script>())
            //{
            //    grabItem.GetComponent<Energy_Cell_Script>().inHand = true;
            //}


            StopCoroutine(GrabDelay());
            StartCoroutine(GrabDelay());
        }
        else if(Input.GetKeyDown(grabKey) && holding && grabDelay == false)
        {
            //Only for Energy Cell (Check if holding item)
            if (grabItem.GetComponent<Energy_Cell_Script>())
            {
                grabItem.GetComponent<Energy_Cell_Script>().inHand = false;
            }



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

        if (playerState == PlayerState.Walk)
        {
            if (isGrounded)
            {
                falling = false;
                rb.drag = groundedDrag;
                moveSpeed = walkSpeed;
            }
            else
            {
                //Player Gravity
                rb.AddForce(-orientation.up * gravityMultiplier, ForceMode.Force);

                if (falling == false)
                {
                    fallingDrag = airDrag;
                    moveSpeed = airSpeed;
                    falling = true;
                }

                fallingDrag -= Time.deltaTime;
                fallingDrag = Mathf.Clamp(fallingDrag, 0, airDrag);
                rb.drag = fallingDrag;
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

    void Sit_In_Elevator()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        //transform.position = elevatorSeat.position;
        rb.MovePosition(elevatorSeat.position);
    }
    public void ExitSeat_Elevator()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        sitting_Elevator = false;

    }



    private void OnTriggerStay(Collider other)
    {
        //Checks if player is under water
        if (other.tag == "Water_Volume")
        {
            playerState = PlayerState.Swim;
        }
        else
        {
            playerState = PlayerState.Walk;
        }


        //Checks if player is in the desert
        if (other.tag == "Desert_Volume")
        {
            playerEnvironmentState = PlayerEnvironmentState.InDesert;
        }
        else
        {
            playerEnvironmentState = PlayerEnvironmentState.Not_InDesert;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //Checks if player is under water
        if (other.tag == "Water_Volume")
        {
            playerState = PlayerState.Walk;
        }

        //Checks if player is in the desert
        if (other.tag == "Desert_Volume")
        {
            playerEnvironmentState = PlayerEnvironmentState.Not_InDesert;
        }
    }

    IEnumerator GrabDelay()
    {
        grabDelay = true;
        yield return new WaitForSeconds(0.2f);
        grabDelay = false;
    }

}
