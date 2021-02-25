using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MessengerProgram
{
    public class ChatUI : MonoBehaviour
    {
        #region UI declarations
        
        public InputField usernameInputField;
        public InputField passwordInputField;
        public InputField registerUsernameInputField;
        public InputField registerDisplayNameInputField;
        public InputField registerPasswordInputField;
        public InputField registerRePasswordInputField;
        // public InputField messageInputField;
        // public InputField roomnameInputField;

        public Text roomnameHeaderText;
        public Text welcomeChatText;
        public Text chatText;
        public Text errorText;
        public Text UsernameText;

        // public Button loginButton;
        // public Button gotoRegisterPanel;
        // public Button registerButton;
        // public Button sendButton;
        // public Button leaveButton;

        public Transform content;

        public GameObject loginPanel;
        public GameObject loginFailedPanel;
        public GameObject registerPanel;
        public GameObject registerFailPanel;
        public GameObject roomlistBoard;
        public GameObject failedPanel;
        public GameObject chatBoard;

        #endregion

        [SerializeField] List<Text> chatCount = new List<Text>();

        string roomname;
        string username;
        string displayname;
        // string registerUsername;
        // string registerDisplayname;
        // string registerPassword;
        // string registerRePassword;
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

        void OnCreateRoom(WebsocketConnection.SocketEvent eventCheck)
        {
            displayname = WebsocketConnection.displayname;
            roomname = WebsocketConnection.roomname;
            if (eventCheck.status == "success")
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

        void OnJoinRoom(WebsocketConnection.SocketEvent eventCheck)
        {
            displayname = WebsocketConnection.displayname;
            roomname = WebsocketConnection.roomname;
            if (eventCheck.status == "success")
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

        void OnLeaveRoom(WebsocketConnection.SocketEvent eventCheck)
        {
            foreach (Text count in chatCount)
            {
                Destroy(count);
            }
            chatCount.Clear();
            roomlistBoard.SetActive(true);
            chatBoard.SetActive(false);
        }

        void OnLogin(WebsocketConnection.SocketEvent eventCheck)
        {
            displayname = WebsocketConnection.displayname;

            if (eventCheck.status == "success")
            {
                loginPanel.SetActive(false);
                roomlistBoard.SetActive(true);
                usernameInputField.text = string.Empty;
                passwordInputField.text = string.Empty;
                UsernameText.text = $"Connect as : [{eventCheck.data}]";
            }
            else
            {
                loginFailedPanel.SetActive(true);
            }
        }

        void OnRegister(WebsocketConnection.SocketEvent eventCheck)
        {
            if (eventCheck.status == "success")
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

        void OnChatUpdate(WebsocketConnection.SocketEvent eventCheck)
        {
            WebsocketConnection.MessageData recieveMessageData = JsonUtility.FromJson<WebsocketConnection.MessageData>(eventCheck.data);
            Text newTextbox = Instantiate(chatText, content) as Text;
            newTextbox.transform.SetParent(content.transform);
            newTextbox.text = string.Empty;
            if (recieveMessageData.userName == displayname)
            {
                // Text newTextbox = Instantiate(chatText, content) as Text;
                // newTextbox.transform.SetParent(content.transform);
                // newTextbox.text = string.Empty;
                newTextbox.alignment = TextAnchor.UpperRight;
                newTextbox.text += $"<color={recieveMessageData.color}>{recieveMessageData.userName}</color> : {recieveMessageData.message}";
                chatCount.Add(newTextbox);
            }
            else
            {
                // Text newTextbox = Instantiate(chatText, content) as Text;
                // newTextbox.transform.SetParent(content.transform);
                // newTextbox.text = string.Empty;
                newTextbox.alignment = TextAnchor.UpperLeft;
                newTextbox.text += $"<color={recieveMessageData.color}>{recieveMessageData.userName}</color> : {recieveMessageData.message}";
                chatCount.Add(newTextbox);
            }
        }
    }
}
