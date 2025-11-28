public class Payment: aDatabase, iDatabase
{
    public int ID { get; set; }
    public string TransactionID { get; set; }
    public int ParkingLotID { get; set; }
    public int SessionID { get; set; }
    public int UserID { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Method { get; set; }
    public string Issuer { get; set; }
    public string Bank { get; set; }
    public string Hash { get; set; }
    public DateTime CompletedDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }

    public override string ToInsertSQL() => $"INSERT INTO Transaction (TransactionID, ParkingLotID, SessionID, UserID, Amount, Date, Method, Issuer, Bank, Hash, CompletedDateTime) VALUES (\"{TransactionID}\", {ParkingLotID}, {SessionID}, {UserID}, {Amount}, {Date}, \"{Method}\", \"{Issuer}\", \"{Bank}\", \"{Hash}\", {CompletedDateTime})";

    public override string ToUpdateSQL() => $"UPDATE Transaction SET TransactionID = \"{TransactionID}\", ParkingLotID = {ParkingLotID}, SessionID = {SessionID}, UserID = {UserID}, Amount = {Amount}, Date = {Date:YYYY-MM-DD hh:mm:ss}, Method = \"{Method}\", Issuer = \"{Issuer}\", Bank = \"{Bank}\", Hash = \"{Hash}\", CompletedDateTime = {CompletedDateTime:YYYY-MM-DD hh:mm:ss} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Transaction WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Transaction WHERE ID = {ID}";
}