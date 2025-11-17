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

    public static int SaveUser(string username, string hash, string fullName, string email, string phoneNumber, int birthYear)
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

    public static User GetUserByID(int userID)
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM Users WHERE ID = {userID}";
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
    public static bool IsUserAdmin(int userID)
    {
        try
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = $"SELECT RoleID FROM Users WHERE ID = @UserID";
            MySqlCommand cmd = new MySqlCommand(query,conn);
            cmd.Parameters.AddWithValue("@UserID", userID);

            object result = cmd.ExecuteScalar();
            if (result != null && Convert.ToInt32(result) == 1)
            {
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public static Payment[] GetAllPaymentsBySession(int UserID)
    {
        try
        {
            using MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            
            string query = "SELECT * FROM Transaction WHERE UserID = @UserID";
            using MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", UserID);
            
            List<Payment> paymentsList = new List<Payment>();
            using MySqlDataReader reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                Payment payment = new Payment
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    TransactionID = reader["TransactionID"].ToString(),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    UserID = Convert.ToInt32(reader["UserID"]),
                    ParkingLotID = Convert.ToInt32(reader["ParkingLotID"]),
                    SessionID = Convert.ToInt32(reader["SessionID"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Method = reader["Method"].ToString(),
                    Issuer = reader["Issuer"].ToString(),
                    Bank = reader["Bank"].ToString(),
                    Hash = reader["Hash"].ToString(),
                    CompletedDateTime = reader["CompletedDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["CompletedDateTime"]) : null,
                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"]),
                    UpdatedDateTime = Convert.ToDateTime(reader["UpdatedDateTime"])
                };
                paymentsList.Add(payment);
            }
            
            return paymentsList.ToArray();
        }
        catch (Exception)
        {
            return new Payment[0];
        }
    }

    public static Payment GetPaymentById( int userId)
    {
        try
        {
            using MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            
            string query = "SELECT * FROM Transaction WHERE UserID = @userId";
            using MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            
            using MySqlDataReader reader = cmd.ExecuteReader();
            
            if (reader.Read())
            {
                return new Payment
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    TransactionID = reader["TransactionID"].ToString(),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    UserID = Convert.ToInt32(reader["UserID"]),
                    ParkingLotID = Convert.ToInt32(reader["ParkingLotID"]),
                    SessionID = Convert.ToInt32(reader["SessionID"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Method = reader["Method"].ToString(),
                    Issuer = reader["Issuer"].ToString(),
                    Bank = reader["Bank"].ToString(),
                    Hash = reader["Hash"].ToString(),
                    CompletedDateTime = reader["CompletedDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["CompletedDateTime"]) : null,
                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"]),
                    UpdatedDateTime = Convert.ToDateTime(reader["UpdatedDateTime"])
                };
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string AddPaymentTransaction(
        string transactionID,
        int parkingLotID,
        int sessionID,
        int userID, 
        decimal amount,
        string method,
        string issuer,
        string bank,
        DateTime createdDateTime,
        DateTime updatedDateTime)
        {
        try
        {
            using MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            string hash = Guid.NewGuid().ToString();
            DateTime now = DateTime.Now;

            string query = @"
                INSERT INTO Transaction 
                (TransactionID, ParkingLotID, SessionID, UserID, Amount, Date, Method, Issuer, Bank, Hash, CompletedDateTime, CreatedDateTime, UpdatedDateTime)
                VALUES 
                (@transactionID, @parkingLotID, @sessionID, @userID, @amount, @date, @method, @issuer, @bank, @hash, @CompletedDateTime, @createdDateTime, @updatedDateTime)";

            using MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@transactionID", transactionID);
            cmd.Parameters.AddWithValue("@parkingLotID", parkingLotID);
            cmd.Parameters.AddWithValue("@sessionID", sessionID);
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@date", now);
            cmd.Parameters.AddWithValue("@method", method);
            cmd.Parameters.AddWithValue("@issuer", issuer);
            cmd.Parameters.AddWithValue("@bank", bank);
            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.Parameters.AddWithValue("@CompletedDateTime", DateTime.MaxValue);
            cmd.Parameters.AddWithValue("@createdDateTime", createdDateTime);
            cmd.Parameters.AddWithValue("@updatedDateTime", updatedDateTime);

            int rowsAffected = cmd.ExecuteNonQuery();
            
            return rowsAffected > 0 ? transactionID : null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}