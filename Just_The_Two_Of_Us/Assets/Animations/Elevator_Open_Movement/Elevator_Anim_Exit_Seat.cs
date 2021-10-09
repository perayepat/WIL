using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator_Anim_Exit_Seat : MonoBehaviour
{
    [SerializeField] Player_Controller_Script player;

    private void ExitSeat()
    {
        player.ExitSeat_Elevator();
    }
}
