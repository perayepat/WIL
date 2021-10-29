using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core_Button_Press_Script : MonoBehaviour
{
    public enum LeverState
    {
        On,
        Off
    }

    [SerializeField] Event_Manager event_;

    [SerializeField] Animator coreLever_Anim;
    public LeverState leverState;
    [SerializeField] bool canSwitchBack_ON;
    [SerializeField] bool switchDelay = true;
    [SerializeField] MeshRenderer coreLightStatus;
    [SerializeField] Material[] coreLight_Mat;



    public void CoreButtonPressed()
    {
        if(leverState == LeverState.On && switchDelay)
        {
            coreLever_Anim.Play("Core_Switch_OFF_Anim");
            leverState = LeverState.Off;
            switchDelay = false;
        }
        else if (leverState == LeverState.Off && canSwitchBack_ON && switchDelay)
        {
            coreLever_Anim.Play("Core_Switch_ON_Anim");
            leverState = LeverState.On;
            switchDelay = false;
        }
    }




    public void AnimSwitch_Complete()
    {
        if (leverState == LeverState.On)
        {
            //
            switchDelay = true;
            coreLightStatus.material = coreLight_Mat[0];
        }
        else if (leverState == LeverState.Off)
        {
            //
            switchDelay = true;
            coreLightStatus.material = coreLight_Mat[1];
            //Turn Core OFF
            event_.ChangeCorePowerState(false);
        }
    }
}
