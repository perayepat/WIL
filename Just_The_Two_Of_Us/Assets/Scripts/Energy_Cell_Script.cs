using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy_Cell_Script : MonoBehaviour
{
    public int cellPowerLevel = 0;
    public bool inHand = false;
    public bool inChamber = false;
    [SerializeField] MeshRenderer cell_Mat;
    [SerializeField] Material[] materials;


    public void SetCell_Mat(string value)
    {
        if(value == "Magenta")
        {
            cell_Mat.material = materials[0];
        }
        else if (value == "Cyan")
        {
            cell_Mat.material = materials[1];
        }
        else if (value == "White")
        {
            cell_Mat.material = materials[2];
        }
        else
        {
            Debug.Log("No Data");
        }
    }
}
