using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keypad_Instance : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI numPanel;
    public string panelOutput = "";

    // Start is called before the first frame update
    void Start()
    {
        numPanel.text = panelOutput;
    }

    public void AppendKeypadInput(string value)
    {
        if(panelOutput.Length < 6)
        {
            panelOutput += value;
            numPanel.text = panelOutput;
        }
        else
        {
            panelOutput = "";
            numPanel.text = panelOutput;
        }
    }
}
