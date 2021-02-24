using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class TestConnect : MonoBehaviour
{
    [System.Serializable]
    struct StudentData
    {
        public string eventName;
        public string studentID;

        public StudentData(string eventName, string data)
        {
            this.eventName = eventName;
            this.studentID = data;
        }
    }

    [System.Serializable]
    struct GetStudentData
    {
        public string eventName;
        public string status;
        public string message;
        public string studentName;
        public string studentEmail;

        public GetStudentData(string eventName, string status, string data, string studentName, string studentEmail)
        {
            this.eventName = eventName;
            this.status = status;
            this.message = data;
            this.studentName = studentName;
            this.studentEmail = studentEmail;
        }
    }

    [SerializeField] List<GetStudentData> storeData = new List<GetStudentData>();
    string tempMessageString;

    private WebSocket websocket;

    void Start()
    {
        websocket = new WebSocket($"ws://gi455-305013.an.r.appspot.com");
        websocket.OnMessage += OnMessage;
        websocket.Connect();
    }

    public void CheckID()
    {
        if (websocket.ReadyState == WebSocketState.Open)
        {
            StudentData newSocketEvent = new StudentData("GetStudentData", "1620703171");
            string jsonStr = JsonUtility.ToJson(newSocketEvent);
            websocket.Send(jsonStr);
        }
    }

    public void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {
        tempMessageString = messageEventArgs.Data;
        Debug.Log(tempMessageString);
        GetStudentData recieveMsgEvent = JsonUtility.FromJson<GetStudentData>(tempMessageString);
        storeData.Add(recieveMsgEvent);
        Debug.Log(recieveMsgEvent.eventName);
        Debug.Log(recieveMsgEvent.status);
        Debug.Log(recieveMsgEvent.message);
        Debug.Log(recieveMsgEvent.studentName);
        Debug.Log(recieveMsgEvent.studentEmail);
    }
}
