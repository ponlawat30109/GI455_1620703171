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

        struct UserData
        {
            public string username;
            public string password;
            public string displayname;
            public UserData(string _username, string _password, string _displayname)
            {
                username = _username;
                password = _password;
                displayname = _displayname;
            }
        }

        [System.Serializable]
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

        public static string message;

        

        public InputField usernameInputField;
        public InputField passwordInputField;
        public InputField registerUsernameInputField;
        public InputField registerDisplayNameInputField;
        public InputField registerPasswordInputField;
        public InputField registerRePasswordInputField;
        public InputField messageInputField;
        public InputField roomnameInputField;

        public Text roomnameHeaderText;
        public Text welcomeChatText;
        public Text chatText;
        public Text errorText;
        public Text UsernameText;

        public Button loginButton;
        public Button gotoRegisterPanel;
        public Button registerButton;
        public Button sendButton;
        public Button leaveButton;

        public Transform content;

        public GameObject loginPanel;
        public GameObject loginFailedPanel;
        public GameObject registerPanel;
        public GameObject registerFailPanel;
        public GameObject roomlistBoard;
        public GameObject failedPanel;
        public GameObject chatBoard;

        [SerializeField] List<Text> chatCount = new List<Text>();

        [SerializeField] List<MessageData> messageList = new List<MessageData>();
        // [SerializeField] List<UserData> userList = new List<UserData>();

        public delegate void ChatHandler(string eventCheck);
        public ChatHandler OnCreateRoom;
        public ChatHandler OnJoinRoom;
        public ChatHandler OnLeaveRoom;
        public ChatHandler OnSendmessage;
        public ChatHandler OnLogin;
        public ChatHandler OnRegister;
        public ChatHandler OnChatUpdate;

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
                        displayname = eventCheck.data;
                        OnLogin(eventCheck.status);
                        // if (eventCheck.status == "success")
                        // {
                        //     loginPanel.SetActive(false);
                        //     roomlistBoard.SetActive(true);
                        //     usernameInputField.text = string.Empty;
                        //     passwordInputField.text = string.Empty;
                        //     username = eventCheck.data;    
                        //     UsernameText.text = $"Connect as : [{username}]";
                        // }
                        // else
                        // {
                        //     loginFailedPanel.SetActive(true);
                        // }
                        break;
                    case "Register":
                        OnRegister(eventCheck.status);
                        // if (eventCheck.status == "success")
                        // {
                        //     loginPanel.SetActive(true);
                        //     registerPanel.SetActive(false);
                        //     registerUsernameInputField.text = string.Empty;
                        //     registerDisplayNameInputField.text = string.Empty;
                        //     registerPasswordInputField.text = string.Empty;
                        //     registerRePasswordInputField.text = string.Empty;
                        // }
                        // else
                        // {
                        //     registerFailPanel.SetActive(true);
                        // }
                        break;
                    case "CreateRoom":
                        OnCreateRoom(eventCheck.status);
                        // if (eventCheck.status == "success")
                        // {
                        //     roomlistBoard.SetActive(false);
                        //     chatBoard.SetActive(true);

                        //     roomnameHeaderText.text = $"Room : [{roomname}]";
                        //     welcomeChatText.text = $"\n<b>Welcome {username} to {roomname} Chatroom</b>\n";
                        //     welcomeChatText.alignment = TextAnchor.UpperCenter;
                        // }
                        // else
                        // {
                        //     failedPanel.SetActive(true);
                        //     errorText.text = "Failed to create room.\nRoom already exist";
                        // }
                        break;
                    case "JoinRoom":
                        OnJoinRoom(eventCheck.status);
                        // if (eventCheck.status == "success")
                        // {
                        //     roomlistBoard.SetActive(false);
                        //     chatBoard.SetActive(true);

                        //     roomnameHeaderText.text = $"Room : [{roomname}]";
                        //     welcomeChatText.text = $"\n<b>Welcome {username} to {roomname} Chatroom</b>\n";
                        //     welcomeChatText.alignment = TextAnchor.UpperCenter;
                        // }
                        // else
                        // {
                        //     failedPanel.SetActive(true);
                        //     errorText.text = "Failed to join room.\nRoom is not exist";
                        // }
                        break;
                    case "LeaveRoom":
                        OnLeaveRoom(eventCheck.status);
                        //GameObject[] chatCount = GameObject.FindGameObjectsWithTag("ChatBox");
                        //foreach (GameObject count in chatCount)
                        // foreach (Text count in chatCount)
                        // {
                        //     Destroy(count);
                        // }
                        // chatCount.Clear();
                        // roomlistBoard.SetActive(true);
                        // chatBoard.SetActive(false);
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

                message = $"<color={recieveMessageData.color}>{recieveMessageData.userName}</color> : {recieveMessageData.message}";
                OnChatUpdate(recieveMessageData.userName);
                // if (recieveMessageData.userName == username)
                // {
                //     Text newTextbox = Instantiate(chatText, content) as Text;
                //     newTextbox.transform.SetParent(content.transform);
                //     newTextbox.text = string.Empty;
                //     newTextbox.alignment = TextAnchor.UpperRight;
                //     newTextbox.text += $"<color={recieveMessageData.color}>{recieveMessageData.userName}</color> : {recieveMessageData.message}";
                //     chatCount.Add(newTextbox);
                // }
                // else
                // {
                //     Text newTextbox = Instantiate(chatText, content) as Text;
                //     newTextbox.transform.SetParent(content.transform);
                //     newTextbox.text = string.Empty;
                //     newTextbox.alignment = TextAnchor.UpperLeft;
                //     newTextbox.text += $"<color={recieveMessageData.color}>{recieveMessageData.userName}</color> : {recieveMessageData.message}";
                //     chatCount.Add(newTextbox);
                // }

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