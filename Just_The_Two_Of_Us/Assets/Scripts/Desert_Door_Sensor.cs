using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desert_Door_Sensor : MonoBehaviour
{
    [SerializeField] bool unlocked;
    public Animator door;
    [SerializeField] Material lockedMaterial;
    [SerializeField] Material unlockedMaterial;
    [SerializeField] MeshRenderer[] lockedStatus_Mesh;
    [SerializeField] Light[] light_;
    [SerializeField] Color locked_Light_Color;
    [SerializeField] Color unlocked_Light_Color;


    private void Start()
    {
        if (unlocked)
        {
            ChangeDoorLockedStatus(true);
        }
        else if (!unlocked)
        {
            ChangeDoorLockedStatus(false);
        }
    }


    public void ChangeDoorLockedStatus(bool value)
    {
        if (value)
        {
            unlocked = true;

            for (int i = 0; i < light_.Length; i++)
            {
                lockedStatus_Mesh[i].material = unlockedMaterial;
                light_[i].color = unlocked_Light_Color;
            }


        }
        else if (!value)
        {
            unlocked = false;

            for (int i = 0; i < light_.Length; i++)
            {
                lockedStatus_Mesh[i].material = lockedMaterial;
                light_[i].color = locked_Light_Color;
            }

        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && unlocked)
        {
            if(door != null)
            {
                door.SetBool("Open", true);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if(door != null)
            {
                door.SetBool("Open", false);
            }
        }
    }
}
