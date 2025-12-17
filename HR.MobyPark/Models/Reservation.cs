using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

public class Reservation : aModel, iDatabase
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
    public DateTime CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }

    public static JObject ToJSON(Reservation reservation)
    {
        JObject returnValue = new()
        {
            new JProperty("id", reservation.ID),
            new JProperty("userID", reservation.UserID),
            new JProperty("ParkingLotID", reservation.ParkingLotID),
            new JProperty("vehicleID", reservation.VehicleID),
            new JProperty("StartDateTime", reservation.StartDateTime),
            new JProperty("EndDateTime", reservation.EndDateTime),
            new JProperty("Status", reservation.Status),
            new JProperty("Amount", reservation.Amount),
            new JProperty("createDateTime", reservation.CreateDateTime),
            new JProperty("updateDateTime", reservation.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<Reservation> reservations)
    {
        JArray returnValue = new();

        foreach (Reservation reservation in reservations)
        {
            returnValue.Add(ToJSON(reservation));
        }

        return returnValue;
    }

    public static Reservation? GetByID(int id)
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

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            Reservation returnValue = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Status = reader["Status"].ToString(),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<Reservation> GetAll()
    {
        List<Reservation> returnValue = [];

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

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            Reservation newReservation = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Status = reader["Status"].ToString(),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newReservation);
        }

        conn.Close();
        return returnValue;
    }

    public static List<Reservation> GetAllByParkingLot(int id)
    {
        List<Reservation> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByParkingLotSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            DateTime RCreateDateTime = DateTime.Now;
            DateTime? RUpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            Reservation newReservation = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Status = reader["Status"].ToString(),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newReservation);
        }

        conn.Close();
        return returnValue;
    }

    public static List<Reservation> GetAllByVehicle(int id)
    {
        List<Reservation> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByVehicleSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            DateTime RCreateDateTime = DateTime.Now;
            DateTime? RUpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            Reservation newReservation = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                ParkingLotID = Convert.ToInt32(reader["ParkingLotID"].ToString()),
                VehicleID = Convert.ToInt32(reader["VehicleID"].ToString()),
                StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                Status = reader["Status"].ToString(),
                Amount = Convert.ToDecimal(reader["Amount"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newReservation);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO Reservations (UserID, ParkingLotID, VehicleID, StartDateTime, EndDateTime) VALUES ({UserID}, {ParkingLotID}, {VehicleID}, {StartDateTime:YYYY-MM-DD hh:mm:ss}, {EndDateTime:YYYY-MM-DD hh:mm:ss})";

    public override string ToUpdateSQL() => $"UPDATE Reservations SET UserID = {UserID}, ParkingLotID = {ParkingLotID}, VehicleID = {VehicleID}, StartDateTime = {StartDateTime:YYYY-MM-DD hh:mm:ss}, EndDateTime = {EndDateTime:YYYY-MM-DD hh:mm:ss} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Reservations WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Reservations WHERE ID = {ID}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM Reservations WHERE ID = {id}";

    public static string ToSelectByParkingLotSQL(int id) => $"SELECT * FROM Reservations WHERE ParkingLotID = {id}";

    public static string ToSelectByUserSQL(int id) => $"SELECT * FROM Reservations WHERE UserID = {id}";

    public static string ToSelectByVehicleSQL(int id) => $"SELECT * FROM Reservations WHERE VehicleID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM Reservations";
}