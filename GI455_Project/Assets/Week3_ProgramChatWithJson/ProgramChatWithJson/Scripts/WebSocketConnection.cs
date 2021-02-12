using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnection : MonoBehaviour
    {
        public class MessageData
        {
            public string username;
            public string message;
        }

        public GameObject rootConnection;
        public GameObject rootMessenger;

        public InputField inputText;
        public InputField inputUsername;
        public Text sendText;
        public Text receiveText;
        
        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:5500/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            rootConnection.SetActive(false);
            rootMessenger.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData newMessageData = new MessageData();
            newMessageData.username = inputUsername.text;
            newMessageData.message = inputText.text;

            string toJSONStr = JsonUtility.ToJson(newMessageData);

            ws.Send(toJSONStr);
            inputText.text = "";
        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                MessageData recieveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

                if(recieveMessageData.username == inputUsername.text)
                {
                    sendText.text += recieveMessageData.username + ": " + recieveMessageData.message + "\n";
                }
                else
                {
                    receiveText.text += recieveMessageData.username + ": " + recieveMessageData.message + "\n";
                }
                tempMessageString = string.Empty;
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            tempMessageString = messageEventArgs.Data;
        }
    }
}
