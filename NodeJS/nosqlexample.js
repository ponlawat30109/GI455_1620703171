var mongo = require("mongodb").MongoClient;
var url = "mongodb://localhost:27017/";

mongo.connect(url, { useUnifiedTopology: true }, (err, result) => {
  if (err) throw err;
  // console.log("Connect to Database");
  var selectDB = result.db("GI455");

  Register(selectDB, "1", "1", "test");
});

var Register = (db, _playerID, _password, _playerName) => {
  var newData = {
    playerID: _playerID,
    password: _password,
    playerName: _playerName,
    money: 0,
  };

  var playerData = "playerData";

  var query = {
    playerID: _playerID,
    playerName: _playerName,
  };
  db.collection(playerData)
    .find(query)
    .toArray((err, result) => {
      if (err) {
        console.log(`Register Err : ${err}`);
      } else {
        if (result.lenght == 0) {
          db.collection(playerData).insertOne(newData, (err, result) => {
            if (err) {
              console.log(err);
            } else {
              if (result.result.ok == 1) {
                console.log("Register success");
              } else {
                console.log("Register fail");
              }
            }
          });
        } else {
          console.log("playerID is exist");
        }
      }
    });
};
var AddMoney = (db, _playerID) => {
  var addMoney = 500;
  var collectionName = "playerData";

  var query = {
    playerID: _playerID,
  };

  var updateData = { $set: { money: addMoney } };

  db.collection(collectionName)
    .findOne(query)
    .toArray((err, result) => {
      if (err) {
        console.log(err);
      } else {
        if (result.lenght == 0) {
          console.log("Can't find playerID : " + _playerID);
        } else {
          var updateMoney = result[0].money + addMoney;
          var updateData = { $set: { money: addMoney } };
        }
      }
    });

  db.collection(collectionName).updateOne(query, updateData, (err, result) => {
    if (err) throw err;
    else {
      if (result.result.nModified == 1) {
        console.log("AddMoney success");
      } else {
        console.log("AddMoney fail");
      }
    }
  });
};

//   db.collection(playerData).insertOne(newData, (err, result) => {
//     if (err) {
//       console.log(err);
//     } else {
//       console.log(result.result.ok);
//     }
//   });
// };

// var CreateCollection = (db) => {
//   db.CreateCollection("playerData", (err, res) => {
//     if (err) throw err;
//   });
//   db.CreateCollection("inventory", (err, res) => {
//     if (err) throw err;
//   });
//   db.CreateCollection("shopData", (err, res) => {
//     if (err) throw err;
//   });
// };
