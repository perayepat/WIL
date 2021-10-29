using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport_Area_ID : MonoBehaviour
{
    Event_Manager event_Manager;

    public string area_ID;



    private void Awake()
    {
        event_Manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Event_Manager>();
    }



    public void Teleport_Area()
    {
        if (area_ID == "Sender")
        {
            event_Manager.TeleportItem_Confirm(true);
        }
        else if (area_ID == "Receiver")
        {
            event_Manager.TeleportItem_Confirm(false);
        }
    }
}
