using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MessengerProgram
{
    public class EnterData : MonoBehaviour
    {
        public GameObject urlInput, portInput, nameInput;
        public Button connectButton;
        public static string url, port, username;

        private void Start()
        {
            connectButton.onClick.AddListener(Connect);
        }

        public void Connect()
        {
            url = urlInput.GetComponent<Text>().text;
            port = portInput.GetComponent<Text>().text;
            username = nameInput.GetComponent<Text>().text;
            if (url == "")
            {
                url = "127.0.0.1";
            }

            if (port == "")
            {
                port = "5500";
            }

            //if (username != "")
            //{
            //    print($"{username} : {url}:{port}");
            //    SceneManager.LoadScene("MessengerProgram");
            //}
            SceneManager.LoadScene("MessengerProgram");
        }
    }
}
