using System.Reflection.Metadata.Ecma335;
using MySql.Data.MySqlClient;
using ZstdSharp.Unsafe;

public static class CDatabaseReservation
{
    public static bool SaveReservation(int userId, int parkingLotId, int vehicleId, DateTime startTime, DateTime endTime, decimal amount)
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"INSERT INTO Reservations VALUES({userId}, {parkingLotId} ,{vehicleId}, '{startTime:yyyy-MM-dd HH:mm:ss}', '{endTime:yyyy-MM-dd HH:mm:ss}', {amount})";

        cmd.Connection = conn;
        cmd.CommandText = query;


        int rowsAffected = cmd.ExecuteNonQuery();
        conn.Close();

        return rowsAffected > 0; 
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

    public static bool DeleteReservation(int reservationId)
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"DELETE FROM Reservations WHERE Id = {reservationId}";

        cmd.Connection = conn;
        cmd.CommandText = query;

        int rowsAffected = cmd.ExecuteNonQuery();
        conn.Close();

        if(rowsAffected > 0)
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

    public static int GetUserIdFromToken(string token)
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT userId FROM Sessions WHERE Token = {token}";
        cmd.Connection = conn;
        cmd.CommandText = query;

        int userId = Convert.ToInt32(cmd.ExecuteScalar());
        conn.Close();
        return userId;
    }

    public static Reservation GetReservationById(int userID)
    {

        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM Reservations WHERE UserID = {userID}";
        cmd.Connection = conn;
        cmd.CommandText = query;

        var reader = cmd.ExecuteReader();

        if(reader.Read())
        {
            return new Reservation
            {
                Id = reader.GetInt32("Id"),
                UserId = reader.GetInt32("UserId"),
                ParkingLotId = reader.GetInt32("ParkingLotId"),
                VehicleId = reader.GetInt32("VehicleId"),
                StartTime = reader.GetDateTime("StartTime"),
                EndTime = reader.GetDateTime("EndTime"),
                Status = reader.GetString("Status"),
                Amount = reader.GetDecimal("Amount")
            };
        }
        return null!;

        
    }
}