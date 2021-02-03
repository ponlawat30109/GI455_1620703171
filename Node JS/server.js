var websocket = require("ws");

var callbackInitServer = ()=>{
    console.log("Fralyst Server is running.");
}

var wss = new websocket.Server({port:5500}, callbackInitServer)

var wsList = [];

wss.on("connection", (ws)=>{
    console.log("client connected.");
    wsList.push(ws);

    ws.on("message", (data)=>{
        console.log("send from client : "+ data);
        Broadcast(data);
    })

    ws.on("close", ()=>{
        wsList = ArrayRemove(wsList, ws);
        console.log("client disconnected.");
    })
})

function ArrayRemove(arr, value){
    return arr.filter((element)=>{
        return element != value;
    });
}

function Broadcast(data){
    for(var i = 0; i < wsList.length; i++){
        wsList[i].send(data);
    }
}