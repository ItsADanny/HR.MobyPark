using MySql.Data.MySqlClient;

public static class CDatabase
{
    // enviroment var's
    public static bool UserExists(string username)
    {
        // hier de database logica om te checken of de gebruiker bestaat
        // return true als de gebruiker bestaat, anders false
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT COUNT(*) FROM Users WHERE Username = {username}";
        cmd.Connection = conn;
        cmd.CommandText = query;
        int count = Convert.ToInt32(cmd.ExecuteScalar());
        conn.Close();
        if (count > 0)
        {
            return true;
        }
        return false; // placeholder
    }

    public static int SaveUser(string username, string hash, string fullName, string email, string phoneNumber)
    {
        // hier de database logica om de gebruiker op te slaan
        // return de gegenereerde gebruikers-ID
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"INSERT INTO Users VALUES({username}, {hash} ,{fullName}, {email}, {phoneNumber})";
        cmd.Connection = conn;
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();
        string query2 = $"SELECT ID FROM Users WHERE Username = {username}";
        cmd.CommandText = query2;
        int id = Convert.ToInt32(cmd.ExecuteScalar());
        conn.Close();
        return id; // placeholder
    }

    public static void SaveUserSalt(int userId, string salt)
    {
        // hier de database logica om de salt op te slaan voor de gebruiker
        // return de gegenereerde gebruikers-ID
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();

        string query = $"INSERT INTO Salts VALUES({userId}, {salt})";
        cmd.Connection = conn;
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();

        conn.Close();
    }

    // voor login, checken of username klopt tov. database
    public static User GetUser(string username)
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM Users WHERE Username = {username}";
        cmd.Connection = conn;
        cmd.CommandText = query;

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            User user = new User
            {
                Username = reader["Username"].ToString(),
                // de rest ook
            };
            conn.Close();
            return user;
        }
        else
        {
            conn.Close();
            return null;
        }
    }
    public static string GetUsernameById(int userId)
{
    try
    {
        using MySqlConnection conn = new MySqlConnection(connectionString);
        conn.Open();

        string query = "SELECT Username FROM Users WHERE ID = @userId";
        using MySqlCommand cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@userId", userId);

        object result = cmd.ExecuteScalar();
        return result != null ? Convert.ToString(result) : null;
    }
    catch (Exception)
    {
        return null;
    }
}
    public static Payments[] GetPaymentsBySession(string username)
    {
        try
        {
            using MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM Payments WHERE Initiator = @username";
            using MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);

            List<Payments> paymentsList = new List<Payments>();
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Payments payment = new Payments
                {
                    Transaction = Convert.ToString(reader["Transaction"]),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Initiator = Convert.ToString(reader["Initiator"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    Completed = Convert.ToDateTime(reader["Completed"]),
                    Hash = Convert.ToString(reader["Hash"]),
                    SessionId = Convert.ToString(reader["SessionId"]),
                    ParkingLotId = Convert.ToString(reader["ParkingLotId"]),
                };
                paymentsList.Add(payment);
            }


            return paymentsList.ToArray();
        }
        catch (Exception)
        {
            return new Payments[0];
        }
    }
}
