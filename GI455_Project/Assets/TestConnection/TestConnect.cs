using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class TestConnect : MonoBehaviour
{
    struct StudentEventData
    {
        public string eventName;
        public string studentID;

        public StudentEventData(string _eventName, string _studentID)
        {
            eventName = _eventName;
            studentID = _studentID;
        }
    }

    struct RequestExam
    {
        public string eventName;
        public string token;
        public RequestExam(string _eventName, string _token)
        {
            eventName = _eventName;
            token = _token;
        }
    }

    struct ExamData
    {
        public string eventName;
        public bool status;
        public string message;

        public ExamData(string _eventName, bool _status, string _message)
        {
            eventName = _eventName;
            status = _status;
            message = _message;
        }
    }

    struct ResultData
    {
        public string eventName;
        public string token;
        public string answer;
        public ResultData(string _eventName, string _token, string _answer)
        {
            eventName = _eventName;
            token = _token;
            answer = _answer;
        }
    }

    struct TempStruct
    {
        public string eventName;
        public TempStruct(string _eventName)
        {
            eventName = _eventName;
        }
    }

    private string tempMessageString;

    [SerializeField] Button checkIDButton;
    [SerializeField] Button startExamButton;
    [SerializeField] Button requestExamButton;
    [SerializeField] Button sendAnswerButton;

    [SerializeField] InputField IDInput;
    [SerializeField] InputField AnsInput;


    private WebSocket websocket;
    private bool IsWebsocketConnect() => (websocket != null && websocket.ReadyState == WebSocketState.Open) ? true : false;

    void Awake()
    {
        checkIDButton.onClick.AddListener(CheckID);
        startExamButton.onClick.AddListener(StartExam);
        requestExamButton.onClick.AddListener(RequestExamInfo);
        sendAnswerButton.onClick.AddListener(SendAnswer);
    }

    void Start()
    {
        websocket = new WebSocket($"ws://gi455-305013.an.r.appspot.com");
        websocket.OnMessage += OnMessage;
        websocket.Connect();
        Debug.Log((IsWebsocketConnect()) ? "Connect to server" : "Failed to connect server");
    }

    void Update()
    {
        ExamInfo();
    }

    void CheckID()
    {
        if (IsWebsocketConnect())
        {
            StudentEventData studentData = new StudentEventData("GetStudentData", IDInput.text);
            websocket.Send(JsonUtility.ToJson(studentData));
            // Debug.Log("test GetStudentData");
        }
    }

    void StartExam()
    {
        if (IsWebsocketConnect())
        {
            StudentEventData examData = new StudentEventData("StartExam", IDInput.text);
            websocket.Send(JsonUtility.ToJson(examData));
            // Debug.Log("test StartExam");
        }
    }

    void RequestExamInfo()
    {
        if (IsWebsocketConnect())
        {
            RequestExam requestExam = new RequestExam("RequestExamInfo", IDInput.text);
            websocket.Send(JsonUtility.ToJson(requestExam));
            // Debug.Log("test RequestExamInfo");
        }
    }

    void SendAnswer()
    {
        if (IsWebsocketConnect())
        {
            ResultData resultData = new ResultData("SendAnswer", IDInput.text, AnsInput.text);
            websocket.Send(JsonUtility.ToJson(resultData));
            // Debug.Log("test SendAnswer");
        }
    }

    void ExamInfo()
    {
        if (!string.IsNullOrEmpty(tempMessageString))
        {
            TempStruct tempStruct = JsonUtility.FromJson<TempStruct>(tempMessageString);
            if (tempStruct.eventName == "RequestExamInfo")
            {
                ExamData examInfo = JsonUtility.FromJson<ExamData>(tempMessageString);
                if (examInfo.status == true)
                {
                    // var data = examInfo.message.Split(',');
                    List<string> data = new List<string>(examInfo.message.Split(','));
                }
                Debug.Log("success");
            }
            else{
                Debug.Log("err");
            }
        }
        tempMessageString = string.Empty;
    }

    public void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {
        tempMessageString = messageEventArgs.Data;
    }
}
