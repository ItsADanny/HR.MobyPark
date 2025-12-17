using MySql.Data.MySqlClient;

public class UserSession
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

    public static UserSession? GetByID(DBHelper dbHelper, int id)
    {
        MySqlConnection conn = new MySqlConnection(dbHelper.GetConnectionSTR());
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

            UserSession returnValue = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString),
                                 reader["Token"].ToString(), CreateDateTime,
                                 UpdateDateTime);

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<UserSession> GetAll(DBHelper dbHelper)
    {
        List<UserSession> returnValue = [];

        MySqlConnection conn = new MySqlConnection(dbHelper.GetConnectionSTR());
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

            UserSession newUserSession = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString),
                                 reader["Token"].ToString(), CreateDateTime,
                                 UpdateDateTime);

            returnValue.Add(newUserSession);
        }

        conn.Close();
        return returnValue;
    }

    public static UserSession? GetUserSession(DBHelper dbHelper, string authToken)
    {
        MySqlConnection conn = new MySqlConnection(dbHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM Sessions WHERE Token = {authToken}";
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

            UserSession returnValue = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString),
                                 reader["Token"].ToString(), CreateDateTime,
                                 UpdateDateTime);

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static bool SessionExist(DBHelper dbHelper, string token) => GetUserSession(dbHelper, token) is not null;

    public static bool RemoveSession(DBHelper dbHelper, string token)
    {
        UserSession? foundSession = GetUserSession(dbHelper, token);
        if (foundSession is not null)
        {
            return foundSession.DeleteDB(dbHelper);
        }
        return false;
    }

    public static string ToDeleteSQL(int id) => $"DELETE FROM UserSessions WHERE ID = {id}";

    public static string ToSelectSQL(int id) => $"SELECT * FROM UserSessions WHERE ID = {id}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM UserSessions WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM UserSessions";

    public bool DeleteDB(DBHelper dbHelper) => dbHelper.Delete(ToDeleteSQL(ID));
}