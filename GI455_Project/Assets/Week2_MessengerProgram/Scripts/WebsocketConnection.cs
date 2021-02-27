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
        public static WebsocketConnection instance;

        struct LoginData
        {
            public string username;
            public string password;
            public LoginData(string _username, string _password)
            {
                username = _username;
                password = _password;
            }
        }

        struct RegisterData
        {
            public string username;
            public string displayname;
            public string password;
            public string repassword;
            public RegisterData(string _username, string _displayname, string _password, string _repassword)
            {
                username = _username;
                displayname = _displayname;
                password = _password;
                repassword = _repassword;
            }
        }
        [System.Serializable]
        public struct UserData
        {
            public string UserID;
            public string Password;
            public string Name;
            public UserData(string _username, string _password, string _displayname)
            {
                UserID = _username;
                Password = _password;
                Name = _displayname;
            }
        }

        [System.Serializable]
        public struct MessageData
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

        public struct SocketEvent
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
        public static string roomname;
        public static string username;
        public static string displayname;
        public static string password;
        string registerUsername;
        string registerDisplayname;
        string registerPassword;
        string registerRePassword;

        string textMessage;
        string tempMessageString;

        public InputField usernameInputField;
        public InputField passwordInputField;
        public InputField registerUsernameInputField;
        public InputField registerDisplayNameInputField;
        public InputField registerPasswordInputField;
        public InputField registerRePasswordInputField;
        public InputField messageInputField;
        public InputField roomnameInputField;

        // public Text roomnameHeaderText;
        // public Text welcomeChatText;
        // public Text chatText;
        // public Text errorText;
        // public Text UsernameText;

        // public Button loginButton;
        // public Button gotoRegisterPanel;
        // public Button registerButton;
        // public Button sendButton;
        // public Button leaveButton;

        // public Transform content;

        // public GameObject loginPanel;
        public GameObject loginFailedPanel;
        // public GameObject registerPanel;
        public GameObject registerFailPanel;
        // public GameObject roomlistBoard;
        // public GameObject failedPanel;
        // public GameObject chatBoard;

        [SerializeField] List<Text> chatCount = new List<Text>();

        [SerializeField] List<MessageData> messageList = new List<MessageData>();
        [SerializeField] List<UserData> userInfo = new List<UserData>();

        public delegate void ChatHandler(SocketEvent eventCheck);
        public event ChatHandler OnCreateRoom;
        public event ChatHandler OnJoinRoom;
        public event ChatHandler OnLeaveRoom;
        public event ChatHandler OnSendmessage;
        public event ChatHandler OnLogin;
        public event ChatHandler OnRegister;
        public event ChatHandler OnChatUpdate;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //Debug.Log("Connected");

            websocket = new WebSocket($"ws://{url}:{port}/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            //sendButton.onClick.AddListener(GetText);
            //leaveButton.onClick.AddListener(LeaveChat);
        }

        private void Update()
        {
            EventCheck();
            //ChatUpdate();
        }

        void EventCheck()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent eventCheck = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                switch (eventCheck.eventName)
                {
                    case "Login":
                        // displayname = eventCheck.data;

                        UserData newUserInfo = JsonUtility.FromJson<UserData>(eventCheck.data);
                        userInfo.Add(newUserInfo);
                        displayname = newUserInfo.Name;
                        if (OnLogin != null)
                            OnLogin(eventCheck);
                        break;
                    case "Register":
                        if (OnRegister != null)
                            OnRegister(eventCheck);
                        break;
                    case "CreateRoom":
                        if (OnCreateRoom != null)
                            OnCreateRoom(eventCheck);
                        break;
                    case "JoinRoom":
                        if (OnJoinRoom != null)
                            OnJoinRoom(eventCheck);
                        break;
                    case "LeaveRoom":
                        if (OnLeaveRoom != null)
                            OnLeaveRoom(eventCheck);
                        break;
                    case "SendMessage":
                        ChatUpdate(eventCheck);
                        break;
                }

                tempMessageString = string.Empty;
            }
        }

        public void ChatUpdate(SocketEvent eventCheck)
        {

            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent recieveMsgEvent = JsonUtility.FromJson<SocketEvent>(tempMessageString);
                MessageData recieveMessageData = JsonUtility.FromJson<MessageData>(recieveMsgEvent.data);

                OnChatUpdate(recieveMsgEvent);

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

        public void Login()
        {
            username = usernameInputField.text;
            password = passwordInputField.text;

            LoginData newLoginData = new LoginData();
            newLoginData.username = username;
            newLoginData.password = password;

            string toJSONStr = JsonUtility.ToJson(newLoginData);
            Debug.Log(toJSONStr);

            if ((string.IsNullOrEmpty(username) == false) && (string.IsNullOrEmpty(password) == false))
            {
                if (websocket.ReadyState == WebSocketState.Open)
                {
                    SocketEvent newSocketEvent = new SocketEvent("Login", toJSONStr, "");
                    string jsonStr = JsonUtility.ToJson(newSocketEvent);
                    websocket.Send(jsonStr);
                }
            }
            else
            {
                loginFailedPanel.SetActive(true);
            }
        }

        public void Register()
        {
            registerUsername = registerUsernameInputField.text;
            registerDisplayname = registerDisplayNameInputField.text;
            registerPassword = registerPasswordInputField.text;
            registerRePassword = registerRePasswordInputField.text;

            RegisterData newRegisterData = new RegisterData();
            newRegisterData.username = registerUsername;
            newRegisterData.displayname = registerDisplayname;
            newRegisterData.password = registerPassword;
            newRegisterData.repassword = registerRePassword;

            string toJSONStr = JsonUtility.ToJson(newRegisterData);
            Debug.Log(toJSONStr);

            if ((string.IsNullOrEmpty(registerUsername) == false) && (string.IsNullOrEmpty(registerDisplayname) == false) && (string.IsNullOrEmpty(registerPassword) == false) && (string.IsNullOrEmpty(registerRePassword) == false))
            {
                if (registerPassword == registerRePassword)
                {
                    if (websocket.ReadyState == WebSocketState.Open)
                    {
                        SocketEvent newSocketEvent = new SocketEvent("Register", toJSONStr, "");
                        string jsonStr = JsonUtility.ToJson(newSocketEvent);
                        websocket.Send(jsonStr);
                    }
                }
                else
                {
                    registerFailPanel.SetActive(true);
                }
            }
            else
            {
                registerFailPanel.SetActive(true);
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
            textMessage = messageInputField.text;
            messageInputField.text = string.Empty;

            MessageData newMessageData = new MessageData();
            newMessageData.userName = displayname;
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