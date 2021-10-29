using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Event_Manager : MonoBehaviour
{
    public enum EventScenario
    {
        ICEBox_1,
        DESERTBox_1
    }

    public enum LaserLocation
    {
        TestArea,
        BlastDoorArea
    }


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

    [Header("MAIN HUB LOGIC")]
    [SerializeField] Animator IceExit_ElevatorDoor;
    [SerializeField] bool mainCorePowerState = true;
    [SerializeField] TextMeshProUGUI coreTerminateTimer_UI;
    [SerializeField] bool CoreTerminate = false;
    [SerializeField] float coreStartTime_sec = 600;
    float minute_h;
    float seconds_h;
    [SerializeField] bool timeUpCheck = false;

    [Header("DESERT DOME LOGIC")]
    [SerializeField] Animator elevatorTravel;
    bool travel;
    string currentLevel = "Up";
    [SerializeField] bool[] blackhole_Rings_InPos = new bool[5];
    [SerializeField] int boolCheck = 0;
    [SerializeField] Desert_Door_Sensor laserDoor;

    [Header("LASER LOGIC")]
    [SerializeField] Transform Laser_Object;
    public LaserLocation laserLocation;
    [SerializeField] Animator laser_Trail_Anim;
    [SerializeField] Animator laser_Orb_Anim;
    [SerializeField] Animator laser_TestDoor_Anim;
    [SerializeField] Animator laser_BlastDoor_Upper_Anim;
    [SerializeField] Animator laser_BlastDoor_Lower_Anim;
    [SerializeField] bool Laser_Fired = false;
    [SerializeField] bool Laser_CurrentlyActive = false;

    [Header("TELEPORTER LOGIC")]
    [SerializeField] bool hasItem;
    [SerializeField] bool teleportActive_Desert;
    [SerializeField] bool teleportActive_MainHub;
    [SerializeField] Animator teleport_Nosle_Desert;
    [SerializeField] Animator teleport_Nosle_MainHub;
    [SerializeField] Transform LaserTeleport_Position;

    [Header("BOMB LOGIC")]
    [SerializeField] Bomb_Script bomb_Obj;
    [SerializeField] Animator bomb_Orb_Anim;
    [SerializeField] float bomb_Anim_Speed;
    public bool inevitableExplosion = false;
    [SerializeField] bool bomb_Anim_Playing;

    [Header("EVENT LOGIC")]
    public EventScenario @event;



    // Start is called before the first frame update
    void Start()
    {
        _Manager = GetComponent<Game_Manager>();
        fade_BG_Anim.Play("Fade_Out");
        laserLocation = LaserLocation.TestArea;

        //Intialize core timer values
        minute_h = (coreStartTime_sec / 60f) - 1;
        seconds_h = 59;
        timeUpCheck = false;
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
            //Blackhole Puzzle Logic
            if(boolCheck == blackhole_Rings_InPos.Length)
            {
                UnlockLaserItemDoor_DESERT_DOME();
                Debug.Log("Unlocked");
            }
        }



        //Core Timer Logic
        if (mainCorePowerState && CoreTerminate == false)
        {
            seconds_h -= Time.deltaTime;

            if (seconds_h <= 0)
            {
                seconds_h = 59;
                minute_h -= 1;
            }


            if(minute_h < 0)
            {
                coreTerminateTimer_UI.text = "[Core-Overload]";
                timeUpCheck = true;
                CoreTerminate = true;
                _Manager.KillPlayer();
            }
            if(timeUpCheck == false)
            {
                coreTerminateTimer_UI.text = minute_h.ToString("00") + ":" + seconds_h.ToString("00");
            }
        }
        else if(mainCorePowerState == false && CoreTerminate == false)
        {
            coreTerminateTimer_UI.text = "[OFF]";
        }




        //Bomb Logic !! The Bomb is independant of the player location
        if (bomb_Obj.bombCurrentLocation == Bomb_Script.BombCurrentLocation.Desert)
        {
            if (_Manager.desertTemperature > 60)
            {
                if (!bomb_Anim_Playing)
                {
                    bomb_Orb_Anim.Play("Bomb_Explode_Increase_Anim");
                    bomb_Anim_Playing = true;
                    Debug.Log("Bomb Active");
                }

                //Explode Faster if temp is over 90 
                if(_Manager.desertTemperature > 90)
                {
                    bomb_Anim_Speed = 2;
                }
                else
                {
                    bomb_Anim_Speed = 1;
                }

            }
            else if (_Manager.desertTemperature <= 60)
            {
                bomb_Anim_Speed = -1;
            }

            //Check if explosion is inevitable
            if(inevitableExplosion == false)
            {
                bomb_Orb_Anim.SetFloat("Anim_Speed", bomb_Anim_Speed);
            }
            else if(inevitableExplosion == true)
            {
                bomb_Anim_Speed = 1;
                bomb_Orb_Anim.SetFloat("Anim_Speed", bomb_Anim_Speed);
            }
        }
        else if (bomb_Obj.bombCurrentLocation == Bomb_Script.BombCurrentLocation.MainHub)
        {
            if (_Manager.localTemperature > 60)
            {
                if (!bomb_Anim_Playing)
                {
                    bomb_Orb_Anim.Play("Bomb_Explode_Increase_Anim");
                    bomb_Anim_Playing = true;
                    Debug.Log("Bomb Active");
                }

                //Explode Faster if temp is over 90 
                if (_Manager.localTemperature > 90)
                {
                    bomb_Anim_Speed = 2;
                }
                else
                {
                    bomb_Anim_Speed = 1;
                }
            }
            else if (_Manager.localTemperature <= 60)
            {
                bomb_Anim_Speed = -1;
            }

            //Check if explosion is inevitable
            if (inevitableExplosion == false)
            {
                bomb_Orb_Anim.SetFloat("Anim_Speed", bomb_Anim_Speed);
            }
            else if (inevitableExplosion == true)
            {
                bomb_Anim_Speed = 1;
                bomb_Orb_Anim.SetFloat("Anim_Speed", bomb_Anim_Speed);
            }
        }
    }



    //Change Core Power State
    public void ChangeCorePowerState(bool value)
    {
        mainCorePowerState = value;
    }




    public void TeleportItem_Confirm(bool status)
    {
        hasItem = status;

        if (status)
        {
            Laser_Object.gameObject.SetActive(false);
            laserLocation = LaserLocation.BlastDoorArea;
        }
        else
        {
            Laser_Object.gameObject.SetActive(true);
            Laser_Object.position = LaserTeleport_Position.position;
            LaserTeleport_Position.gameObject.SetActive(false);
        }
    }



    public void LaserFiring_Status()
    {

    }



    public void ActivateTeleport_LaserToMainHUB()
    {
        if (!teleportActive_Desert && !Laser_Fired)
        {
            teleportActive_Desert = true;
            teleport_Nosle_Desert.Play("Teleport_Nosle_Desert_ToMain_Anim");
        }
        else
        {
            Debug.Log("Can't Teleport Right Now");
        }
    }


    public void ActivateTeleport_ReceiveLaser()
    {
        if (!teleportActive_MainHub && hasItem && !Laser_Fired)
        {
            teleportActive_MainHub = true;
            teleport_Nosle_MainHub.Play("Teleport_Nosle_R_Deploy_Anim");

        }
        else
        {
            Debug.Log("Nothing to Receive");
        }
    }




    public void ActivateLaserFire()
    {
        if (laserLocation == LaserLocation.BlastDoorArea && laser_BlastDoor_Upper_Anim && laser_BlastDoor_Lower_Anim && Laser_Fired == false)
        {
            laser_Orb_Anim.Play("Laser_Orb_Anim");
            laser_BlastDoor_Upper_Anim.Play("Laser_Door_Dissolve_Anim");
            laser_BlastDoor_Lower_Anim.Play("Laser_Door_Dissolve_Anim");
            laser_Trail_Anim.Play("Laser_Trail_Anim");
            Laser_Fired = true;
        }
        else if(laserLocation == LaserLocation.TestArea && laser_TestDoor_Anim && Laser_Fired == false && !teleportActive_Desert)
        {
            laser_Orb_Anim.Play("Laser_Orb_Anim");
            laser_TestDoor_Anim.Play("Laser_Door_Dissolve_Anim");
            laser_Trail_Anim.Play("Laser_Trail_Anim");
            Laser_Fired = true;
        }
    }


    //The blast door Animation should call this method to despawn its colider (via an Event)
    public void BlastDoorDissolved()
    {

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
        laserDoor.ChangeDoorLockedStatus(true);
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
        IceExit_ElevatorDoor.Play("Elevator_Door_Open");
    }


    public void RestartGameFadeOut()
    {
        fade_BG_Anim.Play("Fade_In");
    }
}
