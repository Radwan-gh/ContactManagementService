function makePhone(length) {
  var result = "";
  var characters = "0123456789";
  var charactersLength = characters.length;
  for (var i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }
  return result;
}

function makeid(length) {
  var result = "";
  var characters =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  var charactersLength = characters.length;
  for (var i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }
  return result;
}

var generateRowsCount = 1000000;

for (let index = 1; index < generateRowsCount; index++) {
  var contactDoc = {
    _id: NumberInt(index),
    CustomAttributes: [
      {
        Name: "Name",
        Value: makeid(10) + " " + makeid(6),
        Type: NumberInt(0),
      },
      {
        Name: "Age",
        Value: "" + NumberInt(index),
        Type: NumberInt(1),
      },
      {
        Name: "Phone",
        Value: "++90" + makePhone(8),
        Type: NumberInt(1),
      },
    ],
    Companies: [
      {
        _id: NumberInt(1),
        CustomAttributes: [
          {
            Name: "Address",
            Value: "address cillum deserunt Except" + makeid(10),
            Type: NumberInt(0),
          },
          {
            Name: "Phone",
            Value: "++5" + makePhone(8),
            Type: NumberInt(0),
          },
          {
            Name: "Name",
            Value: makeid(7) + "" + makeid(9),
            Type: NumberInt(0),
          },
        ],
      },
      {
        _id: NumberInt(12),
        CustomAttributes: [
          {
            Name: "Phone",
            Value: "++5" + makePhone(8),
            Type: NumberInt(0),
          },
          {
            Name: "Address",
            Value: makeid(7) + "" + makeid(9),
            Type: NumberInt(0),
          },
          {
            Name: "Name",
            Value: makeid(7) + "" + makeid(9) + "" + makeid(4),
            Type: NumberInt(0),
          },
          {
            Name: "Employees Count",
            Value: makePhone(3),
            Type: NumberInt(1),
          },
        ],
      },
    ],
  };

  db.Contact.insertOne(contactDoc);
}




////   Company Document
companyDoc = {
  _id: 19,
  CustomAttributes: [
    {
      Name: "test",
      Value: "do Ut ",
      Type: 0,
    },
    {
      Name: "cillum deserunt Except",
      Value: "voluptate",
      Type: 0,
    },
    {
      Name: "Name",
      Value: "ea nostrud 88",
      Type: 0,
    },
  ],
};
