using MySql.Data.MySqlClient;

public static class CDatabaseReservation
{
    public static int GetUserIdFromToken(string token)
    {
        return 1;
    }

    public static int SaveReservation(int userId, int parkingLotId, int vehicleId, DateTime startTime, DateTime endTime, decimal cost)
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"INSERT INTO Reservations VALUES({userId}, {parkingLotId} ,{vehicleId}, '{startTime:yyyy-MM-dd HH:mm:ss}', '{endTime:yyyy-MM-dd HH:mm:ss}', {cost})";

        cmd.Connection = conn;
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();

        string query2 = $"SELECT ID FROM Reservations WHERE UserId = {userId} AND StartTime = {startTime}";
        cmd.Connection = conn;
        cmd.CommandText = query2;

        int id = Convert.ToInt32(cmd.ExecuteScalar());
        conn.Close();

        return id;
    }
}