using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

public class UserSalt : aModel, iDatabase
{
    public int ID {get; private set;}
    public int UserID {get; private set;}
    public string Salt {get; set;}
    public DateTime CreateDateTime {get; private set;}
    public DateTime? UpdateDateTime {get; private set;}

    public UserSalt(int userID, string salt)
    {
        UserID = userID;
        Salt = salt;
    }

    public UserSalt(int id, int userID, string salt, DateTime createDateTime, DateTime? updateDateTime=null)
    {
        ID = id;
        UserID = userID;
        Salt = salt;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static JObject ToJSON(UserSalt userSalt)
    {
        JObject returnValue = new()
        {
            new JProperty("id", userSalt.ID),
            new JProperty("userID", userSalt.UserID),
            new JProperty("salt", userSalt.Salt),
            new JProperty("createDateTime", userSalt.CreateDateTime),
            new JProperty("updateDateTime", userSalt.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<UserSalt> userSalts)
    {
        JArray returnValue = new();

        foreach (UserSalt userSalt in userSalts)
        {
            returnValue.Add(ToJSON(userSalt));
        }

        return returnValue;
    }

    public static UserSalt? GetByID(int id)
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

            UserSalt returnValue = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString()),
                                 reader["Salt"].ToString(), CreateDateTime,
                                 UpdateDateTime);

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<UserSalt> GetAll()
    {
        List<UserSalt> returnValue = [];

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

            UserSalt newUserSalt = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["UserID"].ToString()),
                                 reader["Salt"].ToString(), CreateDateTime,
                                 UpdateDateTime);

            returnValue.Add(newUserSalt);
        }

        conn.Close();
        return returnValue;
    }

    public static UserSalt? GetSalt(int userID)
    {
        MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM UserSalt WHERE UserID = {userID}";
        cmd.Connection = conn;
        cmd.CommandText = query;

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            DateTime CreateDateTime = DateTime.Now;
            DateTime? UpdateDateTime = null;

            string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
            string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

            if (CreateDateTime_STR is not null && CreateDateTime_STR != "") CreateDateTime = reader.GetDateTime("CreateDateTime");
            if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") UpdateDateTime = reader.GetDateTime("UpdateDateTime");

            UserSalt returnValue = new(
                Convert.ToInt32(reader["ID"].ToString()), 
                Convert.ToInt32(reader["UserID"].ToString()), 
                reader["Salt"].ToString(), CreateDateTime,
                UpdateDateTime);
            conn.Close();

            return returnValue;
        }

        conn.Close();
        return null;
    }

    public override string ToInsertSQL() => $"INSERT INTO UserSalt (UserID, Salt) VALUES (\"{UserID}\", \"{Salt}\")";

    public override string ToUpdateSQL() => $"UPDATE UserSalt SET UserID = \"{UserID}\", Salt = \"{Salt}\" WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM UserSalt WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM UserSalt WHERE ID = {ID}";

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM UserSalt WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM UserSalt";
}