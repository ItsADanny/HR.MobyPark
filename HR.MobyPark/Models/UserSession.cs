using MySql.Data.MySqlClient;

public class UserSession : aDatabase, iDatabase
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public string Token { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }

    public static UserSession? GetUserSession(string authToken)
    {
        MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM Sessions WHERE Token = {authToken}";
        cmd.Connection = conn;
        cmd.CommandText = query;

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            UserSession returnValue = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                Token = reader["ID"].ToString(),
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
            };

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static string? CreateSession(User user)
    {
        //Get a new secure token from Auth, and add the UserID to the end
        string newToken = Auth.GenAuthToken() + user.ID;
        
        //Create a new user session object
        UserSession newUserSes = new()
        {
            UserID = user.ID,
            Token = newToken
        };

        if (newUserSes.CreateDB()) return newToken;
        return null;
    }

    public static bool SessionExist(string token) => GetUserSession(token) is not null;

    public static bool RemoveSession(string token)
    {
        UserSession? foundSession = GetUserSession(token);
        if (foundSession is not null)
        {
            return foundSession.DeleteDB();
        }
        return false;
    }

    public override string ToInsertSQL() => $"INSERT INTO UserSessions (UserID, Token) VALUES (\"{UserID}\", \"{Token}\")";

    public override string ToUpdateSQL() => $"UPDATE UserSessions SET UserID = \"{UserID}\", Token = \"{Token}\" WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM UserSessions WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM UserSessions WHERE ID = {ID}";
}