public class NewUser : iSQL
{
    public int ID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateOnly BirthDate { get; set; }

    public string ToDeleteSQL()
    {
        return "";
    }

    public string ToInsertSQL()
    {
        return $"INSERT INTO Users (Username, Password, Name, Email, Phone, BirthDate) VALUES (\"{Username}\", \"{Password}\", \"{Name}\", \"{Email}\", \"{Phone}\", '{BirthDate:yyyy-MM-dd}')";
    }

    public string ToUpdateSQL()
    {
        return "";
    }
}