using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera_Controller : MonoBehaviour
{
    [SerializeField] Transform playerCam;
    [SerializeField] Transform orientation;
    [SerializeField] float sensX;
    [SerializeField] float sensY;


    float mouseX;
    float mouseY;

    [SerializeField] float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    private void Awake()
    {
        //Application.targetFrameRate = 60;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        PlayerInput();

        playerCam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    void PlayerInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX + sensX * multiplier;
        xRotation -= mouseY + sensY * multiplier;


        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
