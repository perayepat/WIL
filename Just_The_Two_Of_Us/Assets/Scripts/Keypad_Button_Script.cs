using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad_Button_Script : MonoBehaviour
{
    public int keypad_Num;
    Keypad_Instance Keypad_;

    private void Awake()
    {
        Keypad_ = gameObject.GetComponentInParent<Keypad_Instance>();
    }

    public Keypad_Instance ReturnKeyPadObj()
    {
        if (Keypad_)
        {
            return Keypad_;
        }
        else
        {
            return null;
        }

    }
}
