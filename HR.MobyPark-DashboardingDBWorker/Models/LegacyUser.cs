using MySql.Data.MySqlClient;

public class LegacyUser : aModel, iDatabase
{
    public int ID {get; set;}
    public int OrigID {get; set;}
    public int NewUserID {get; set;}
    public string Username {get; set;}
    public string Name {get; set;}
    public string Email {get; set;}
    public string Phone {get; set;}
    public string Role {get; set;}
    public string CreatedAt {get; set;}
    public int BirthYear {get; set;}
    public bool Active {get; set;}
    public DateTime CreateDateTime {get; set;}
    public DateTime? UpdateDateTime {get; set;}

    public LegacyUser(int id, int origID, int newUserID, string username, string name, string email, string phone, string role,
                      string createdAt, int birthYear, bool active, DateTime createDateTime, DateTime? updateDateTime = null)
    {
        ID = id;
        OrigID = origID;
        NewUserID = newUserID;
        Username = username;
        Name = name;
        Email = email;
        Phone = phone;
        Role = role;
        CreatedAt = createdAt;
        BirthYear = birthYear;
        Active = active;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static List<LegacyUser> GetAll(DBHelper dBHelper)
    {
        List<LegacyUser> returnValue = [];

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

            LegacyUser newLegacyUser = new(Convert.ToInt32(reader["ID"].ToString()), Convert.ToInt32(reader["OrigID"].ToString()), Convert.ToInt32(reader["NewUserID"].ToString()),
                                           reader["Username"].ToString(), reader["Name"].ToString(), reader["Email"].ToString(), reader["Phone"].ToString(), reader["Role"].ToString(), 
                                           reader["CreatedAt"].ToString(), Convert.ToInt32(reader["BirthYear"].ToString()), Convert.ToInt32(reader["Active"].ToString()) == 1, 
                                           CreateDateTime, UpdateDateTime);

            returnValue.Add(newLegacyUser);
        }

        conn.Close();
        return returnValue;
    }

    public override string ToInsertSQL() => $"INSERT INTO LegacyAccounts (ID, OrigID, Username, Name, Email, Phone, Role, CreatedAt, BirthYear, Active, CreateDateTime, UpdateDateTime, NewUserID) VALUES ({ID}, {OrigID}, {Username}, {Name}, {Email}, {Phone}, {Role}, {CreatedAt}, {BirthYear}, {Active}, '{CreateDateTime:YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime:YYYY-MM-DD hh:mm:ss}', {NewUserID})";
    public static string ToSelectAllSQL() => "SELECT * FROM LegacyAccounts";
    public static string ToDeleteAllSQL() => "DELETE FROM `LegacyAccounts`";
}