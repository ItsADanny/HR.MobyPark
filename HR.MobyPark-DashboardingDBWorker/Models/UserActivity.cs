using MySql.Data.MySqlClient;

public class UserActivity : aModel, iDatabase
{
    public int ID {get; set;}
    public int UserID {get; set;}
    public string Token {get; set;}
    public string Endpoint {get; set;}
    public string Method {get; set;}
    public DateTime CreateDateTime {get; set;}

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
            DateTime RCreateDateTime = DateTime.Now;
            DateTime? RUpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") RCreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") RUpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

            UserActivity newUserActivity = new()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                UserID = Convert.ToInt32(reader["UserID"].ToString()),
                Token = reader["Token"].ToString(),
                Endpoint = reader["Endpoint"].ToString(),
                Method = reader["Method"].ToString(),
                CreateDateTime = RCreateDateTime
            };

            returnValue.Add(newUserActivity);
        }

        conn.Close();
        return returnValue;
    }

    public static string ToSelectAllSQL() => "SELECT * FROM UserActivity";
    public static string ToDeleteAllSQL() => "DELETE FROM `UserActivity`";

    //IDatabase implementation
    public override string ToInsertSQL() => $"INSERT INTO UserActivity (ID, UserID, Token, Endpoint, Method, CreateDateTime) VALUES ({ID}, {UserID}, \"{Token}\", \"{Endpoint}\", \"{Method}\", '{CreateDateTime::YYYY-MM-DD hh:mm:ss}')";
}