using MySql.Data.MySqlClient;

public class User : aModel, iDatabase
{
    public int ID { get; set;}
    public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Role { get; set; }
    public DateOnly BirthDate { get; set; }
    public bool Active { get; set; }
    public DateTime CreateDateTime { get; set;}
    public DateTime? UpdateDateTime { get; set;}

    private User(int id, string username, string name, string email, string phone, int roleID, DateOnly birthDate, bool active, DateTime createDateTime, DateTime? updateDateTime=null)
    {
        ID = id;
        Username = username;
        Name = name;
        Email = email;
        Phone = phone;
        Role = roleID;
        BirthDate = birthDate;
        Active = active;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static List<User> GetAll(DBHelper dBHelper)
    {
        List<User> returnValue = [];

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

            User newUser = new (Convert.ToInt32(reader["ID"].ToString()), reader["Username"].ToString(),
                                 reader["Name"].ToString(), reader["Email"].ToString(), reader["Phone"].ToString(),
                                 Convert.ToInt32(reader["RoleID"].ToString()), DateOnly.FromDateTime(Convert.ToDateTime(reader["BirthDate"].ToString())),
                                 GeneralMethods.IntToBool(Convert.ToInt32(reader["Active"].ToString())), CreateDateTime,
                                 UpdateDateTime
            );

            returnValue.Add(newUser);
        }

        conn.Close();
        return returnValue;
    }

    //IDatabase implementation
    public override string ToInsertSQL() => $"INSERT INTO Users (ID, Username, Name, Email, Phone, BirthDate, RoleID, CreateDateTime, UpdateDateTime) VALUES ({ID}, \"{Username}\", \"{Name}\", \"{Email}\", \"{Phone}\", '{BirthDate:yyyy-MM-dd}', {Role}, '{CreateDateTime::YYYY-MM-DD hh:mm:ss}', '{UpdateDateTime::YYYY-MM-DD hh:mm:ss}')";
    public static string ToSelectAllSQL() => "SELECT * FROM Users";
    public static string ToDeleteAllSQL() => "DELETE FROM `Users`";
}