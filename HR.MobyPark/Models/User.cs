using MySql.Data.MySqlClient;

public class User : aDatabase, iDatabase
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
    public DateTime UpdateDateTime { get; private set;}

    //This method will simply return a true or false when checking the users password,
    //this will make sure that the password is never exposed or to be accessed outside of this class
    public bool IsPasswordCorrect(string guess, string salt) => Auth.Hash(guess, salt)[0] == _password;
    public bool IsPasswordCorrect(string guess) => Auth.Hash(guess, UserSalt.GetSalt(ID).Salt)[0] == _password;
    public bool IsLegacyPassword(string guess) => Auth.LegacyHash(guess) == _password;
    
    public void UpdatePasswordFromLegacy(string password)
    {
        string[] hashedPassword = Auth.Hash(password);

        _password = hashedPassword[0];
        if (new UserSalt(ID, hashedPassword[1]).CreateDB()) UpdateDB();
    }

    public void SetPassword(string password) => _password = password;

    //Static methods
    public static bool DoesEmailExist(string email) => GetUserEmail(email) is not null ? true : false;
    public static bool DoesUsernameExist(string username) => GetUserUsername(username) is not null ? true : false;

    public static User? GetUser(int userID) => _mGetUser($"SELECT * FROM Users WHERE ID = {userID}");
    public static User? GetUserUsername(string accountUsername) => _mGetUser($"SELECT * FROM Users WHERE Username = {accountUsername}");
    public static User? GetUserEmail(string accountEmail) => _mGetUser($"SELECT * FROM Users WHERE Email = {accountEmail}");

    //Private static - Database interactions
    private static User? _mGetUser(string sqlStatement)
    {
        try
        {
            MySqlConnection conn = new MySqlConnection(Environment.GetEnvironmentVariable("dbConString"));
            MySqlCommand cmd = new MySqlCommand();

            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = sqlStatement;

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                User user = new ()
                {
                    ID = Convert.ToInt32(reader["ID"].ToString()),
                    Username = reader["Username"].ToString(),
                    _password = reader["Password"].ToString(),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    BirthDate = DateOnly.FromDateTime(Convert.ToDateTime(reader["BirthDate"].ToString())),
                    Role = UserRole.GetUserRole(Convert.ToInt32(reader["RoleID"].ToString())),
                    Active = GeneralMethods.IntToBool(Convert.ToInt32(reader["Active"].ToString())),
                    CreateDateTime = Convert.ToDateTime(reader["CreateDateTime"].ToString()),
                    UpdateDateTime = Convert.ToDateTime(reader["UpdateDateTime"].ToString())
                };
                conn.Close();
                return user;
            }
            else
            {
                conn.Close();
                return null;
            } 
        }
        catch
        {
            return null;
        }
    }

    //IDatabase implementation
    public override string ToInsertSQL() => $"INSERT INTO Users (Username, Password, Name, Email, Phone, BirthDate, RoleID) VALUES (\"{Username}\", \"{_password}\", \"{Name}\", \"{Email}\", \"{Phone}\", '{BirthDate:yyyy-MM-dd}', 1)";

    public override string ToUpdateSQL() => $"UPDATE Users SET Username = \"{Username}\", Password = \"{_password}\", Name = \"{Name}\", Email = \"{Email}\", Phone = \"{Phone}\", BirthDate = '{BirthDate:yyyy-MM-dd}', RoleID = {Role.ID}, Active = {Active} WHERE ID = {ID}";

    public override string ToDeleteSQL() => $"DELETE FROM Users WHERE ID = {ID}";

    public override string ToSelectSQL() => $"SELECT * FROM Users WHERE ID = {ID}";

    public override bool CreateDB()
    {
        if (!string.IsNullOrWhiteSpace(_password)) {
            string[] hashedPassword = Auth.Hash(_password);
            _password = hashedPassword[0];

            int? returnedID = DBHelper.Create(ToInsertSQL());

            if (returnedID is not null && returnedID != 0)
            {
                if (new UserSalt(ID, hashedPassword[1]).CreateDB()) return true;
            }
        }
        return false;
    }
}