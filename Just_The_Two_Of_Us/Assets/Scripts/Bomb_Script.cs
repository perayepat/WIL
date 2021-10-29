using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Script : MonoBehaviour
{
    public enum BombCurrentLocation
    {
        Desert,
        MainHub,
        Cryo
    }

    Game_Manager game_Manager;

    public BombCurrentLocation bombCurrentLocation;
    [SerializeField] Animator blast_Volume_Anim;
    bool check = false;




    private void Awake()
    {
        game_Manager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Game_Manager>();
    }




    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && !check)
        {
            blast_Volume_Anim.Play("Bomb_Volume_Active_Anim");
        }

        //Checks if bomb is in the desert
        if (other.tag == "Desert_Volume")
        {
            bombCurrentLocation = BombCurrentLocation.Desert;
        }
        else if(other.tag == "Bomb_Cryo")
        {
            bombCurrentLocation = BombCurrentLocation.Cryo;
        }
    }




    private void OnTriggerExit(Collider other)
    {
        //Checks if bomb is in the desert
        if (other.tag == "Desert_Volume")
        {
            bombCurrentLocation = BombCurrentLocation.MainHub;
        }
        else if (other.tag == "Bomb_Cryo")
        {
            bombCurrentLocation = BombCurrentLocation.MainHub;
        }
    }




    //This is used to tell the game manager that the player should die (Bomb)
    public void CommandPlayerDeath()
    {
        game_Manager.KillPlayer();
    }
}
