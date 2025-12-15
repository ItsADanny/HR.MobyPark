using MySql.Data.MySqlClient;

public class UserRole : aModel, iDatabase
{
    public int? ID {get; private set;}
    public string Name {get; private set;}
    //Read/Write premission for users own information
    public int User {get; private set;}
    //premission for parkinglots
    public int ParkingLots {get; private set;}
    //premission for users own reservations
    public int Reservation {get; private set;}
    //premission for users own Vehicles
    public int Vehicle {get; private set;}
    //premission for users own Payments
    public int Payments {get; private set;}
    //premission for GLOBAL user informations
    public int GlobalUsers {get; private set;}
    //premission for GLOBAL reservations
    public int GlobalReservations {get; private set;}
    //premission for GLOBAL Vehicles
    public int GlobalVehicle {get; private set;}
    //premission for GLOBAL ParkingLots
    public int GlobalParkingLots {get; private set;}
    //premission for GLOBAL Payments
    public int GlobalPayments {get; private set;}
    //premission for GLOBAL Userrole
    public int GlobalUserRoles {get; private set;}
    public DateTime CreateDateTime {get; private set;}
    public DateTime? UpdateDateTime {get; private set;}

    public UserRole(int? id, string name, int user, int parkinglots, int reservation, int vehicle, int payments, int globalUsers,
                    int globalReservations, int globalVehicles, int globalParkingLots, int globalPayments,
                    int globalUserRole, DateTime createDateTime, DateTime? updateDateTime=null)
    {
        ID = id;
        Name = name;
        User = user;
        ParkingLots = parkinglots;
        //Premissions for users own reservations
        Reservation = reservation;
        //Premission for users own Vehicles
        Vehicle = vehicle;
        //Premission for users own Payments
        Payments = payments;
        //Premissions for GLOBAL user informations
        GlobalUsers = globalUsers;
        //premissions for GLOBAL reservations
        GlobalReservations = globalReservations;
        //premissions for GLOBAL Vehicles
        GlobalVehicle = globalVehicles;
        //premissions for GLOBAL ParkingLots
        GlobalParkingLots = globalParkingLots;
        //premissions for GLOBAL Payments
        GlobalPayments = globalPayments;
        //premissions for GLOBAL UserRoles
        GlobalUserRoles = globalUserRole;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static List<UserRole> GetAll(DBHelper dBHelper)
    {
        List<UserRole> returnValue = [];

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

            UserRole newUserRole = new(Convert.ToInt32(reader["ID"].ToString()), reader["Name"].ToString(),
                                 Convert.ToInt32(reader["User"].ToString()), Convert.ToInt32(reader["ParkingLots"].ToString()), Convert.ToInt32(reader["Reservation"].ToString()),
                                 Convert.ToInt32(reader["Vehicle"].ToString()), Convert.ToInt32(reader["Payments"].ToString()), 
                                 Convert.ToInt32(reader["GlobalUsers"].ToString()), Convert.ToInt32(reader["GlobalReservations"].ToString()), Convert.ToInt32(reader["GlobalVehicle"].ToString()),
                                 Convert.ToInt32(reader["GlobalParkingLots"].ToString()), Convert.ToInt32(reader["GlobalPayments"].ToString()), 
                                 Convert.ToInt32(reader["GlobalUserRole"].ToString()),
                                 CreateDateTime, UpdateDateTime);

            returnValue.Add(newUserRole);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO UserRole (ID, Name, User, ParkingLots, Reservation, Vehicle, Payments, GlobalUsers, GlobalReservations, GlobalParkingLots, GlobalPayments, GlobalUserRole, CreateDateTime, UpdateDateTime) " +
                                   $"VALUES ({ID}, \"{Name}\", {User}, {ParkingLots}, {Reservation}, {Vehicle}, {Payments}, {GlobalUsers}" +
                                   $", {GlobalReservations}, {GlobalParkingLots}, {GlobalPayments}, {GlobalUserRoles}, '{CreateDateTime:YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime:YYYY-MM-DD hh:mm:ss}')";

    public static string ToSelectAllSQL() => "SELECT * FROM UserRole";
    public static string ToDeleteAllSQL() => "DELETE FROM `UserRole`";
}