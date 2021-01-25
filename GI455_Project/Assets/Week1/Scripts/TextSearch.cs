using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSearch : MonoBehaviour
{
    public GameObject InputField;
    public Text resultText, dataText;
    public List<string> textData = new List<string>();

    void Start()
    {
        foreach (string dataName in textData)
        {
            print(dataName);
            dataText.text += dataName + "\n";
        }
    }

    public void CheckName()
    {
        name = InputField.GetComponent<Text>().text;

        if (textData.Contains(name))
        {
            print(name + " is found");
            resultText.text = "[ <color=green>" + name + "</color> ] is found.";
        }
        else
        {
            print(name + " is not found");
            resultText.text = "[ <color=red>" + name + "</color> ] is not found.";
        }
    }
}
