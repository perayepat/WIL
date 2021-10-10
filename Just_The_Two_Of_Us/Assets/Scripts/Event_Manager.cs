using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Manager : MonoBehaviour
{
    Game_Manager _Manager;

    [Header("Optional UI Logic")]
    [SerializeField] Animator fade_BG_Anim;

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

    [Header("DESERT DOME LOGIC")]
    [SerializeField] Animator elevatorTravel;
    bool travel;
    string currentLevel = "Up";
    [SerializeField] bool[] blackhole_Rings_InPos = new bool[5];
    [SerializeField] int boolCheck = 0;


    public enum EventScenario
    {
        ICEBox_1,
        DESERTBox_1
    }

    [Header("EVENT LOGIC")]
    public EventScenario @event;

    // Start is called before the first frame update
    void Start()
    {
        _Manager = GetComponent<Game_Manager>();
        fade_BG_Anim.Play("Fade_Out");
    }

    // Update is called once per frame
    void Update()
    {
        //ICE Dome logic
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

        //Deserts Dome logic
        if (@event == EventScenario.DESERTBox_1)
        {
            if(boolCheck == blackhole_Rings_InPos.Length)
            {
                UnlockLaserItemDoor_DESERT_DOME();
                Debug.Log("Unlocked");
            }
        }
    }


    public void UpdateRingBoolState(int ringID, bool state)
    {
        if(blackhole_Rings_InPos[ringID] == true)
        {
            boolCheck--;
        }
        else if(state == true)
        {
            boolCheck++;
        }

        blackhole_Rings_InPos[ringID] = state;
    }


    void UnlockLaserItemDoor_DESERT_DOME()
    {

    }


    public void Exit_ICE_DOME()
    {
        ElevatorDoorCommand_ICE_DOME(false);
        setDoorOpen = false;

        @event = EventScenario.DESERTBox_1;

        StartCoroutine(TakePlayerToMainHub());
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


    public bool ElevatorTravelCommand_DESERT_DOME(string travelDir)
    {
        bool returnValue = false;

        if(travelDir == "Up" && currentLevel == "Down")
        {
            currentLevel = travelDir;
            elevatorTravel.Play("Elevator_Travel_Up");
            returnValue = true;
        }
        else if(travelDir == "Down" && currentLevel == "Up")
        {
            currentLevel = travelDir;
            elevatorTravel.Play("Elevator_Travel_Down");
            returnValue = true;
        }

        return returnValue;
    }


    IEnumerator TakePlayerToMainHub()
    {
        fade_BG_Anim.Play("Fade_In");

        yield return new WaitForSeconds(4f);

        fade_BG_Anim.Play("Fade_Out");
        _Manager.NewPlayerPosition(_Manager.mainHubSpawnPoint);
    }


    public void RestartGameFadeOut()
    {
        fade_BG_Anim.Play("Fade_In");
    }
}
