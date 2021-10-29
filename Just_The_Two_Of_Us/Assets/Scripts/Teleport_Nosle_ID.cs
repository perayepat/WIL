using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport_Nosle_ID : MonoBehaviour
{
    Event_Manager event_Manager;

    public string nosle_ID;
    [SerializeField] Animator teleport_Area_Anim;



    private void Awake()
    {
        event_Manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Event_Manager>();
    }



    public void Activate_Teleport_Area()
    {
        teleport_Area_Anim.Play("Teleport_Area_Activate_Anim");

        //if(nosle_ID == "Sender")
        //{
        //    event_Manager.TeleportItem_Confirm(true);
        //}
        //else if (nosle_ID == "Receiver")
        //{
        //    event_Manager.TeleportItem_Confirm(false);
        //}
    }
}
