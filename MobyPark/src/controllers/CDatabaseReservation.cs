using System.Reflection.Metadata.Ecma335;
using MySql.Data.MySqlClient;

public static class CDatabaseReservation
{
    public static int GetUserIdFromToken(string token)
    {
        Database Database = new Database();
        
        MySqlConnection conn = new MySqlConnection(Database.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT userId FROM Sessions WHERE Token = {token}";
        cmd.Connection = conn;
        cmd.CommandText = query;

        int userId = Convert.ToInt32(cmd.ExecuteScalar());
        conn.Close();
        return userId;
    }

    public static int SaveReservation(int userId, int parkingLotId, int vehicleId, DateTime startTime, DateTime endTime, decimal amount)
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"INSERT INTO Reservations VALUES({userId}, {parkingLotId} ,{vehicleId}, '{startTime:yyyy-MM-dd HH:mm:ss}', '{endTime:yyyy-MM-dd HH:mm:ss}', {amount})";

        cmd.Connection = conn;
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();

        string query2 = $"SELECT ID FROM Reservations WHERE UserId = {userId} AND StartTime = '{startTime:yyyy-MM-dd HH:mm:ss}'";
        cmd.Connection = conn;
        cmd.CommandText = query2;

        int id = Convert.ToInt32(cmd.ExecuteScalar());
        conn.Close();

        return id;
    }

    public static bool UpdateReservation(int reservationId, int parkingLotId, int vehicleId, DateTime startTime, DateTime endTime, decimal amount)
    {

        // Database Database = new Database();
        // string connectionString = Database.GetConnectionSTR();

        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        if (ReservationExists(reservationId) == false)
        {
            return false;
        }

        conn.Open();

        string Query = $"UPDATE Reservations SET ParkingLotId = {parkingLotId}, VehicleId = {vehicleId}, StartTime = '{startTime:yyyy-MM-dd HH:mm}', EndTime = '{endTime:yyyy-MM-dd HH:mm}', Amount = {amount} WHERE ID = {reservationId}";

        cmd.Connection = conn;
        cmd.CommandText = Query;

        int RowAffected = cmd.ExecuteNonQuery();

        conn.Close();

        if (RowAffected > 0)
        {
            return true;
        }

        return false;


    }

    public static bool ReservationExists(int reservationId)
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT COUNT(*) FROM Reservations WHERE ID = {reservationId}";

        cmd.Connection = conn;
        cmd.CommandText = query;        
        int count = Convert.ToInt32(cmd.ExecuteScalar());

        conn.Close();
        if (count > 0)
        {
            return true;
        }
        return false;
    }
}