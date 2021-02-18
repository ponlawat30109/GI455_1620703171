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
            public string roomName;
            public string status;

            public SocketEvent(string eventName, string roomName, string status)
            {
                this.eventName = eventName;
                this.roomName = roomName;
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

            sendButton.onClick.AddListener(GetText);
            //leaveButton.onClick.AddListener(LeaveChat);
        }

        private void Update()
        {
            IsJoinRoom();
            ChatUpdate();
        }

        void IsJoinRoom()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent eventCheck = JsonUtility.FromJson<SocketEvent>(tempMessageString);
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
                    if (eventCheck.eventName == "CreateRoom")
                    {
                        errorText.text = "Failed to create room.\nRoom already exist";
                    }
                    else if (eventCheck.eventName == "JoinRoom")
                    {
                        errorText.text = "Failed to join room.\nRoom is not exist";
                    }
                }

                if (eventCheck.eventName == "LeaveRoom")
                {
                    roomlistBoard.SetActive(true);
                    chatBoard.SetActive(false);
                }

                tempMessageString = string.Empty;
            }
        }

        public void ChatUpdate()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                MessageData recieveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

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
                //Debug.Log($"{messageList[messageList.Count -1].userName}: {messageList[messageList.Count - 1].message}");

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
                    //string currentMessage = string.Concat($"{username}: {textMessage}");
                    websocket.Send(toJSONStr);
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