using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

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

    public static JObject ToJSON(Payment payment)
    {
        JObject returnValue = new()
        {
            new JProperty("id", payment.ID),
            new JProperty("transactionID", payment.TransactionID),
            new JProperty("parkingLotID", payment.ParkingLotID),
            new JProperty("sessionID", payment.SessionID),
            new JProperty("userID", payment.UserID),
            new JProperty("amount", payment.Amount),
            new JProperty("date", payment.Date),
            new JProperty("method", payment.Method),
            new JProperty("issuer", payment.Issuer),
            new JProperty("bank", payment.Bank),
            new JProperty("hash", payment.Hash),
            new JProperty("completedDateTime", payment.CompletedDateTime),
            new JProperty("createDateTime", payment.CreateDateTime),
            new JProperty("updateDateTime", payment.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<Payment> payments)
    {
        JArray returnValue = new();

        foreach (Payment payment in payments)
        {
            returnValue.Add(ToJSON(payment));
        }

        return returnValue;
    }

    public static Payment? GetByID(int id)
    {
        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByIDSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            DateTime RCreateDateTime = DateTime.Now;
            DateTime? RUpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            // if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            // if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = reader.GetDateTime("CreateDateTime");
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = reader.GetDateTime("UpdateDateTime");

            Payment returnValue = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                TransactionID = reader["TransactionID"].ToString(),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                Date = reader.GetDateTime("Date"),
                Method = reader["Method"].ToString(),
                Issuer = reader["Issuer"].ToString(),
                Bank = reader["Bank"].ToString(),
                Hash = reader["Hash"].ToString(),
                CompletedDateTime = reader.GetDateTime("CompletedDateTime"),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<Payment> GetAll()
    {
        List<Payment> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
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

            // if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            // if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = reader.GetDateTime("CreateDateTime");
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = reader.GetDateTime("UpdateDateTime");

            Payment newPayment = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                TransactionID = reader["TransactionID"].ToString(),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                Date = reader.GetDateTime("Date"),
                Method = reader["Method"].ToString(),
                Issuer = reader["Issuer"].ToString(),
                Bank = reader["Bank"].ToString(),
                Hash = reader["Hash"].ToString(),
                CompletedDateTime = reader.GetDateTime("CompletedDateTime"),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newPayment);
        }

        conn.Close();
        return returnValue;
    }

    public static List<Payment> GetAllByUserID(int userID)
    {
        List<Payment> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByUserIDSQL(userID);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            DateTime RCreateDateTime = DateTime.Now;
            DateTime? RUpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = reader.GetDateTime("CreateDateTime");
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = reader.GetDateTime("UpdateDateTime");

            Payment newPayment = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                TransactionID = reader["TransactionID"].ToString(),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                SessionID = Convert.ToInt32(reader["SessionID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                Date = reader.GetDateTime("Date"),
                Method = reader["Method"].ToString(),
                Issuer = reader["Issuer"].ToString(),
                Bank = reader["Bank"].ToString(),
                Hash = reader["Hash"].ToString(),
                CompletedDateTime = reader.GetDateTime("CompletedDateTime"),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newPayment);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO Transaction (TransactionID, ParkingLotID, SessionID, UserID, Amount, Date, Method, Issuer, Bank, Hash, CompletedDateTime, CreateDateTime) VALUES (\"{TransactionID}\", {ParkingLotID}, {SessionID}, {UserID}, {Amount}, '{Date:yyyy-MM-dd HH:mm:ss}', \"{Method}\", \"{Issuer}\", \"{Bank}\", \"{Hash}\", '{CompletedDateTime:yyyy-MM-dd HH:mm:ss}', '{CreateDateTime:yyyy-MM-dd HH:mm:ss}')";

    public override string ToUpdateSQL() => $"UPDATE Transaction SET TransactionID = \"{TransactionID}\", ParkingLotID = {ParkingLotID}, SessionID = {SessionID}, UserID = {UserID}, Amount = {Amount}, Date = '{Date:yyyy-MM-dd HH:mm:ss}', Method = \"{Method}\", Issuer = \"{Issuer}\", Bank = \"{Bank}\", Hash = \"{Hash}\", CompletedDateTime = '{CompletedDateTime:yyyy-MM-dd HH:mm:ss}', UpdateDateTime = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Transaction WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Transaction WHERE ID = {ID}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM Transaction WHERE ID = {id}";

    public static string ToSelectByUserIDSQL(int id) => $"SELECT * FROM Transaction WHERE UserID = {id}";

    public static string ToSelectByTransactionIDSQL(int id) => $"SELECT * FROM Transaction WHERE TransactionID = {id}";

    public static string ToSelectByParkingLotIDSQL(int id) => $"SELECT * FROM Transaction WHERE ParkingLotID = {id}";

    public static string ToSelectBySessionIDSQL(int id) => $"SELECT * FROM Transaction WHERE SessionID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM Transaction";
}