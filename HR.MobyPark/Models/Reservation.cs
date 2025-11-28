public class Reservation : aDatabase, iDatabase
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public int ParkingLotID { get; set; }
    public int VehicleID { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Status { get; set; } = "active";
    public decimal Amount { get; set; } //Decimal is beter dan double om afronden te voorkomen.
    // public DateTime CreatedAt { get; set; } = DateTime.Now;

    public override string ToInsertSQL() => $"INSERT INTO Reservations (UserID, ParkingLotID, VehicleID, StartDateTime, EndDateTime) VALUES ({UserID}, {ParkingLotID}, {VehicleID}, {StartDateTime:YYYY-MM-DD hh:mm:ss}, {EndDateTime:YYYY-MM-DD hh:mm:ss})";

    public override string ToUpdateSQL() => $"UPDATE Reservations SET UserID = {UserID}, ParkingLotID = {ParkingLotID}, VehicleID = {VehicleID}, StartDateTime = {StartDateTime:YYYY-MM-DD hh:mm:ss}, EndDateTime = {EndDateTime:YYYY-MM-DD hh:mm:ss} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Reservations WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Reservations WHERE ID = {ID}";
}