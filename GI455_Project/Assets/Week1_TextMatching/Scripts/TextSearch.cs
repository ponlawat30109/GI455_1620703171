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
            Debug.Log(dataName);
            dataText.text += dataName + "\n";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CheckName();
        }
    }

    public void CheckName()
    {
        name = InputField.GetComponent<Text>().text;

        if (textData.Contains(name))
        {
            Debug.Log(name + " is found");
            resultText.text = $"[ <color=green>{name}</color> ] is found.";
        }
        else
        {
            Debug.Log(name + " is not found");
            resultText.text = $"[ <color=red>{name}</color> ] is not found.";
        }
    }
}
