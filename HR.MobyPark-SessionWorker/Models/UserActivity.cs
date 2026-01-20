using MySql.Data.MySqlClient;

public class UserActivity : aModel, iDatabase
{
    public int ID {get; set;}
    public int UserID {get; set;}
    public string Token {get; set;}
    public string Endpoint {get; set;}
    public EHTTPMethod Method {get; set;}
    public DateTime CreateDateTime {get; set;}

    //IDatabase implementation
    public override string ToInsertSQL() => $"INSERT INTO UserActivity (UserID, Token, Endpoint, Method) VALUES ({UserID}, \"{Token}\", \"{Endpoint}\", \"{Method}\")";

    public override string ToUpdateSQL() => $"UPDATE UserActivity SET UserID = {UserID}, Token = \"{Token}\", Endpoint = \"{Endpoint}\", Method = \"{Method}\" WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM UserActivity WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM UserActivity WHERE ID = {ID}";

    public static void SaveActivity(UserSession userSession, string endpoint, EHTTPMethod eHTTPMethod)
    {
        UserActivity newUserActivity = new()
        {
            UserID = userSession.ID,
            Token = userSession.Token,
            Endpoint = endpoint,
            Method = eHTTPMethod
        };

        if (newUserActivity.CreateDB())
        {
            Log.WriteLine("UserActivity:SaveActivity():CreateDB(), Insertion successful, UserActivity saved to Database");
        } 
        else
        {
            Log.WriteErrorLine("UserActivity:SaveActivity():CreateDB(), Insertion failed, please check why the insertion failed");
        }
    }

    public static void SaveActivity(string token, string endpoint, EHTTPMethod eHTTPMethod)
    {
        UserSession? foundUserSession = UserSession.GetUserSession(token);
        if (foundUserSession is not null) SaveActivity(foundUserSession, endpoint, eHTTPMethod);
    }

    public static List<UserActivity> GetAll()
    {
        List<UserActivity> returnValue = new();

        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = "SELECT * FROM UserActivity";
        cmd.Connection = conn;
        cmd.CommandText = query;

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            UserActivity newUserActivity = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                Token = reader["Token"].ToString() ?? "",
                Endpoint = reader["Endpoint"].ToString() ?? "",
                Method = (EHTTPMethod)Enum.Parse(typeof(EHTTPMethod), reader["Method"].ToString() ?? "GET"),
                CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString())
            };

            returnValue.Add(newUserActivity);
        }

        conn.Close();
        return returnValue;
    }
}