using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MessengerProgram
{
    public class ChatUI : MonoBehaviour
    {
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

        string roomname;
        string username;
        string displayname;
        string registerUsername;
        string registerDisplayname;
        string registerPassword;
        string registerRePassword;
        string message;

        void Start()
        {
            WebsocketConnection.instance.OnCreateRoom += OnCreateRoom;
            WebsocketConnection.instance.OnJoinRoom += OnJoinRoom;
            WebsocketConnection.instance.OnLeaveRoom += OnLeaveRoom;
            WebsocketConnection.instance.OnLogin += OnLogin;
            WebsocketConnection.instance.OnRegister += OnRegister;
            WebsocketConnection.instance.OnChatUpdate += OnChatUpdate;
        }

        void OnCreateRoom(string status)
        {
            displayname = WebsocketConnection.displayname;
            roomname = WebsocketConnection.roomname;
            if (status == "success")
            {
                roomlistBoard.SetActive(false);
                chatBoard.SetActive(true);

                roomnameHeaderText.text = $"Room : [{roomname}]";
                welcomeChatText.text = $"\n<b>Welcome {displayname} to {roomname} Chatroom</b>\n";
                welcomeChatText.alignment = TextAnchor.UpperCenter;
            }
            else
            {
                failedPanel.SetActive(true);
                errorText.text = "Failed to create room.\nRoom already exist";
            }
        }

        void OnJoinRoom(string status)
        {
            displayname = WebsocketConnection.displayname;
            roomname = WebsocketConnection.roomname;
            if (status == "success")
            {
                roomlistBoard.SetActive(false);
                chatBoard.SetActive(true);

                roomnameHeaderText.text = $"Room : [{roomname}]";
                welcomeChatText.text = $"\n<b>Welcome {displayname} to {roomname} Chatroom</b>\n";
                welcomeChatText.alignment = TextAnchor.UpperCenter;
            }
            else
            {
                failedPanel.SetActive(true);
                errorText.text = "Failed to join room.\nRoom is not exist";
            }
        }

        void OnLeaveRoom(string status)
        {
            foreach (Text count in chatCount)
            {
                Destroy(count);
            }
            chatCount.Clear();
            roomlistBoard.SetActive(true);
            chatBoard.SetActive(false);
        }

        void OnLogin(string status)
        {
            displayname = WebsocketConnection.displayname;

            if (status == "success")
            {
                loginPanel.SetActive(false);
                roomlistBoard.SetActive(true);
                usernameInputField.text = string.Empty;
                passwordInputField.text = string.Empty;
                UsernameText.text = $"Connect as : [{displayname}]";
            }
            else
            {
                loginFailedPanel.SetActive(true);
            }
        }

        void OnRegister(string status)
        {
            if (status == "success")
            {
                loginPanel.SetActive(true);
                registerPanel.SetActive(false);
                registerUsernameInputField.text = string.Empty;
                registerDisplayNameInputField.text = string.Empty;
                registerPasswordInputField.text = string.Empty;
                registerRePasswordInputField.text = string.Empty;
            }
            else
            {
                registerFailPanel.SetActive(true);
            }
        }

        void OnChatUpdate(string _username)
        {
            message = WebsocketConnection.message;
            username = WebsocketConnection.username;
            
            if (_username == username)
            {
                Text newTextbox = Instantiate(chatText, content) as Text;
                newTextbox.transform.SetParent(content.transform);
                newTextbox.text = string.Empty;
                newTextbox.alignment = TextAnchor.UpperRight;
                newTextbox.text += $"{message}";
                chatCount.Add(newTextbox);
            }
            else
            {
                Text newTextbox = Instantiate(chatText, content) as Text;
                newTextbox.transform.SetParent(content.transform);
                newTextbox.text = string.Empty;
                newTextbox.alignment = TextAnchor.UpperLeft;
                newTextbox.text += $"{message}";
                chatCount.Add(newTextbox);
            }
        }
    }
}
