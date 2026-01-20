using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

public class UserSession : aModel, iDatabase
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public string Token { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }

    private UserSession(int userID, string token)
    {
        UserID = userID;
        Token = token;
    }

    private UserSession(int id, int userID, string token, DateTime createDateTime, DateTime? updateDateTime = null)
    {
        ID = id;
        UserID = userID;
        Token = token;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static JObject ToJSON(UserSession userSession)
    {
        JObject returnValue = new()
        {
            new JProperty("id", userSession.ID),
            new JProperty("userID", userSession.UserID),
            new JProperty("token", userSession.Token),
            new JProperty("createDateTime", userSession.CreateDateTime),
            new JProperty("updateDateTime", userSession.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<UserSession> UserSession)
    {
        JArray returnValue = new();

        foreach (UserSession userSession in UserSession)
        {
            returnValue.Add(ToJSON(userSession));
        }

        return returnValue;
    }

    public static UserSession? GetByID(int id)
    {
        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectByIDSQL(id);

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            DateTime CreateDateTime = DateTime.Now;
            DateTime? UpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") CreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") UpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            UserSession returnValue = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString()),
                                 reader["Token"].ToString(), CreateDateTime,
                                 UpdateDateTime);

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<UserSession> GetAll()
    {
        List<UserSession> returnValue = [];

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
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

            UserSession newUserSession = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString()),
                                 reader["Token"].ToString(), CreateDateTime,
                                 UpdateDateTime);

            returnValue.Add(newUserSession);
        }

        conn.Close();
        return returnValue;
    }

    public static UserSession? GetUserSession(string authToken)
    {
        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM UserSessions WHERE Token = \"{authToken}\"";
        cmd.Connection = conn;
        cmd.CommandText = query;

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            DateTime CreateDateTime = DateTime.Now;
            DateTime? UpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") CreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") UpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            UserSession returnValue = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString()),
                                 reader["Token"].ToString(), CreateDateTime,
                                 UpdateDateTime);

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
        UserSession newUserSes = new(user.ID, newToken);

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

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM UserSessions WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM UserSessions";
}