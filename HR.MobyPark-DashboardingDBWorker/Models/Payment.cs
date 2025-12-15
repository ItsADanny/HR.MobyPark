using MySql.Data.MySqlClient;

public class Payment: aModel, iDatabase
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
    public DateTime CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }

    public static List<Payment> GetAll(DBHelper dbHelper)
    {
        List<Payment> returnValue = [];

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

            Payment newPayment = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                TransactionID = reader["TransactionID"].ToString(),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                Date = Convert.ToDateTime(reader["Date"].ToString()),
                Method = reader["Method"].ToString(),
                Issuer = reader["Issuer"].ToString(),
                Bank = reader["Bank"].ToString(),
                Hash = reader["Hash"].ToString(),
                CompletedDateTime = Convert.ToDateTime(reader["CompletedDateTime"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newPayment);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO `Transaction` (ID, TransactionID, ParkingLotID, SessionID, UserID, Amount, Date, Method, Issuer, Bank, Hash, CompletedDateTime, CreateDateTime, UpdateDateTime) VALUES ({ID}, \"{TransactionID}\", {ParkingLotID}, {SessionID}, {UserID}, {Amount}, {Date}, \"{Method}\", \"{Issuer}\", \"{Bank}\", \"{Hash}\", '{CompletedDateTime::YYYY-MM-DD hh:mm:ss}', '{CreateDateTime::YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime::YYYY-MM-DD hh:mm:ss}')";
    public static string ToSelectAllSQL() => "SELECT * FROM `Transaction`";

    public static string ToDeleteAllSQL() => "DELETE FROM `Transaction`";
}