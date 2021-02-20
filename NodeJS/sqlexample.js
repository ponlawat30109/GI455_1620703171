const sqlite = require("sqlite3").verbose();

let db = new sqlite.Database(
  "./DB/chatDB.db",
  sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE,
  (err) => {
    if (err) throw err;
    // if (err) {
    //     console.log(err);
    // }

    console.log("Connected to database");

    var id = "test007";
    var password = "123456";
    var name = "test7";

    // var sqlSelect = "SELECT * FROM UserData WHERE UserID = '" + id + "' AND Password = '" + password + "' ";
    // var sqlInsert = "INSERT INTO UserData (UserID, Password, Name, Money) VALUES ('" + id + "', '" + password + "', '" + name + "', '100')";
    // var sqlUpdate = "UPDATE UserData SET Money = '500' WHERE UserID = '" + id + "'";
    var sqlSelect = `SELECT * FROM UserData WHERE UserID = '${id}' AND Password = '${password}' `;
    var sqlInsert = `INSERT INTO UserData (UserID, Password, Name, Money) VALUES ('${id}', '${password}', '${name}', '100')`;
    var sqlUpdate = `UPDATE UserData SET Money = '500' WHERE UserID = '${id}'`;

    db.all(
      // `SELECT Money FROM UserData WHERE UserID = '${id}'`,
      "SELECT * FROM UserData WHERE Money != 0",
      // sqlSelect,
      (err, rows) => {
        if (err) {
          console.log(err);
        } else {
          if (rows.length > 0) {
            var currentMoney = rows[0].Money;
            currentMoney += 100;

            db.all(
              `UPDATE UserData SET Money = '${currentMoney}' WHERE UserID = '${id}'`,
              (err, rows) => {
                if (err) {
                  console.log(err);
                } else {
                  var result = {
                    status: true,
                    money: currentMoney,
                  };

                  // console.log(JSON.stringify(result));
                }
              }
            );
          } else {
            console.log("UserID not found");
          }
        }

        // console.log(rows);
        console.table(rows);
        // console.log(`${id} ${password}`);
      }
    );
  }
);
