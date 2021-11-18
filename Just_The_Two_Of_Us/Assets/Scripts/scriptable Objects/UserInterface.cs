using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New Visor UI", menuName = "Visor UI")]
public class UserInterface : ScriptableObject
{
    public enum snowLevels
    {
        NORMAL,
        COLD,
        FREEZING
    }
    public enum desertLevels
    {
        NORMAL,
        WARM,
        ARRID
    }
    public enum EnviromentType
    {
        THUNDRA,
        DESERT
    }

    [SerializeField] Game_Manager manager;

    [Header("UI Values")]
    public float temperature;
    public int InvetorySize;
    public List<Color32> TempgaugeColors;
    public List<Image> PlayerCondition;
    public List<Image> InventortyImages;
    public snowLevels sLevels;
    public desertLevels dLelevels;
    public EnviromentType enviromentType;

    [Header("Player UI Values")]
    [SerializeField] float health = 100;
    [SerializeField] float startingTemp = 0;
 


    [Header("UI")]
    public TextMeshProUGUI Interact_Grab_UI;
    public TextMeshProUGUI Interact_PressButton_UI;
    public TextMeshProUGUI Health_UI;
    public TextMeshProUGUI temperature_UI;
    public TextMeshProUGUI playerHealth_UI;
    public RawImage Minimap;


    [Header("UI GameObjects")]
    [SerializeField] GameObject TempratureGauge;
    [SerializeField] GameObject HealthGauge;
    [SerializeField] GameObject InteractableField;

    

    
}

