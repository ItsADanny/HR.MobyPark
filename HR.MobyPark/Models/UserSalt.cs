using MySql.Data.MySqlClient;

public class UserSalt : aDatabase, iDatabase
{
    public int ID {get; private set;}
    public int UserID {get; private set;}
    public string Salt {get; private set;}
    public DateTime CreateDateTime {get; private set;}
    public DateTime UpdateDateTime {get; private set;}

    public UserSalt(int userID, string salt)
    {
        UserID = userID;
        Salt = salt;
    }

    public UserSalt(int id, int userID, string salt, DateTime createDateTime, DateTime updateDateTime)
    {
        ID = id;
        UserID = userID;
        Salt = salt;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static UserSalt? GetSalt(int userID)
    {
        MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
        MySqlCommand cmd = new MySqlCommand();

        conn.Open();
        string query = $"SELECT * FROM Sessions WHERE Token = {userID}";
        cmd.Connection = conn;
        cmd.CommandText = query;

        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            UserSalt returnValue = new(
                Convert.ToInt32(reader["ID"].ToString), 
                Convert.ToInt32(reader["UserID"].ToString), 
                reader["Salt"].ToString(), Convert.ToDateTime(reader["CreateDateTime"].ToString),
                Convert.ToDateTime(reader["UpdateDateTime"].ToString));
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
}