using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recharge_Station_Button_Script : MonoBehaviour
{
    public enum Button_Type
    {
        Color,
        Reset,
        Enter
    }

    Recharge_Station_Instance Station_;
    MeshRenderer material;

    [Header("FOR COLOR BUTTONS ONLY")]
    public string button_Color_ID;
    [SerializeField] Material mainMat;
    [SerializeField] Material emmisionMat;
    public bool pressCheck = false;

    [Header("Button Type")]
    public Button_Type button_Type;
    bool canPressDelayCheck = true;




    private void Awake()
    {
        Station_ = gameObject.GetComponentInParent<Recharge_Station_Instance>();
        material = gameObject.GetComponent<MeshRenderer>();
    }



    public void PressColorButton()
    {
        if (Station_ && !pressCheck && button_Type == Button_Type.Color)
        {
            Station_.InputColorData(button_Color_ID);
            material.material = emmisionMat;
            pressCheck = true;
        }
    }



    public void ResetColorButton()
    {
        if (button_Type == Button_Type.Color)
        {
            material.material = mainMat;
            pressCheck = false;
        }
    }



    //Only for ENTER Button
    public void PressEnterButton()
    {
        if(canPressDelayCheck == true)
        {
            StartCoroutine(Station_.ProcessData());
            StartCoroutine(EnterResetButtonPressDelay());
        }
    }



    //Used for Reset Button Only
    public void PressResetButton()
    {
        if(canPressDelayCheck == true)
        {
            StartCoroutine(Station_.RemoveData());
            StartCoroutine(EnterResetButtonPressDelay());
        }
    }




    IEnumerator EnterResetButtonPressDelay()
    {
        canPressDelayCheck = false;
        yield return new WaitForSeconds(2f);
        canPressDelayCheck = true;
    }
}
