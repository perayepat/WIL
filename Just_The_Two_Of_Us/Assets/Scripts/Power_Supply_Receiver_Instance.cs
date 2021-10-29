using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Supply_Receiver_Instance : MonoBehaviour
{
    [SerializeField] Recharge_Station_Instance Recharge_;


    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Power_Supply" && other != null)
        {
            Recharge_.ChangePowerLevel(other.GetComponent<Power_Supply_Script>().powerLevel);
            Destroy(other.gameObject);
        }
    }
}
