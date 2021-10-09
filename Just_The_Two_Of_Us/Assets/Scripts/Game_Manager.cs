using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    [Header("Player/Spawn Data")]
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject spawnResPoint;
    public Transform iceDomeSpawnPoint;
    public Transform mainHubSpawnPoint;
    public bool spawnPlayerAtResPoint = false;

    [Header("Temperature Data")]
    public float currentTemperature = -10;
    [SerializeField] float TempIncreaseInterval_sec = 5;
    [SerializeField] float worldTime = 0;
    private float time = 0;

    [SerializeField] TextMeshProUGUI temperature_UI;

    private void Start()
    {
        temperature_UI.text = currentTemperature.ToString("0");

        if (spawnPlayerAtResPoint && playerObj != null)
        {
            Vector3 offset = new Vector3(0, 2, 0);
            playerObj.transform.position = spawnResPoint.transform.position + offset;
        }
        else
        {
            Vector3 offset = new Vector3(0, 2, 0);
            playerObj.transform.position = iceDomeSpawnPoint.transform.position + offset;
        }
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


    public void NewPlayerPosition(Transform newPos)
    {
        if (playerObj != null)
        {
            Vector3 offset = new Vector3(0, 2, 0);
            playerObj.transform.position = newPos.position + offset;
        }
    }
}
