using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Manager : MonoBehaviour
{
    Game_Manager _Manager;

    [Header("ICE DOME LOGIC")]
    [SerializeField] Keypad_Instance keypad_Instance;
    [SerializeField] string keypad_Passcode;
    [SerializeField] bool keycard_1;
    [SerializeField] bool keycard_2;
    [SerializeField] Animator elevatorDoor;
    bool setDoorOpen;

    [SerializeField] GameObject iceOnDoor_Obj;
    [SerializeField] bool doorICEMelted;
    Vector3 doorMeltPos;


    public enum EventScenario
    {
        ICEBox_1
    }

    public EventScenario @event;

    // Start is called before the first frame update
    void Start()
    {
        _Manager = GetComponent<Game_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(@event == EventScenario.ICEBox_1)
        {
            //Melt ICE On Door
            if (_Manager.currentTemperature >= 0 && iceOnDoor_Obj.transform.position.y > 6.15f)
            {
                doorMeltPos = new Vector3(0, -0.0116f, 0);
                iceOnDoor_Obj.transform.position += doorMeltPos * Time.deltaTime;
            }
            else if (iceOnDoor_Obj.transform.position.y < 6.15f)
            {
                doorICEMelted = true;
            }


            //ICE Door Logic (KeyCard)
            if (keycard_1 && keycard_2 && setDoorOpen == false && doorICEMelted && keypad_Passcode == keypad_Instance.panelOutput)
            {
                ElevatorDoorCommand_ICE_DOME(true);
                setDoorOpen = true;
            }


        }
    }



    public void UpdateKeyCardStatus(int keycard_ID ,bool boolState)
    {
        if(keycard_ID == 1)
        {
            keycard_1 = boolState;
        }
        if (keycard_ID == 2)
        {
            keycard_2 = boolState;
        }
    }

    private void ElevatorDoorCommand_ICE_DOME(bool open)
    {
        if (open)
        {
            elevatorDoor.Play("Elevator_Door_Open");
        }
        else
        {
            elevatorDoor.Play("Elevator_Door_Close");
        }

    }

}
