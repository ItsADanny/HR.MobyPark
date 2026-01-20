public class User : iSQL
{
    // Sample JSON data
    // ========================================================
    // {
    //     "id":"1",
    //     "username":"cindy.leenders42",
    //     "password":"6b37d1ec969838d29cb611deaff50a6b",
    //     "name":"Cindy Leenders",
    //     "email":"cindyleenders@upcmail.nl",
    //     "phone":"+310792215694",
    //     "role":"USER",
    //     "created_at":"2017-10-06",
    //     "birth_year":1937,
    //     "active":true
    // }

    public string id { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string role { get; set; }
    public string created_at { get; set; }
    public int birth_year { get; set; }
    public bool active { get; set; }
    public int? NewUserID { get; set; }

    public string ToDeleteSQL()
    {
        throw new NotImplementedException();
    }

    public string ToInsertSQL()
    {
        return $"INSERT INTO LegacyAccounts (OrigID, Username, Password, Name, Email, Phone, Role, CreatedAt, BirthYear, Active) VALUES ({id}, \"{username}\", \"{password}\", \"{name}\", \"{email}\", \"{phone}\", \"{role}\", \"{created_at}\", {birth_year}, {active});";
    }

    public string ToUpdateSQL()
    {
        return $"UPDATE LegacyAccounts SET NewUserID = {NewUserID} WHERE ID = {id};";
    }
}