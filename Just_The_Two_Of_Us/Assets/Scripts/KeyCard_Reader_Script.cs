using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCard_Reader_Script : MonoBehaviour
{
    Event_Manager @event;
    [SerializeField] int requiredKeycard_ID;

    // Start is called before the first frame update
    void Start()
    {
        @event = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Event_Manager>();
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Keycard")
        {
            @event.UpdateKeyCardStatus(requiredKeycard_ID, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Keycard")
        {
            @event.UpdateKeyCardStatus(requiredKeycard_ID, false);
        }
    }
}
