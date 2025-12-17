using MySql.Data.MySqlClient;

public class UserActivity
{
    public int ID {get; set;}
    public string Token {get; set;}
    public DateTime CreateDateTime {get; set;}

    public UserActivity(int id, string token, DateTime createDateTime)
    {
        ID = id;
        Token = token;
        CreateDateTime = createDateTime;
    }

    public static List<UserActivity> GetAll(DBHelper dbHelper)
    {
        List<UserActivity> returnValue = [];

        MySqlConnection conn = new MySqlConnection(dbHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllSQL();

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            DateTime CreateDateTime = DateTime.Now;
            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") CreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);

            UserActivity newUserActivity = new(Convert.ToInt32(reader["ID"].ToString()), reader["Token"].ToString(), CreateDateTime);

            returnValue.Add(newUserActivity);
        }

        conn.Close();
        return returnValue;
    }

    public static List<UserActivity> GetAll(DBHelper dbHelper, string token)
    {
        List<UserActivity> returnValue = [];

        MySqlConnection conn = new MySqlConnection(dbHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = ToSelectAllFromTokenSQL(token);

        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            DateTime CreateDateTime = DateTime.Now;
            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") CreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);

            UserActivity newUserActivity = new(Convert.ToInt32(reader["ID"].ToString()), reader["Token"].ToString(), CreateDateTime);

            returnValue.Add(newUserActivity);
        }

        conn.Close();
        return returnValue;
    }

    public static string ToSelectAllSQL() => "SELECT * FROM UserActivity";
    public static string ToSelectAllFromTokenSQL(string token) => $"SELECT * FROM UserActivity WHERE Token = \"{token}\"";
}