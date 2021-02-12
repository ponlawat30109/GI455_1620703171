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

        [SerializeField]
        public class MessageData
        {
            [SerializeField] public string userName;
            [SerializeField] public string message;
        }

        string url = EnterData.url;
        string port = EnterData.port;
        string username = EnterData.username;
        string textMessage = "";
        string tempMessageString;

        public InputField inputField;
        public Text chatText, headerText;
        public Button sendButton, leaveButton;

        public Transform content;

        [SerializeField] List<MessageData> messageList = new List<MessageData>();
        //[SerializeField] Dictionary<string, string> message = new Dictionary<string, string>();

        void Start()
        {
            headerText.text = $"Room ws://{url}:{port}/";

            websocket = new WebSocket($"ws://{url}:{port}/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            chatText.text = $"\n<b>Welcome {username} to GI455 Chat</b>\n";
            chatText.alignment = TextAnchor.UpperCenter;

            sendButton.onClick.AddListener(GetText);
            leaveButton.onClick.AddListener(LeaveChat);
        }

        private void Update()
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
                    newTextbox.text += recieveMessageData.userName + ": " + recieveMessageData.message;
                }
                else
                {
                    Text newTextbox = Instantiate(chatText, content) as Text;
                    newTextbox.transform.SetParent(content.transform);
                    newTextbox.text = string.Empty;
                    newTextbox.alignment = TextAnchor.UpperLeft;
                    newTextbox.text += recieveMessageData.userName + ": " + recieveMessageData.message;
                }

                messageList.Add(recieveMessageData);
                //Debug.Log($"{messageList[messageList.Count -1].userName}: {messageList[messageList.Count - 1].message}");

                tempMessageString = string.Empty;
            }
        }

        //public void CreateNewChat()
        //{
        //    Text newTextbox = Instantiate(chatText, content) as Text;
        //    newTextbox.transform.SetParent(content.transform);
        //    newTextbox.text += message[message.Count - 1];
        //}

        //public void ChatUpdate()
        //{
        //chatText.text += message[message.Count - 1] + "\n";

        //Text newTextbox = Instantiate(chatText, content) as Text;
        //newTextbox.transform.SetParent(content.transform);
        //newTextbox.text = string.Empty;

        //string[] temp = message[message.Count - 1].Split(':');

        //newTextbox.alignment = TextAnchor.UpperLeft;
        //if (temp[0] == username)
        //{
        //    newTextbox.alignment = TextAnchor.UpperRight;
        //    int usernameStrLenght = temp[0].Length;
        //    //string newTemp = message[message.Count - 1].Substring(usernameStrLenght);
        //    newTextbox.text += $"{message[message.Count - 1].Substring(usernameStrLenght + 2)}\n";
        //    //$"{message[message.Count - 1].Substring(usernameStrLenght + 2)} <size=24>({System.DateTime.Now.ToString("hh:mm")})</size>\n";
        //}
        //else
        //{
        //    newTextbox.text += $"{message[message.Count - 1]}\n";
        //}
        //}

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

            MessageData newMessageData = new MessageData();
            newMessageData.userName = username;
            newMessageData.message = textMessage;

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
            //Debug.Log("Recieve msg from " + messageEventArgs.Data);

            tempMessageString = messageEventArgs.Data;
        }

        public void LeaveChat()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}