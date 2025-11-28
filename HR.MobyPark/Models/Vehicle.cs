//Sample DATA from original JSON file
//  {
//      "id":"1",
//      "user_id":"1",
//      "license_plate":"76-KQQ-7",
//      "make":"Peugeot",
//      "model":"308",
//      "color":"Brown",
//      "year":2024,
//      "created_at":"2024-08-13"
//  }

public class Vehicle : aDatabase, iDatabase
{
    //Public properties
    public int ID { get; }
    public int UserID { get; }
    public string LicensePlate { get; }
    public string Make { get; }
    public string Model { get; }
    public string Color { get; }
    public int ModelYear { get; }
    public DateTime CreateDateTime { get; }
    public DateTime UpdateDateTime { get; }

    public override string ToInsertSQL() => $"INSERT INTO Vehicles (UserID, LicensePlate, Make, Model, Color, Year) VALUES (\"{UserID}\", \"{LicensePlate}\", \"{Make}\", \"{Model}\", \"{Color}\", \"{ModelYear}\")";

    public override string ToUpdateSQL() => $"UPDATE Vehicles SET UserID = \"{UserID}\", LicensePlate = \"{LicensePlate}\", Make = \"{Make}\", Model = \"{Model}\", Color = \"{Color}\", Year = \"{ModelYear}\" WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Vehicles WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Vehicles WHERE ID = {ID}";
}