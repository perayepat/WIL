using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    Event_Manager @event;

    [Header("Player/Spawn Data")]
    [SerializeField] Player_Controller_Script playerObj;
    [SerializeField] GameObject spawnResPoint;
    public Transform iceDomeSpawnPoint;
    public Transform mainHubSpawnPoint;
    public bool spawnPlayerAtResPoint = false;

    [Header("Temperature Data")]
    public float currentTemperature = -10;
    public float localTemperature;
    [SerializeField] float TempIncreaseInterval_sec = 5;
    [SerializeField] float worldTime = 0;
    [SerializeField] float initialDesertTemperature = 38;
    [SerializeField] float timeInHazardArea;
    [SerializeField] float healthDecreaseInterval_sec = 5;
    [SerializeField] float healthDecreaseInterval_Multiplier = 0.5f;
    private float time = 0;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI temperature_UI;
    [SerializeField] TextMeshProUGUI playerHealth_UI;


    private void Awake()
    {
        @event = GetComponent<Event_Manager>();
    }


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
            if(playerObj.playerEnvironmentState == Player_Controller_Script.PlayerEnvironmentState.InDesert)
            {
                UpdateTemperatureDataPoints(initialDesertTemperature);
            }
            else if(playerObj.playerEnvironmentState == Player_Controller_Script.PlayerEnvironmentState.Not_InDesert)
            {
                UpdateTemperatureDataPoints(0f);
            }
        }

        UpdatePlayerHealth();

        playerHealth_UI.text = playerObj.RetrievePlayerHealth().ToString("0") + "%";
    }


    void UpdatePlayerHealth()
    {
        if(localTemperature >= 30)
        {
            timeInHazardArea += Time.deltaTime;

            if(timeInHazardArea >= healthDecreaseInterval_sec)
            {
                playerObj.DamagePlayerHealth(((localTemperature * 10) / 100) * healthDecreaseInterval_Multiplier);
                timeInHazardArea = 0;
            }
        }
        if(playerObj.RetrievePlayerHealth() <= 0)
        {
            KillPlayer();
        }
    }



    void UpdateTemperatureDataPoints(float temperatureEffect)
    {
        currentTemperature += 1;
        time = 0;

        localTemperature = temperatureEffect + currentTemperature;

        temperature_UI.text = localTemperature.ToString("0");
    }



    public void NewPlayerPosition(Transform newPos)
    {
        if (playerObj != null)
        {
            Vector3 offset = new Vector3(0, 2, 0);
            playerObj.transform.position = newPos.position + offset;
        }
    }



    void KillPlayer()
    {
        @event.RestartGameFadeOut();
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
