using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

namespace MessengerProgram
{
    public class WebsocketConnection : MonoBehaviour
    {
        private WebSocket websocket;

        struct MessageData
        {
            public string userName;
            public string message;
            public string color;
            public MessageData(string username, string message, string color)
            {
                this.userName = username;
                this.message = message;
                this.color = color;
            }
        }

        struct SocketEvent
        {
            public string eventName;
            public string data;
            public string status;

            public SocketEvent(string eventName, string data, string status)
            {
                this.eventName = eventName;
                this.data = data;
                this.status = status;
            }
        }

        string url = EnterData.url;
        string port = EnterData.port;
        string username = EnterData.username;
        string textMessage = "";
        string tempMessageString;

        string roomname = "";

        public InputField inputField;
        public InputField roomnameInputField;

        public Text chatText, headerText;
        public Text errorText;

        public Button sendButton, leaveButton;

        public Transform content;

        public GameObject chatBoard, roomlistBoard, failedPanel;

        [SerializeField] List<MessageData> messageList = new List<MessageData>();

        void Start()
        {
            Debug.Log("Connected");

            websocket = new WebSocket($"ws://{url}:{port}/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            //sendButton.onClick.AddListener(GetText);
            //leaveButton.onClick.AddListener(LeaveChat);
        }

        private void Update()
        {
            EventCheck();
            ChatUpdate();
        }

        void EventCheck()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent eventCheck = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                switch (eventCheck.eventName)
                {
                    case "CreateRoom":
                        if (eventCheck.status == "success")
                        {
                            roomlistBoard.SetActive(false);
                            chatBoard.SetActive(true);

                            headerText.text = $"{roomname}";
                            chatText.text = $"\n<b>Welcome {username} to {roomname} Chatroom</b>\n";
                            chatText.alignment = TextAnchor.UpperCenter;
                        }
                        else
                        {
                            failedPanel.SetActive(true);
                            errorText.text = "Failed to create room.\nRoom already exist";
                        }
                        break;
                    case "JoinRoom":
                        if (eventCheck.status == "success")
                        {
                            roomlistBoard.SetActive(false);
                            chatBoard.SetActive(true);

                            headerText.text = $"{roomname}";
                            chatText.text = $"\n<b>Welcome {username} to {roomname} Chatroom</b>\n";
                            chatText.alignment = TextAnchor.UpperCenter;
                        }
                        else
                        {
                            failedPanel.SetActive(true);
                            errorText.text = "Failed to join room.\nRoom is not exist";
                        }
                        break;
                    case "LeaveRoom":
                        roomlistBoard.SetActive(true);
                        chatBoard.SetActive(false);
                        break;
                    case "SendMessage":
                        ChatUpdate();
                        break;
                }

                tempMessageString = string.Empty;
            }
        }

        public void ChatUpdate()
        {

            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent recieveMsgEvent = JsonUtility.FromJson<SocketEvent>(tempMessageString);
                MessageData recieveMessageData = JsonUtility.FromJson<MessageData>(recieveMsgEvent.data);

                if (recieveMessageData.userName == username)
                {
                    Text newTextbox = Instantiate(chatText, content) as Text;
                    newTextbox.transform.SetParent(content.transform);
                    newTextbox.text = string.Empty;
                    newTextbox.alignment = TextAnchor.UpperRight;
                    newTextbox.text += $"<color={recieveMessageData.color}>{recieveMessageData.userName}</color> : {recieveMessageData.message}";
                }
                else
                {
                    Text newTextbox = Instantiate(chatText, content) as Text;
                    newTextbox.transform.SetParent(content.transform);
                    newTextbox.text = string.Empty;
                    newTextbox.alignment = TextAnchor.UpperLeft;
                    newTextbox.text += $"<color={recieveMessageData.color}>{recieveMessageData.userName}</color> : {recieveMessageData.message}";
                }

                messageList.Add(recieveMessageData);

                tempMessageString = string.Empty;
            }
        }

        private void OnDestroy()
        {
            if (websocket != null)
            {
                websocket.Close();
            }
        }

        public void CreateRoom(string roomName)
        {
            roomname = roomnameInputField.text;
            roomnameInputField.text = string.Empty;
            roomName = roomname;

            if (string.IsNullOrEmpty(roomname) == false)
            {
                if (websocket.ReadyState == WebSocketState.Open)
                {
                    SocketEvent newSocketEvent = new SocketEvent("CreateRoom", roomName, "");
                    string jsonStr = JsonUtility.ToJson(newSocketEvent);
                    websocket.Send(jsonStr);
                }
            }
        }

        public void JoinRoom(string roomName)
        {
            roomname = roomnameInputField.text;
            roomnameInputField.text = string.Empty;
            roomName = roomname;

            if (string.IsNullOrEmpty(roomname) == false)
            {
                if (websocket.ReadyState == WebSocketState.Open)
                {
                    SocketEvent newSocketEvent = new SocketEvent("JoinRoom", roomName, "");
                    string jsonStr = JsonUtility.ToJson(newSocketEvent);
                    websocket.Send(jsonStr);
                }
            }
        }

        public void LeaveRoom(string roomName)
        {
            roomName = roomname;

            if (websocket.ReadyState == WebSocketState.Open)
            {
                SocketEvent newSocketEvent = new SocketEvent("LeaveRoom", roomName, "");
                string jsonStr = JsonUtility.ToJson(newSocketEvent);
                websocket.Send(jsonStr);
            }
        }

        public void GetText()
        {
            textMessage = inputField.text;
            inputField.text = string.Empty;

            MessageData newMessageData = new MessageData();
            newMessageData.userName = username;
            newMessageData.message = textMessage;
            newMessageData.color = "#FFAC13";

            string toJSONStr = JsonUtility.ToJson(newMessageData);

            if (string.IsNullOrEmpty(textMessage) == false)
            {
                if (websocket.ReadyState == WebSocketState.Open)
                {
                    SocketEvent newSocketEvent = new SocketEvent("SendMessage", toJSONStr, "");
                    string jsonStr = JsonUtility.ToJson(newSocketEvent);
                    websocket.Send(jsonStr);
                }
            }
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            tempMessageString = messageEventArgs.Data;
            Debug.Log(tempMessageString);
        }

        public void Disconnected()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            Debug.Log("Disconnected");
        }
    }
}