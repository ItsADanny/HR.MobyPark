using MySql.Data.MySqlClient;

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
    public DateTime? UpdateDateTime { get; private set; }

    private Vehicle(int id, int userid, string licensePlate, string make, string model, string color, int modelYear, DateTime createDateTime, DateTime? updateDateTime = null)
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

    public static List<Vehicle> GetAll(DBHelper dBHelper)
    {
        List<Vehicle> returnValue = [];

        MySqlConnection conn = new MySqlConnection(dBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllSQL();

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            DateTime CreateDateTime = DateTime.Now;
            DateTime? UpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") CreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") UpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            Vehicle newVehicle = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString()),
                                 reader["LicensePlate"].ToString(), reader["Make"].ToString(), reader["Model"].ToString(),
                                 reader["Color"].ToString(), Convert.ToInt32(reader["Year"].ToString()),
                                 CreateDateTime, UpdateDateTime);

            returnValue.Add(newVehicle);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO Vehicles (ID, UserID, LicensePlate, Make, Model, Color, Year, CreateDateTime, UpdateDateTime) VALUES ({ID}, \"{UserID}\", \"{LicensePlate}\", \"{Make}\", \"{Model}\", \"{Color}\", \"{ModelYear}\", '{CreateDateTime:YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime:YYYY-MM-DD hh:mm:ss}')";
    public static string ToSelectAllSQL() => "SELECT * FROM Vehicles";
    public static string ToDeleteAllSQL() => "DELETE FROM `Vehicles`";
}