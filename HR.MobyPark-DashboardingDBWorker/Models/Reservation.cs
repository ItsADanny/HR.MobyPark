using MySql.Data.MySqlClient;

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

    public static List<Reservation> GetAll(DBHelper dBHelper)
    {
        List<Reservation> returnValue = [];

        MySqlConnection conn = new MySqlConnection(dBHelper.GetConnectionSTR());
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

    public static List<Reservation> GetAllByParkingLot(DBHelper dBHelper, int id)
    {
        List<Reservation> returnValue = [];

        MySqlConnection conn = new MySqlConnection(dBHelper.GetConnectionSTR());
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

    public static string ToSelectByParkingLotSQL(int id) => $"SELECT * FROM Reservations WHERE ParkingLotID = {id}";
    public override string ToInsertSQL() => $"INSERT INTO Reservations (ID, UserID, ParkingLotID, VehicleID, StartDateTime, EndDateTime, CreateDateTime, UpdateDateTime) VALUES ({ID}, {UserID}, {ParkingLotID}, {VehicleID}, '{StartDateTime:YYYY-MM-DD hh:mm:ss}', '{EndDateTime:YYYY-MM-DD hh:mm:ss}', '{CreateDateTime::YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime::YYYY-MM-DD hh:mm:ss}')";
    public static string ToSelectAllSQL() => "SELECT * FROM Reservations";
    public static string ToDeleteAllSQL() => "DELETE FROM `Reservations`";
}