using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller_Script : MonoBehaviour
{
    [SerializeField] Transform orientation;
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

    [Header("KeyBinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("Drag")]
    public float groundedDrag = 6f;
    public float airDrag = 2f;
    public float swimDrag = 2f;
    public float zero_G_Drag = 0f;

    [Header("Grounded")]
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
    }


    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDist, groundMask);

        PlayerInput();
        ControlDrag();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }


    void PlayerInput()
    {
        horiMovement = Input.GetAxisRaw("Horizontal");
        vertMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * vertMovement + orientation.right * horiMovement;
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
                rb.AddForce(-orientation.up * 9.81f, ForceMode.Force);
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
        playerState = PlayerState.Walk;
    }
}
