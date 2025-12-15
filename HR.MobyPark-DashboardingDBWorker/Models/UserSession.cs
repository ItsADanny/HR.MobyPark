using MySql.Data.MySqlClient;

public class UserSession : aModel, iDatabase
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }

    private UserSession(int id, int userID, DateTime createDateTime, DateTime? updateDateTime = null)
    {
        ID = id;
        UserID = userID;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static List<UserSession> GetAll(DBHelper dBHelper)
    {
        List<UserSession> returnValue = [];

        MySqlConnection conn = new MySqlConnection(dBHelper.GetConnectionSTR());
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

            UserSession newUserSession = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString()), CreateDateTime,
                                 UpdateDateTime);

            returnValue.Add(newUserSession);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO UserSessions (ID, UserID, CreateDateTime, UpdateDateTime) VALUES ({ID}, \"{UserID}\", '{CreateDateTime:YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime:YYYY-MM-DD hh:mm:ss}')";
    public static string ToSelectAllSQL() => "SELECT * FROM UserSessions";
    public static string ToDeleteAllSQL() => "DELETE FROM `UserSessions`";
}