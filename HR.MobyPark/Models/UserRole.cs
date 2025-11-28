using MySql.Data.MySqlClient;

public class UserRole : aDatabase, iDatabase
{
    public int? ID {get; private set;}
    public string Name {get; private set;}
    //Read/Write premission for users own information
    public Premission User {get; private set;}
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

    public UserRole(int? id, string name, int user, int reservation, int vehicle, int payments, int globalUsers,
                    int globalReservations, int globalVehicles, int globalParkingLots, int globalPayments,
                    int globalUserRole, DateTime createDateTime, DateTime updateDateTime)
    {
        ID = id;
        Name = name;
        User = new Premission(user);
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
                int ID = Convert.ToInt32(reader["ID"].ToString());
                string Name = reader["Name"].ToString();
                int UserPremission = Convert.ToInt32(reader["User"].ToString());
                int ReservationPremission = Convert.ToInt32(reader["Reservation"].ToString());
                int VehiclePremission = Convert.ToInt32(reader["Vehicle"].ToString());
                int PaymentsPremission = Convert.ToInt32(reader["Payments"].ToString());
                int GlobalUsersPremission = Convert.ToInt32(reader["GlobalUsers"].ToString());
                int GlobalReservationPremission = Convert.ToInt32(reader["GlobalReservations"].ToString());
                int GlobalParkingLotsPremission = Convert.ToInt32(reader["GlobalParkingLots"].ToString());
                int GlobalPaymentsPremission = Convert.ToInt32(reader["GlobalPayments"].ToString());
                int GlobalUserRolePremission = Convert.ToInt32(reader["GlobalUserRole"].ToString());
                DateTime CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString());
                DateTime UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString());
                
                UserRole userRole = new(ID, Name, UserPremission, ReservationPremission, VehiclePremission,
                                        PaymentsPremission, GlobalUsersPremission, GlobalReservationPremission,
                                        GlobalReservationPremission, GlobalParkingLotsPremission,
                                        GlobalPaymentsPremission, GlobalUserRolePremission,
                                        CreateDateTime, UpdateDateTime);
                
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

    public override string ToInsertSQL() => $"INSERT INTO UserRole (Name, User, Reservation, Vehicle, Payments, GlobalUsers, GlobalReservations, GlobalParkingLots, GlobalPayments, GlobalUserRole) " +
                                   $"VALUES (\"{Name}\", {User.premissionInteger}, {Reservation.premissionInteger}, {Vehicle.premissionInteger}, {Payments.premissionInteger}, {GlobalUsers.premissionInteger}" +
                                   $", {GlobalReservations.premissionInteger}, {GlobalParkingLots.premissionInteger}, {GlobalPayments.premissionInteger}, {GlobalUserRoles.premissionInteger})";

    public override string ToUpdateSQL() => $"UPDATE UserRole SET Name = \"{Name}\", User = {User.premissionInteger} , Reservation = {Reservation.premissionInteger}, Vehicle = {Vehicle.premissionInteger}" + 
                                   $", Payments = {Payments.premissionInteger}, GlobalUsers = {GlobalUsers.premissionInteger}, GlobalReservations = {GlobalReservations.premissionInteger}, GlobalParkingLots = {GlobalParkingLots.premissionInteger}" + 
                                   $", GlobalPayments = {GlobalPayments.premissionInteger}, GlobalUserRole = {GlobalUserRoles.premissionInteger} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM UserRole WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM UserRole WHERE ID = {ID}";
}