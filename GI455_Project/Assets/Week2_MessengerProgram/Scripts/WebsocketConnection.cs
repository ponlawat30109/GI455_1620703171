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

        string url = EnterData.url;
        string port = EnterData.port;
        string username = EnterData.username;
        string textMessage = "";

        public InputField inputField;
        public Text chatText, headerText;
        public Button sendButton, leaveButton;

        public Transform content;

        [SerializeField] List<string> message = new List<string>();

        void Start()
        {
            headerText.text = $"Room ws://{url}:{port}/";

            websocket = new WebSocket($"ws://{url}:{port}/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            chatText.text = $"\n<b>Welcome {username} to GI455 Chat</b>\n";
            chatText.alignment = TextAnchor.UpperCenter;

            //Text newTextbox = Instantiate(chatText, content) as Text;
            //newTextbox.transform.SetParent(content.transform);
            //newTextbox.text = string.Empty;

            sendButton.onClick.AddListener(GetText);
            leaveButton.onClick.AddListener(LeaveChat);
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Return))
        //    {
        //        GetText();
        //    }
        //}

        //public void CreateNewChat()
        //{
        //    Text newTextbox = Instantiate(chatText, content) as Text;
        //    newTextbox.transform.SetParent(content.transform);
        //    newTextbox.text += message[message.Count - 1];
        //}

        public void ChatUpdate()
        {
            //chatText.text += message[message.Count - 1] + "\n";

            Text newTextbox = Instantiate(chatText, content) as Text;
            newTextbox.transform.SetParent(content.transform);
            newTextbox.text = string.Empty;

            string[] temp = message[message.Count - 1].Split(':');

            newTextbox.alignment = TextAnchor.UpperLeft;
            if (temp[0] == username)
            {
                newTextbox.alignment = TextAnchor.UpperRight;
                int usernameStrLenght = temp[0].Length;
                //string newTemp = message[message.Count - 1].Substring(usernameStrLenght);
                newTextbox.text += $"{message[message.Count - 1].Substring(usernameStrLenght + 2)}\n";  
                //$"{message[message.Count - 1].Substring(usernameStrLenght + 2)} <size=24>({System.DateTime.Now.ToString("hh:mm")})</size>\n";
            }
            else
            {
                newTextbox.text += $"{message[message.Count - 1]}\n";
            }
        }

        private void OnDestroy()
        {
            if (websocket != null)
            {
                websocket.Close();
            }
        }

        public void GetText()
        {
            textMessage = inputField.text;
            inputField.text = string.Empty;

            if (textMessage != "")
            {
                if (websocket.ReadyState == WebSocketState.Open)
                {
                    string currentMessage = string.Concat($"{username}: {textMessage}");
                    websocket.Send($"{currentMessage}");
                }
            }
            //ChatUpdate();
            //CreateNewChat();
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log("Recieve msg from " + messageEventArgs.Data);

            message.Add(messageEventArgs.Data);

            ChatUpdate();
        }

        public void LeaveChat()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}