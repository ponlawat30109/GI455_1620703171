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

            //string url = urlInput.GetComponent<Text>().text;
            //string port = portInput.GetComponent<Text>().text;
            //string name = nameInput.GetComponent<Text>().text;
            //if (url == "")
            //{
            //    url = "127.0.0.1";
            //}

            //if (port == "")
            //{
            //    port = "5500";
            //}

            //if (name == "")
            //{
            //    name = "Anonymous";
            //}

            //print($"{url}:{port}");
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

            if (username != "")
            {
                print($"{username} : {url}:{port}");
                SceneManager.LoadScene("MessengerProgram");
            }
        }
    }
}
