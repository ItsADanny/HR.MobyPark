using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

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

    public static JObject ToJSON(User user)
    {
        JObject returnValue = new()
        {
            new JProperty("id", user.ID),
            new JProperty("username", user.Username),
            new JProperty("name", user.Name),
            new JProperty("email", user.Email),
            new JProperty("phone", user.Phone),
            new JProperty("birthdate", user.BirthDate.ToString()),
            new JProperty("roleID", user.Role.ID),
            new JProperty("roleName", user.Role.Name),
            new JProperty("active", user.Active),
            new JProperty("createDateTime", user.CreateDateTime),
            new JProperty("updateDateTime", user.UpdateDateTime)
        };

        return returnValue;
    }

    public static JArray ToJSON(List<User> users)
    {
        JArray returnValue = new();

        foreach (User user in users)
        {
            returnValue.Add(ToJSON(user));
        }

        return returnValue;
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

    public void SetPassword(string password)
    {
        //Check to see if there is already a salt in the system for this user
        //When a new account is created then their will not be a salt.
        //EASY METHOD FOR SEEING IF WE ARE GOING TO UPDATE A PASSWORD
        UserSalt? salt = UserSalt.GetSalt(ID);
        if (salt is not null)
        {
            //Check to see if the update password isn't the same as the old password.
            if (_password != Auth.Hash(password, salt.Salt)[0]) {
                //If its not the same. update it.

                //Create a new salt and hash the password
                string[] hashedPasswordWithSalt = Auth.Hash(password);

                //Update the new usersalt with the new salt
                salt.Salt = hashedPasswordWithSalt[1];
                salt.UpdateDB();

                //Finally update the password
                _password = hashedPasswordWithSalt[0];
            }
        }
        else
        {
            _password = password;
        }
    }
    // => _password = password;

    //Static methods
    public static bool DoesEmailExist(string email) => GetUserEmail(email) is not null ? true : false;
    public static bool DoesUsernameExist(string username) => GetUserUsername(username) is not null ? true : false;

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

    public static string ToSelectByIDSQL(int id) => $"SELECT * FROM Users WHERE ID = {id}";

    public static string ToSelectAllSQL() => "SELECT * FROM Users";

    public override string ToString()
    {
        return ToJSON(this).ToString();
    }
}