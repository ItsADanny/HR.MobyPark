using MySql.Data.MySqlClient;

public class Billing : aModel, iDatabase
{
    public int ID { get; set; }
    public int SessionID { get; set; }
    public int ParkingLotID { get; set; }
    public decimal Amount { get; set; }
    public string tHash { get; set; }
    public decimal Payed { get; set; }
    public decimal Balance { get; set; }
    public DateTime CompletedDateTime { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }

    public static List<Billing> GetAll(DBHelper dbHelper)
    {
        List<Billing> returnValue = [];

        MySqlConnection conn = new MySqlConnection(dbHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllSQL();

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            DateTime RCreateDateTime = DateTime.Now;
            DateTime? RUpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            Billing newBilling = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                tHash = reader["tHash"].ToString(),
                Payed = Convert.ToDecimal(reader["Payed"].ToString()),
                Balance = Convert.ToDecimal(reader["Balance"].ToString()),
                CompletedDateTime = Convert.ToDateTime(reader["CompletedDateTime"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newBilling);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO Billing (ID, SessionID, ParkingLotID, Amount, tHash, Payed, Balance, CompletedDateTime, CreateDateTime, UpdateDateTime) VALUES ({ID}, {SessionID}, {ParkingLotID}, {Amount}, \"{tHash}\", {Payed}, {Balance}, '{CompletedDateTime::YYYY-MM-DD hh:mm:ss}', '{CreateDateTime::YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime::YYYY-MM-DD hh:mm:ss}')";
    public static string ToSelectAllSQL() => "SELECT * FROM Billing";
    public static string ToDeleteAllSQL() => "DELETE FROM `Billing`";
}