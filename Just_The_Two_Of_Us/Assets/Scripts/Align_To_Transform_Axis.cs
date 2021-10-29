using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align_To_Transform_Axis : MonoBehaviour
{
    public enum Axis_To_Align
    {
        X_Axis, 
        Y_Axis, 
        Z_Axis
    }

    public Transform followTransform;
    public Axis_To_Align align_Axis;
    Vector3 newPos;


    // Update is called once per frame
    void Update()
    {
        if(align_Axis == Axis_To_Align.X_Axis)
        {
            newPos = new Vector3(followTransform.position.x, transform.position.y, transform.position.z);

            if(transform.position != newPos)
            {
                transform.position = newPos;
            }
        }
        else if (align_Axis == Axis_To_Align.Y_Axis)
        {
            newPos = new Vector3(transform.position.x, followTransform.position.y, transform.position.z);

            if (transform.position != newPos)
            {
                transform.position = newPos;
            }
        }
        else if (align_Axis == Axis_To_Align.Z_Axis)
        {
            newPos = new Vector3(transform.position.x, transform.position.y, followTransform.position.z);

            if (transform.position != newPos)
            {
                transform.position = newPos;
            }
        }
    }
}
