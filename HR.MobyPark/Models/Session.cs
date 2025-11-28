public class Session : aDatabase, iDatabase
{
    public int ID { get; set; }
    public int ParkingLotId { get; set; }
    public string LicensePlate { get; set; }
    public int VehicleID { get; set; }
    public int UserID { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int Duration { get; set; }
    public double Amount { get; set; }
    public string PaymentStatus { get; set; } = "unpaid";
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }

    public override string ToInsertSQL() => $"INSERT INTO Sessions (ParkingLotID, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus) VALUES ({ParkingLotId}, \"{LicensePlate}\", {VehicleID}, {UserID}, {StartDateTime:YYYY-MM-DD hh:mm:ss}, {EndDateTime:YYYY-MM-DD hh:mm:ss}, {Duration}, {Amount}, \"{PaymentStatus}\")";

    public override string ToUpdateSQL() => $"UPDATE Sessions SET ParkingLotID = {ParkingLotId}, LicensePlate = \"{LicensePlate}\", VehicleID = {VehicleID}, UserID = {UserID}, StartDateTime = {StartDateTime:YYYY-MM-DD hh:mm:ss}, EndDateTime = {EndDateTime:YYYY-MM-DD hh:mm:ss}, Duration = {Duration}, Amount = {Amount}, PaymentStatus = \"{PaymentStatus}\" WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Sessions WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Sessions WHERE ID = {ID}";
}