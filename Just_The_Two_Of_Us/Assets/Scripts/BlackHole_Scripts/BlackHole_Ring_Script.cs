using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole_Ring_Script : MonoBehaviour
{
    Transform thisTransform;
    Outline outline;
    Event_Manager @event;
    [SerializeField] GameObject ringElements;

    [SerializeField] int ringID;
    [SerializeField] float startRot = 0;
    [SerializeField] float currentRot;
    [SerializeField] float rotSetInterval = 30;
    [SerializeField] float setInterval;
    [SerializeField] float RotSpeed = 10;
    [SerializeField] bool rotating = true;
    [SerializeField] bool invertRot = false;
    [SerializeField] bool lockRot = false;

    float rotDelta = 0;
    bool check = true;
    bool checkSet = false;
    int intCheck = 0;

    // Start is called before the first frame update
    void Awake()
    {
        thisTransform = this.transform;
        outline = GetComponentInChildren<Outline>();
        outline.OutlineWidth = 0;
        @event = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<Event_Manager>();
        ringElements.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (lockRot)
        {
            rotating = false;
        }


        if (!rotating)
        {
            //outline.OutlineWidth = 2;
        }
        else
        {
            //outline.OutlineWidth = 0;
        }


        //Rotation logic Below---------------------------------------------------------------------
        currentRot = thisTransform.localRotation.eulerAngles.y;


        if(rotating && check)
        {
            if(intCheck < 12)
            {
                intCheck++;
            }
            else
            {
                intCheck = 1;
            }
            setInterval = rotSetInterval * intCheck;
            check = false;
            rotating = true;

            if(setInterval == 360)
            {
                checkSet = true;
            }
        }

        //This is to trigger the stop Rotate Interval function
        if(currentRot >= setInterval && rotating && !checkSet)
        {
            rotating = false;
            checkSet = false;
            thisTransform.localRotation = Quaternion.Euler(0, setInterval, 0);
            StartCoroutine(RotateInterval());
        }
        //This is to reset the changed values when a revolution is complete
        if (checkSet && rotating && currentRot <= 1f)
        {
            rotating = false;
            checkSet = false;
            setInterval = 0;
            thisTransform.localRotation = Quaternion.Euler(0, 0, 0);
            StartCoroutine(RotateInterval());
        }



        if (rotating)
        {
            rotDelta += 0.01f;

            if (!invertRot)
            {
                thisTransform.localRotation = Quaternion.Euler(0, (0 + rotDelta) * RotSpeed, 0);
            }
            else
            {
                thisTransform.localRotation = Quaternion.Euler(0, (10 + rotDelta) * -RotSpeed, 0);
            }
        }

    }


    public void LockRingRotation()
    {
        StopAllCoroutines();

        if (lockRot == true)
        {
            lockRot = false;
            rotating = true;
            check = true;
            ringElements.SetActive(false);
            @event.UpdateRingBoolState(ringID, false);
        }
        else
        {
            lockRot = true;
            rotating = false;
            ringElements.SetActive(true);

            if (currentRot == 0)
            {
                @event.UpdateRingBoolState(ringID, true);
            }

        }
    }

    public bool RotationState()
    {
        return rotating;
    }


    IEnumerator RotateInterval()
    {
        rotating = false;

        yield return new WaitForSeconds(2f);

        rotating = true;
        check = true;
    }
}
