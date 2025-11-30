using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

public class UserRole : aModel, iDatabase
{
    public int? ID {get; private set;}
    public string Name {get; private set;}
    //Read/Write premission for users own information
    public Premission User {get; private set;}
    //premission for parkinglots
    public Premission ParkingLots {get; private set;}
    //premission for users own reservations
    public Premission Reservation {get; private set;}
    //premission for users own Vehicles
    public Premission Vehicle {get; private set;}
    //premission for users own Payments
    public Premission Payments {get; private set;}
    //premission for GLOBAL user informations
    public Premission GlobalUsers {get; private set;}
    //premission for GLOBAL reservations
    public Premission GlobalReservations {get; private set;}
    //premission for GLOBAL Vehicles
    public Premission GlobalVehicle {get; private set;}
    //premission for GLOBAL ParkingLots
    public Premission GlobalParkingLots {get; private set;}
    //premission for GLOBAL Payments
    public Premission GlobalPayments {get; private set;}
    //premission for GLOBAL Userrole
    public Premission GlobalUserRoles {get; private set;}
    public DateTime CreateDateTime {get; private set;}
    public DateTime UpdateDateTime {get; private set;}

    public UserRole(int? id, string name, int user, int parkinglots, int reservation, int vehicle, int payments, int globalUsers,
                    int globalReservations, int globalVehicles, int globalParkingLots, int globalPayments,
                    int globalUserRole, DateTime createDateTime, DateTime updateDateTime)
    {
        ID = id;
        Name = name;
        User = new Premission(user);
        ParkingLots = new Premission(parkinglots);
        //Premissions for users own reservations
        Reservation = new Premission(reservation);
        //Premission for users own Vehicles
        Vehicle = new Premission(vehicle);
        //Premission for users own Payments
        Payments = new Premission(payments);
        //Premissions for GLOBAL user informations
        GlobalUsers = new Premission(globalUsers);
        //premissions for GLOBAL reservations
        GlobalReservations = new Premission(globalReservations);
        //premissions for GLOBAL Vehicles
        GlobalVehicle = new Premission(globalVehicles);
        //premissions for GLOBAL ParkingLots
        GlobalParkingLots = new Premission(globalParkingLots);
        //premissions for GLOBAL Payments
        GlobalPayments = new Premission(globalPayments);
        //premissions for GLOBAL UserRoles
        GlobalUserRoles = new Premission(globalUserRole);
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    //Creating a new UserRole without any premissions
    public UserRole(int? id, string name)
    {
        ID = id;
        Name = name;
        User = new Premission();
        ParkingLots = new Premission();
        //Premissions for users own reservations
        Reservation = new Premission();
        //Premission for users own Vehicles
        Vehicle = new Premission();
        //Premission for users own Payments
        Payments = new Premission();
        //Premissions for GLOBAL user informations
        GlobalUsers = new Premission();
        //premissions for GLOBAL reservations
        GlobalReservations = new Premission();
        //premissions for GLOBAL Vehicles
        GlobalVehicle = new Premission();
        //premissions for GLOBAL ParkingLots
        GlobalParkingLots = new Premission();
        //premissions for GLOBAL Payments
        GlobalPayments = new Premission();
        //premissions for GLOBAL UserRoles
        GlobalUserRoles = new Premission();
        CreateDateTime = DateTime.Now;
        UpdateDateTime = DateTime.Now;
    }

    public static JObject ToJSON(UserRole userRole)
    {
        JProperty[] Premissions = [
            new JProperty("User", userRole.User.premissionInteger),
            new JProperty("ParkingLots", userRole.ParkingLots.premissionInteger),
            new JProperty("Reservation", userRole.Reservation.premissionInteger),
            new JProperty("Vehicle", userRole.Vehicle.premissionInteger),
            new JProperty("Payments", userRole.Payments.premissionInteger)
        ];

        JProperty[] GlobalPremissions = [
            new JProperty("Users", userRole.GlobalUsers.premissionInteger),
            new JProperty("Reservations", userRole.GlobalReservations.premissionInteger),
            new JProperty("Vehicle", userRole.GlobalVehicle.premissionInteger),
            new JProperty("ParkingLots", userRole.GlobalParkingLots.premissionInteger),
            new JProperty("Payments", userRole.GlobalPayments.premissionInteger),
            new JProperty("UserRoles", userRole.GlobalUserRoles.premissionInteger)
        ];

        JObject returnValue = new()
        {
            new JProperty("id", userRole.ID),
            new JProperty("Name", userRole.Name),
            new JObject("Premissions", Premissions),
            new JObject("GlobalPremissions", GlobalPremissions),
            new JProperty("createDateTime", userRole.CreateDateTime),
            new JProperty("updateDateTime", userRole.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<UserRole> userRoles)
    {
        JArray returnValue = new();

        foreach (UserRole userRole in userRoles)
        {
            returnValue.Add(ToJSON(userRole));
        }

        return returnValue;
    }

    public static UserRole? GetByID(int id)
    {
        MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByIDSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            UserRole returnValue = new(Convert.ToInt32(reader["ID"].ToString()), reader["Name"].ToString(),
                                 Convert.ToInt32(reader["User"].ToString()), Convert.ToInt32(reader["ParkingLots"].ToString()), Convert.ToInt32(reader["Reservation"].ToString()),
                                 Convert.ToInt32(reader["Vehicle"].ToString()), Convert.ToInt32(reader["Payments"].ToString()), 
                                 Convert.ToInt32(reader["GlobalUsers"].ToString()), Convert.ToInt32(reader["GlobalReservations"].ToString()), Convert.ToInt32(reader["GlobalVehicle"].ToString()),
                                 Convert.ToInt32(reader["GlobalParkingLots"].ToString()), Convert.ToInt32(reader["GlobalPayments"].ToString()), 
                                 Convert.ToInt32(reader["GlobalUserRole"].ToString()),
                                 Convert.ToDateTime(reader["CreateDateTime"].ToString), Convert.ToDateTime(reader["UpdateDateTime"].ToString));

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<UserRole> GetAll()
    {
        List<UserRole> returnValue = [];

        MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllSQL();

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            UserRole newUserRole = new(Convert.ToInt32(reader["ID"].ToString()), reader["Name"].ToString(),
                                 Convert.ToInt32(reader["User"].ToString()), Convert.ToInt32(reader["ParkingLots"].ToString()), Convert.ToInt32(reader["Reservation"].ToString()),
                                 Convert.ToInt32(reader["Vehicle"].ToString()), Convert.ToInt32(reader["Payments"].ToString()), 
                                 Convert.ToInt32(reader["GlobalUsers"].ToString()), Convert.ToInt32(reader["GlobalReservations"].ToString()), Convert.ToInt32(reader["GlobalVehicle"].ToString()),
                                 Convert.ToInt32(reader["GlobalParkingLots"].ToString()), Convert.ToInt32(reader["GlobalPayments"].ToString()), 
                                 Convert.ToInt32(reader["GlobalUserRole"].ToString()),
                                 Convert.ToDateTime(reader["CreateDateTime"].ToString), Convert.ToDateTime(reader["UpdateDateTime"].ToString));

            returnValue.Add(newUserRole);
        }

        conn.Close();
        return returnValue;
    }

    public static UserRole? GetUserRole(int id) => _mGetUserRole($"SELECT * FROM UserRole WHERE ID = {id}");

    //Private static - Database interactions
    private static UserRole? _mGetUserRole(string sqlStatement)
    {
        try
        {
            MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sqlStatement;

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {   
                UserRole userRole = new(Convert.ToInt32(reader["ID"].ToString()), reader["Name"].ToString(),
                                 Convert.ToInt32(reader["User"].ToString()), Convert.ToInt32(reader["ParkingLots"].ToString()), Convert.ToInt32(reader["Reservation"].ToString()),
                                 Convert.ToInt32(reader["Vehicle"].ToString()), Convert.ToInt32(reader["Payments"].ToString()), 
                                 Convert.ToInt32(reader["GlobalUsers"].ToString()), Convert.ToInt32(reader["GlobalReservations"].ToString()), Convert.ToInt32(reader["GlobalVehicle"].ToString()),
                                 Convert.ToInt32(reader["GlobalParkingLots"].ToString()), Convert.ToInt32(reader["GlobalPayments"].ToString()), 
                                 Convert.ToInt32(reader["GlobalUserRole"].ToString()),
                                 Convert.ToDateTime(reader["CreateDateTime"].ToString), Convert.ToDateTime(reader["UpdateDateTime"].ToString));
                
                conn.Close();
                return userRole;
            }
            else
            {
                conn.Close();
                return null;
            } 
        }
        catch
        {
            return null;
        }
    }

    public override string ToInsertSQL() => $"INSERT INTO UserRole (Name, User, ParkingLots, Reservation, Vehicle, Payments, GlobalUsers, GlobalReservations, GlobalParkingLots, GlobalPayments, GlobalUserRole) " +
                                   $"VALUES (\"{Name}\", {User.premissionInteger}, {ParkingLots.premissionInteger}, {Reservation.premissionInteger}, {Vehicle.premissionInteger}, {Payments.premissionInteger}, {GlobalUsers.premissionInteger}" +
                                   $", {GlobalReservations.premissionInteger}, {GlobalParkingLots.premissionInteger}, {GlobalPayments.premissionInteger}, {GlobalUserRoles.premissionInteger})";

    public override string ToUpdateSQL() => $"UPDATE UserRole SET Name = \"{Name}\", User = {User.premissionInteger} , ParkingLots = {ParkingLots.premissionInteger}, Reservation = {Reservation.premissionInteger}, Vehicle = {Vehicle.premissionInteger}" + 
                                   $", Payments = {Payments.premissionInteger}, GlobalUsers = {GlobalUsers.premissionInteger}, GlobalReservations = {GlobalReservations.premissionInteger}, GlobalParkingLots = {GlobalParkingLots.premissionInteger}" + 
                                   $", GlobalPayments = {GlobalPayments.premissionInteger}, GlobalUserRole = {GlobalUserRoles.premissionInteger} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM UserRole WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM UserRole WHERE ID = {ID}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM UserRole WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM UserRole";
}