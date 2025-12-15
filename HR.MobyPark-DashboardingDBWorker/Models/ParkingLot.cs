using MySql.Data.MySqlClient;

public class ParkingLot : aModel, iDatabase
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Address { get; set; }
    public int Capacity { get; set; }
    public int Reserved { get; set; }
    public decimal Tariff { get; set; }
    public decimal DayTariff { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }

    public static List<ParkingLot> GetAll(DBHelper dbHelper)
    {
        List<ParkingLot> returnValue = [];

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

            ParkingLot newParkingLot = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                Name = reader["ID"].ToString(),
                Location = reader["ID"].ToString(),
                Address = reader["ID"].ToString(),
                Capacity = Convert.ToInt32(reader["Capacity"].ToString()),
                Tariff = Convert.ToDecimal(reader["Tariff"].ToString()),
                DayTariff = Convert.ToDecimal(reader["DayTariff"].ToString()),
                Latitude = Convert.ToDouble(reader["Lat"].ToString()),
                Longitude = Convert.ToDouble(reader["Lon"].ToString()),
                CreateDateTime = RCreateDateTime,
                UpdateDateTime = RUpdateDateTime
            };

            newParkingLot.Reserved = Reservation.GetAllByParkingLot(dbHelper, newParkingLot.ID).Count();

            returnValue.Add(newParkingLot);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO ParkingLots (ID, Name, Location, Address, Capacity, Tariff, DayTariff, Lat, Lon, CreateDateTime, UpdateDateTime) VALUES ({ID}, \"{Name}\", \"{Location}\", \"{Address}\", {Capacity}, {Tariff}, {DayTariff}, {Latitude}, {Longitude}, \"{CreateDateTime::YYYY-MM-DD hh:mm:ss}\", \"{UpdateDateTime::YYYY-MM-DD hh:mm:ss}\")";
    public static string ToSelectAllSQL() => "SELECT * FROM ParkingLots";
    public static string ToDeleteAllSQL() => "DELETE FROM `ParkingLots`";
}