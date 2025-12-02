using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

public class Session : aModel, iDatabase
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

    public static JObject ToJSON(Session session)
    {
        JObject returnValue = new()
        {
            new JProperty("id", session.ID),
            new JProperty("parkingLotID", session.ParkingLotId),
            new JProperty("licensePlate", session.LicensePlate),
            new JProperty("vehicleID", session.VehicleID),
            new JProperty("userID", session.UserID),
            new JProperty("StartDateTime", session.StartDateTime),
            new JProperty("EndDateTime", session.EndDateTime),
            new JProperty("Duration", session.Duration),
            new JProperty("Amount", session.Amount),
            new JProperty("PaymentStatus", session.PaymentStatus),
            new JProperty("createDateTime", session.CreateDateTime),
            new JProperty("updateDateTime", session.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<Session> sessions)
    {
        JArray returnValue = new();

        foreach (Session session in sessions)
        {
            returnValue.Add(ToJSON(session));
        }

        return returnValue;
    }

    public static Session? GetByID(int id)
    {
        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByIDSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            Session returnValue = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ParkingLotId = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                LicensePlate = reader["LicensePlate"].ToString(),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Duration = Convert.ToInt32(reader["Duration"].ToString()),
                Amount = Convert.ToDouble(reader["Amount"].ToString()),
                PaymentStatus = reader["PaymentStatus"].ToString(),
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
            };

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<Session> GetAll()
    {
        List<Session> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllSQL();

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Session newSession = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ParkingLotId = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                LicensePlate = reader["LicensePlate"].ToString(),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Duration = Convert.ToInt32(reader["Duration"].ToString()),
                Amount = Convert.ToDouble(reader["Amount"].ToString()),
                PaymentStatus = reader["PaymentStatus"].ToString(),
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
            };

            returnValue.Add(newSession);
        }

        conn.Close();
        return returnValue;
    }

    public static List<Session> GetAllByUserID(int userID)
    {
        List<Session> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByUserIDSQL(userID);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Session newSession = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ParkingLotId = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                LicensePlate = reader["LicensePlate"].ToString(),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Duration = Convert.ToInt32(reader["Duration"].ToString()),
                Amount = Convert.ToDouble(reader["Amount"].ToString()),
                PaymentStatus = reader["PaymentStatus"].ToString(),
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
            };

            returnValue.Add(newSession);
        }

        conn.Close();
        return returnValue;
    }

    public static Session? GetByUserID(int userID)
    {
        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByUserIDSQL(userID);

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            Session returnValue = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ParkingLotId = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                LicensePlate = reader["LicensePlate"].ToString(),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Duration = Convert.ToInt32(reader["Duration"].ToString()),
                Amount = Convert.ToDouble(reader["Amount"].ToString()),
                PaymentStatus = reader["PaymentStatus"].ToString(),
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
            };

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<Session> GetAllByParkingLotID(int parkingLotID)
    {
        List<Session> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByParkingLotIDSQL(parkingLotID);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Session newSession = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                ParkingLotId = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                LicensePlate = reader["LicensePlate"].ToString(),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Duration = Convert.ToInt32(reader["Duration"].ToString()),
                Amount = Convert.ToDouble(reader["Amount"].ToString()),
                PaymentStatus = reader["PaymentStatus"].ToString(),
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
            };

            returnValue.Add(newSession);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO Sessions (ParkingLotID, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus) VALUES ({ParkingLotId}, \"{LicensePlate}\", {VehicleID}, {UserID}, {StartDateTime:YYYY-MM-DD hh:mm:ss}, {EndDateTime:YYYY-MM-DD hh:mm:ss}, {Duration}, {Amount}, \"{PaymentStatus}\")";

    public override string ToUpdateSQL() => $"UPDATE Sessions SET ParkingLotID = {ParkingLotId}, LicensePlate = \"{LicensePlate}\", VehicleID = {VehicleID}, UserID = {UserID}, StartDateTime = {StartDateTime:YYYY-MM-DD hh:mm:ss}, EndDateTime = {EndDateTime:YYYY-MM-DD hh:mm:ss}, Duration = {Duration}, Amount = {Amount}, PaymentStatus = \"{PaymentStatus}\" WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Sessions WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Sessions WHERE ID = {ID}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM Sessions WHERE ID = {id}";
    
    public static string ToSelectByParkingLotIDSQL(int id) => $"SELECT * FROM Sessions WHERE ParkingLotID = {id}";

    public static string ToSelectByLicensePlateSQL(int id) => $"SELECT * FROM Sessions WHERE LicensePlate = {id}";

    public static string ToSelectByVehicleIDSQL(int id) => $"SELECT * FROM Sessions WHERE VehicleID = {id}";

    public static string ToSelectByUserIDSQL(int id) => $"SELECT * FROM Sessions WHERE UserID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM Sessions";
}