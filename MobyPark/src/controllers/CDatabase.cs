using MySql.Data.MySqlClient;

public static class CDatabase
{
    // enviroment var's
    public static bool UserExists(string username)
    {
        // hier de database logica om te checken of de gebruiker bestaat
        // return true als de gebruiker bestaat, anders false
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCoomand cmd = new MySqlCommand();

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
        MySqlCoomand cmd = new MySqlCommand();

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
        MySqlCoomand cmd = new MySqlCommand();

        conn.Open();

        string query = $"INSERT INTO Salts VALUES({userId}, {salt})";
        cmd.Connection = conn;
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();

        conn.Close();
    }
}