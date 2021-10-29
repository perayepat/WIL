using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Explode_Script : MonoBehaviour
{
    Event_Manager event_Manager;

    private void Awake()
    {
        event_Manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Event_Manager>();
    }

    public void Point_OF_No_Return()
    {
        event_Manager.inevitableExplosion = true;
    }
}
