var websocket = require("ws");

var callbackInitServer = () => {
  console.log("Fralyst Server is running.");
};

var wss = new websocket.Server({ port: 5500 }, callbackInitServer);

var wsList = [];
var roomList = [];

wss.on("connection", (ws) => {
  {
    ws.on("message", (data) => {
      // console.log(data);

      var toJSON = JSON.parse(data);

      // console.log(toJSON["eventName"]);
      // console.log(toJSON.eventName);

      if (toJSON.eventName == "CreateRoom") {
        console.log("Client request CreateRoom [" + toJSON.roomName + "]");
        var isFoundRoom = false;
        for (var i = 0; i < roomList.length; i++) {
          if (roomList[i].roomName == toJSON.roomName) {
            isFoundRoom = true;
            break;
          }
        }

        if (isFoundRoom) {
          //callback to client roomname is exist
          console.log("Room is exist");
          console.log("Create Room failed");
          var resultData = {
            eventName: toJSON.eventName,
            roomName: toJSON.roomName,
            status: "fail",
          };

          var jsonToStr = JSON.stringify(resultData);
          ws.send(jsonToStr);
        } else {
          //create room
          var newRoom = {
            roomName: toJSON.roomName,
            wsList: [],
          };

          newRoom.wsList.push(ws);

          roomList.push(newRoom);

          var resultData = {
            eventName: toJSON.eventName,
            roomName: toJSON.roomName,
            status: "success",
          };

          var jsonToStr = JSON.stringify(resultData);

          console.log("Room is not exist");
          console.log("Create Room : " + newRoom.roomName);

          ws.send(jsonToStr);

          for (var i = 0; i < roomList.length; i++) {
            console.log(
              "ws in " +
                roomList[i].roomName +
                " is " +
                roomList[i].wsList.length
            );
          }
        }
      } else if (toJSON.eventName == "JoinRoom") {
        console.log("Client request to Join Room [" + toJSON.roomName + "]");
        var isRoomExist = false;
        var roomindex;
        for (var i = 0; i < roomList.length; i++) {
          if (roomList[i].roomName == toJSON.roomName) {
            isRoomExist = true;
            roomindex = i;
            break;
          }
        }

        if (isRoomExist) {
          console.log("Join Room [" + toJSON.roomName + "] success");
          roomList[roomindex].wsList.push(ws);

          var resultData = {
            eventName: toJSON.eventName,
            roomName: toJSON.roomName,
            status: "success",
          };

          var jsonToStr = JSON.stringify(resultData);
          ws.send(jsonToStr);
        } else {
          console.log("Join Room [" + toJSON.roomName + "] failed");
          var resultData = {
            eventName: toJSON.eventName,
            roomName: toJSON.roomName,
            status: "fail",
          };

          var jsonToStr = JSON.stringify(resultData);
          ws.send(jsonToStr);
        }
        for (var i = 0; i < roomList.length; i++) {
          console.log(
            "ws in " + roomList[i].roomName + " is " + roomList[i].wsList.length
          );
        }
      } else if (toJSON.eventName == "LeaveRoom") {
        console.log("Client request to Leave Room [" + toJSON.roomName + "]");
        var isLeave = false;
        for (var i = 0; i < roomList.length; i++) {
          for (var j = 0; j < roomList[i].wsList.length; j++) {
            if (ws == roomList[i].wsList[j]) {
              roomList[i].wsList.splice(j, 1);
              if (roomList[i].wsList.length <= 0) {
                roomList.splice(i, 1);
              }
              isLeave = true;
              break;
            }
          }
        }
        var resultData = {
          eventName: toJSON.eventName,
          roomName: toJSON.roomName,
          status: "success",
        };
        var jsonToStr = JSON.stringify(resultData);
        ws.send(jsonToStr);
        if (isLeave) {
          console.log("Leave room [ success ]");
        } else {
          console.log("Leave room [ failed ]");
        }
        for (var i = 0; i < roomList.length; i++) {
          console.log(
            "ws in " + roomList[i].roomName + " is " + roomList[i].wsList.length
          );
        }
      }
    });
  }

  console.log("client connected.");
  wsList.push(ws);

  ws.on("close", () => {
    console.log("client disconnected.");
    for (var i = 0; i < roomList.length; i++) {
      for (var j = 0; j < roomList[i].wsList.length; j++) {
        if (ws == roomList[i].wsList[j]) {
          roomList[i].wsList.splice(j, 1);
          if (roomList[i].wsList.length <= 0) {
            roomList.splice(i, 1);
          }
          isLeave = true;
          break;
        }
      }
    }

    for (var i = 0; i < roomList.length; i++) {
      console.log(
        "ws in " + roomList[i].roomName + " is " + roomList[i].wsList.length
      );
    }
  });
});

function ArrayRemove(arr, value) {
  return arr.filter((element) => {
    return element != value;
  });
}

function Broadcast(data) {
  for (var i = 0; i < wsList.length; i++) {
    wsList[i].send(data);
  }
}
