using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

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
    public DateTime UpdateDateTime { get; set; }

    public static JObject ToJSON(ParkingLot parkingLot)
    {
        JObject returnValue = new()
        {
            new JProperty("id", parkingLot.ID),
            new JProperty("name", parkingLot.Name),
            new JProperty("location", parkingLot.Location),
            new JProperty("address", parkingLot.Address),
            new JProperty("capacity", parkingLot.Capacity),
            new JProperty("reserved", parkingLot.Reserved),
            new JProperty("Tariff", parkingLot.Tariff),
            new JProperty("dayTariff", parkingLot.DayTariff),
            new JProperty("lat", parkingLot.Latitude),
            new JProperty("lon", parkingLot.Longitude),
            new JProperty("createDateTime", parkingLot.CreateDateTime),
            new JProperty("updateDateTime", parkingLot.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<ParkingLot> parkingLots)
    {
        JArray returnValue = new();

        foreach (ParkingLot parkingLot in parkingLots)
        {
            returnValue.Add(ToJSON(parkingLot));
        }

        return returnValue;
    }

    public static ParkingLot? GetByID(int id)
    {
        MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByIDSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            ParkingLot returnValue = new()
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
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
            };

            returnValue.Reserved = Reservation.GetAllByParkingLot(returnValue.ID).Count();

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<ParkingLot> GetAll()
    {
        List<ParkingLot> returnValue = [];

        MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllSQL();

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
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
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
            };

            newParkingLot.Reserved = Reservation.GetAllByParkingLot(newParkingLot.ID).Count();

            returnValue.Add(newParkingLot);
        }

        conn.Close();
        return returnValue;
    }

    //useful methods to check parking lots state
    public bool HasFreeSpots() => Reserved < Capacity;
    public int GetAvailableSpots() => Capacity - Reserved;
    public bool IsFull() => Reserved >= Capacity;

    public override string ToInsertSQL() => $"INSERT INTO ParkingLots (Name, Location, Address, Capacity, Tariff, DayTariff, Lat, Lon) VALUES (\"{Name}\", \"{Location}\", \"{Address}\", {Capacity}, {Tariff}, {DayTariff}, {Latitude}, {Longitude})";

    public override string ToUpdateSQL() => $"UPDATE ParkingLots SET Name = \"{Name}\", Location = \"{Location}\", Address = \"{Address}\", Capacity = {Capacity}, Tariff = {Tariff}, DayTariff = {DayTariff}, Lat = {Latitude}, Lon = {Longitude} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM ParkingLots WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM ParkingLots WHERE ID = {ID}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM ParkingLots WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM ParkingLots";
}