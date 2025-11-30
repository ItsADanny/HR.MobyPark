//Sample DATA from original JSON file
//  {
//      "id":"1",
//      "user_id":"1",
//      "license_plate":"76-KQQ-7",
//      "make":"Peugeot",
//      "model":"308",
//      "color":"Brown",
//      "year":2024,
//      "created_at":"2024-08-13"
//  }

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

public class Vehicle : aModel, iDatabase
{
    //Public properties
    public int ID { get; private set; }
    public int UserID { get; private set; }
    public string LicensePlate { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public string Color { get; private set; }
    public int ModelYear { get; private set; }
    public DateTime CreateDateTime { get; private set; }
    public DateTime UpdateDateTime { get; private set; }

    private Vehicle(int id, int userid, string licensePlate, string make, string model, string color, int modelYear, DateTime createDateTime, DateTime updateDateTime)
    {
        ID = id;
        UserID = userid;
        LicensePlate = licensePlate;
        Make = make;
        Model = model;
        Color = color;
        ModelYear = modelYear;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static JObject ToJSON(Vehicle vehicle)
    {
        JObject returnValue = new()
        {
            new JProperty("id", vehicle.ID),
            new JProperty("userID", vehicle.UserID),
            new JProperty("licensePlate", vehicle.LicensePlate),
            new JProperty("make", vehicle.Make),
            new JProperty("model", vehicle.Model),
            new JProperty("color", vehicle.Color),
            new JProperty("modelYear", vehicle.ModelYear),
            new JProperty("createDateTime", vehicle.CreateDateTime),
            new JProperty("updateDateTime", vehicle.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<Vehicle> vehicles)
    {
        JArray returnValue = new();

        foreach (Vehicle vehicle in vehicles)
        {
            returnValue.Add(ToJSON(vehicle));
        }

        return returnValue;
    }

    public static Vehicle? GetByID(int id)
    {
        MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByIDSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            Vehicle returnValue = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString),
                                 reader["LicensePlate"].ToString(), reader["Make"].ToString(), reader["Model"].ToString(),
                                 reader["Color"].ToString(), Convert.ToInt32(reader["Year"].ToString),
                                 Convert.ToDateTime(reader["CreateDateTime"].ToString), Convert.ToDateTime(reader["UpdateDateTime"].ToString));

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<Vehicle> GetAll()
    {
        List<Vehicle> returnValue = [];

        MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllSQL();

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Vehicle newVehicle = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString),
                                 reader["LicensePlate"].ToString(), reader["Make"].ToString(), reader["Model"].ToString(),
                                 reader["Color"].ToString(), Convert.ToInt32(reader["Year"].ToString),
                                 Convert.ToDateTime(reader["CreateDateTime"].ToString), Convert.ToDateTime(reader["UpdateDateTime"].ToString));

            returnValue.Add(newVehicle);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO Vehicles (UserID, LicensePlate, Make, Model, Color, Year) VALUES (\"{UserID}\", \"{LicensePlate}\", \"{Make}\", \"{Model}\", \"{Color}\", \"{ModelYear}\")";

    public override string ToUpdateSQL() => $"UPDATE Vehicles SET UserID = \"{UserID}\", LicensePlate = \"{LicensePlate}\", Make = \"{Make}\", Model = \"{Model}\", Color = \"{Color}\", Year = \"{ModelYear}\" WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Vehicles WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Vehicles WHERE ID = {ID}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM Vehicles WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM Vehicles";
}