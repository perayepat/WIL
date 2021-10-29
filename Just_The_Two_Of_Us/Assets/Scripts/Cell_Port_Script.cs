using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell_Port_Script : MonoBehaviour
{
    [SerializeField] Energy_Cell_Script powerCell_OBJ;
    public bool hasPowerCell;
    [SerializeField] bool powerCell_ExitCheck = true;
    [SerializeField] Animator cellCover_Anim;
    [SerializeField] GameObject cell_Hologram;


    public void CellCoverAnimState(string state)
    {
        if (state == "Open")
        {
            cellCover_Anim.Play("Cell_Cover_Open_Anim");
        }
        else if(state == "Close")
        {
            cellCover_Anim.Play("Cell_Cover_Close_Anim");
        }
    }


    void CellHologramState(bool state)
    {
        cell_Hologram.SetActive(state);
    }



    private void UpdatePowerCell_Physics(bool value)
    {
        if(value == true)
        {
            powerCell_OBJ.GetComponent<Rigidbody>().isKinematic = true;
            powerCell_OBJ.GetComponent<CapsuleCollider>().isTrigger = true;
            powerCell_OBJ.transform.position = cell_Hologram.transform.position;
            powerCell_OBJ.transform.rotation = cell_Hologram.transform.rotation;
        }
        else if(value == false)
        {
            powerCell_OBJ.GetComponent<Rigidbody>().isKinematic = false;
            powerCell_OBJ.GetComponent<CapsuleCollider>().isTrigger = false;
            //powerCell_OBJ.transform.position = cell_Hologram.transform.position;
            powerCell_OBJ.inChamber = false;
            powerCell_OBJ = null;
        }
    }





    public IEnumerator ReleasePowerCell(string value, int newPowerLevel)
    {
        CellCoverAnimState("Open");
        powerCell_OBJ.SetCell_Mat(value);
        powerCell_OBJ.cellPowerLevel = newPowerLevel;

        yield return new WaitForSeconds(3.5f);

        UpdatePowerCell_Physics(false);
        hasPowerCell = false;
        StartCoroutine(CoolDown());
    }




    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(4f);

        powerCell_ExitCheck = true;
    }




    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Power_Cell" && hasPowerCell == false)
        {
            CellHologramState(true);
            if (other.GetComponent<Energy_Cell_Script>().inHand == false && powerCell_ExitCheck == true)
            {
                powerCell_OBJ = other.GetComponent<Energy_Cell_Script>();
                powerCell_OBJ.inChamber = true;
                UpdatePowerCell_Physics(true);
                hasPowerCell = true;
                powerCell_ExitCheck = false;
                CellCoverAnimState("Close");
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Power_Cell")
        {
            CellHologramState(false);
            //powerCell_ExitCheck = true;
            //if (other.GetComponent<Energy_Cell_Script>().inHand == false && hasPowerCell == false)
            //{
            //    UpdatePowerCell_Physics(false);
            //    hasPowerCell = false;
            //}
        }
    }
}
