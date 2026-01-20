using MySql.Data.MySqlClient;

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
    public DateTime? UpdateDateTime { get; set; }

    public static List<Session> GetAll(DBHelper dBHelper)
    {
        List<Session> returnValue = [];

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
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            returnValue.Add(newSession);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO Sessions (ID, ParkingLotID, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus, CreateDateTime, UpdateDateTime) VALUES ({ID}, {ParkingLotId}, \"{LicensePlate}\", {VehicleID}, {UserID}, '{StartDateTime:YYYY-MM-DD hh:mm:ss}', '{EndDateTime:YYYY-MM-DD hh:mm:ss}', {Duration}, {Amount}, \"{PaymentStatus}\", '{CreateDateTime::YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime::YYYY-MM-DD hh:mm:ss}')";
    public static string ToSelectAllSQL() => "SELECT * FROM Sessions";
    public static string ToDeleteAllSQL() => "DELETE FROM `Sessions`";
}