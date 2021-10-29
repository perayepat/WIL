using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Recharge_Station_Instance : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI numPanel;
    public string panelOutput = "+1";
    bool red, blue, green, white, magenta, cyan, yellow;
    [SerializeField] int inputCount = 0;
    [SerializeField] int currentPowerLevel = 0;
    [SerializeField] int powerCorePowerLevel = 0;

    [SerializeField] Cell_Port_Script cellPort;
    [SerializeField] Recharge_Station_Button_Script[] color_Buttons_OBJ;
    [SerializeField] Energy_Cell_Script Energy_Cell_;
    string newCell_Mat;
    string powerLevelString = "";



    // Start is called before the first frame update
    void Start()
    {
        numPanel.text = panelOutput;
    }



    public void InputColorData(string color_value)
    {
        inputCount += 1;

        //Update PANEL
        if(inputCount >= 2)
        {
            numPanel.text = "+" + (inputCount - 1) + powerLevelString;

        }




        if (color_value == "Red")
        {
            red = true;
        }
        if (color_value == "Blue")
        {
            blue = true;
        }
        if (color_value == "Green")
        {
            green = true;
        }
        if (color_value == "White")
        {
            white = true;
        }
        if (color_value == "Magenta")
        {
            magenta = true;
        }
        if (color_value == "Cyan")
        {
            cyan = true;
        }
        if (color_value == "Yellow")
        {
            yellow = true;
        }
    }




    public void ValidateCombination()
    {
        if(inputCount > 0 && currentPowerLevel >= 1)
        {
            //IF Magenta Output
            if (red && blue && inputCount == 2)
            {
                newCell_Mat = "Magenta";
                CompleteCombination();
                Debug.Log("Magenta");
            }

            //IF Cyan Output
            else if (blue && green && inputCount == 2)
            {
                newCell_Mat = "Cyan";
                CompleteCombination();
                Debug.Log("Cyan");
            }

            //IF White Output
            else if (inputCount == 6)
            {
                if (red && blue && green && magenta && yellow && cyan)
                {
                    newCell_Mat = "White";
                    CompleteCombination();
                    Debug.Log("White");
                }
            }

            //If invalid combination
            else
            {
                ResetColors();
                Debug.Log("Invalid Color Combination");
            }
        }
        else
        {
            ResetColors();
            numPanel.text = "Invalid";
        }

    }





    void CompleteCombination()
    {
        //The Core should become availible to the player (use coroutine)
        StartCoroutine(cellPort.ReleasePowerCell(newCell_Mat, currentPowerLevel));
        newCell_Mat = "";
        ResetColors();
        numPanel.text = "O_O";
    }





    public void ChangePowerLevel(int value)
    {
        currentPowerLevel += value;
        powerLevelString = "_" + currentPowerLevel + "V";

        if(inputCount >= 2)
        {
            numPanel.text = "+" + (inputCount - 1) + powerLevelString;
        }
        else
        {
            numPanel.text = powerLevelString;
        }


        //Optional Mechanic
        //if (currentPowerLevel >= 10)
        //{
        //    ResetColors();
        //    numPanel.text = "High V";
        //}
    }





    public void ResetColors()
    {
        red = false;
        blue = false;
        green = false;
        white = false;
        magenta = false;
        cyan = false;
        yellow = false;

        inputCount = 0;
        currentPowerLevel = 0;
        powerLevelString = "";
        numPanel.text = "[----]";

        for (int i = 0; i < color_Buttons_OBJ.Length; i++)
        {
            color_Buttons_OBJ[i].ResetColorButton();
        }

        Debug.Log("Reset Station");
    }




    public IEnumerator ProcessData()
    {
        numPanel.text = "[Busy]";

        yield return new WaitForSeconds(2f);

        if (cellPort.hasPowerCell)
        {
            ValidateCombination();
        }
        else
        {
            numPanel.text = "[No_Cell]";
        }
    }

    public IEnumerator RemoveData()
    {
        numPanel.text = "[Busy]";

        yield return new WaitForSeconds(2f);

        ResetColors();
    }
}
