using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] float currentTemperature = -10;
    [SerializeField] float TempIncreaseInterval_sec = 5;
    [SerializeField] float worldTime = 0;
    private float time = 0;

    [SerializeField] TextMeshProUGUI temperature_UI;

    private void Start()
    {
        temperature_UI.text = currentTemperature.ToString("00");
    }

    // Update is called once per frame
    void Update()
    {
        worldTime += Time.deltaTime;
        time += Time.deltaTime;

        if(time >= TempIncreaseInterval_sec)
        {
            currentTemperature += 1;
            time = 0;

            temperature_UI.text = currentTemperature.ToString("00");
        }
    }
}
