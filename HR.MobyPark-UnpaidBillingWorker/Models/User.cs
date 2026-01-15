using MySql.Data.MySqlClient;

public class User : aModel, iDatabase
{
    //Original fields on which this class is based

    //{"id": "1", 
    // "username": "cindy.leenders42", 
    // "password": "6b37d1ec969838d29cb611deaff50a6b", 
    // "name": "Cindy Leenders", 
    // "email": "cindyleenders@upcmail.nl", 
    // "phone": "+310792215694", 
    // "role": "USER", 
    // "created_at": "2017-10-06", 
    // "birth_year": 1937, 
    // "active": true

    public int ID { get; private set;}
    public string Username { get; set; }
    public string _password { private get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public UserRole Role { get; set; }
    public DateOnly BirthDate { get; set; }
    public bool Active { get; set; }
    public DateTime CreateDateTime { get; private set;}
    public DateTime? UpdateDateTime { get; private set;}

    public User(string username, string name, string email, string phonenumber, DateOnly birthDate)
    {
        Username = username;
        _password = "";
        Name = name;
        Email = email;
        Phone = phonenumber;
        BirthDate = birthDate;
    }

    private User(int id, string username, string password, string name, string email, string phone, int roleID, DateOnly birthDate, bool active, DateTime createDateTime, DateTime? updateDateTime=null)
    {
        ID = id;
        Username = username;
        _password = password;
        Name = name;
        Email = email;
        Phone = phone;
        Role = UserRole.GetByID(roleID);
        BirthDate = birthDate;
        Active = active;
        CreateDateTime = createDateTime;
        UpdateDateTime = updateDateTime;
    }

    public static User? GetByID(int id)
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

            User returnValue = new (Convert.ToInt32(reader["ID"].ToString()), reader["Username"].ToString(), reader["Password"].ToString(),
                                 reader["Name"].ToString(), reader["Email"].ToString(), reader["Phone"].ToString(),
                                 Convert.ToInt32(reader["RoleID"].ToString()), DateOnly.FromDateTime(Convert.ToDateTime(reader["BirthDate"].ToString())),
                                 GeneralMethods.IntToBool(Convert.ToInt32(reader["Active"].ToString())), CreateDateTime,
                                 UpdateDateTime
            );

            conn.Close();
            return returnValue;
        }

        conn.Close();
        return null;
    }

    public static List<User> GetAll()
    {
        List<User> returnValue = [];

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

            User newUser = new (Convert.ToInt32(reader["ID"].ToString()), reader["Username"].ToString(), reader["Password"].ToString(),
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

    //Static methods
    public static User? GetUser(int userID) => _mGetUser($"SELECT * FROM Users WHERE ID = {userID}");
    public static User? GetUserUsername(string accountUsername) => _mGetUser($"SELECT * FROM Users WHERE Username = \"{accountUsername}\"");
    public static User? GetUserEmail(string accountEmail) => _mGetUser($"SELECT * FROM Users WHERE Email = \"{accountEmail}\"");

    //Private static - Database interactions
    private static User? _mGetUser(string sqlStatement)
    {
        try
        {
            MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sqlStatement;

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                DateTime CreateDateTime = DateTime.Now;
                DateTime? UpdateDateTime = null;

                string? CreateDateTime_STR = reader["CreateDateTime"].ToString();
                string? UpdateDateTime_STR = reader["UpdateDateTime"].ToString();

                if (CreateDateTime_STR is not null && CreateDateTime_STR != "") CreateDateTime = GeneralMethods.ParseDBDateTime(CreateDateTime_STR);
                if (UpdateDateTime_STR is not null && UpdateDateTime_STR != "") UpdateDateTime = GeneralMethods.ParseDBDateTime(UpdateDateTime_STR);

                User user = new (Convert.ToInt32(reader["ID"].ToString()), reader["Username"].ToString(), reader["Password"].ToString(),
                                 reader["Name"].ToString(), reader["Email"].ToString(), reader["Phone"].ToString(),
                                 Convert.ToInt32(reader["RoleID"].ToString()), DateOnly.FromDateTime(Convert.ToDateTime(reader["BirthDate"].ToString())),
                                 GeneralMethods.IntToBool(Convert.ToInt32(reader["Active"].ToString())), CreateDateTime,
                                 UpdateDateTime
                );

                conn.Close();
                return user;
            }
            else
            {
                conn.Close();
                return null;
            } 
        }
        catch (Exception e)
        {
            throw new Exception(e.ToString());
            return null;
        }
    }

    //IDatabase implementation
    public override string ToInsertSQL() => $"INSERT INTO Users (Username, Password, Name, Email, Phone, BirthDate, RoleID) VALUES (\"{Username}\", \"{_password}\", \"{Name}\", \"{Email}\", \"{Phone}\", '{BirthDate:yyyy-MM-dd}', 1)";

    public override string ToUpdateSQL() => $"UPDATE Users SET Username = \"{Username}\", Password = \"{_password}\", Name = \"{Name}\", Email = \"{Email}\", Phone = \"{Phone}\", BirthDate = '{BirthDate:yyyy-MM-dd}', RoleID = {Role.ID}, Active = {Active} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Users WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Users WHERE ID = {ID}";

    public override bool CreateDB() { return true; }

    public bool CreateDB(string password) => false;

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM Users WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM Users";
}