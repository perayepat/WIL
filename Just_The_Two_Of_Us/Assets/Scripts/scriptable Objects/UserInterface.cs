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


    [Header("UI Values")]
    public float temperature;
    public int InvetorySize;
    public List<Color32> TempgaugeColors;
    public List<Image> PlayerCondition;
    public List<Image> InventortyImages;
    [SerializeField] float health = 100;
    public snowLevels sLevels;
    public desertLevels dLelevels;
    

    [Header("UI")]
    [SerializeField] TextMeshProUGUI Interact_Grab_UI;
    [SerializeField] TextMeshProUGUI Interact_PressButton_UI;
    [SerializeField] TextMeshProUGUI Health_UI;
    [SerializeField] RawImage Minimap;


    [Header("UI GameObjects")]
    [SerializeField] GameObject TempratureGauge;
    [SerializeField] GameObject HealthGauge;
    [SerializeField] GameObject InteractableField;
}

