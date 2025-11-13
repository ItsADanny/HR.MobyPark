// register hierin

public static class CUser
{
    public static string Register(HttpRequest request)
    {
        string? username = request.Query["Username"];
        string? password = request.Query["Password"];
        string? fullName = request.Query["FullName"];
        string? email = request.Query["Email"];
        string? phoneNumber = request.Query["PhoneNumber"];
        //date van maken
        string? birthYearStr = request.Query["BirthYear"];

        if (!int.TryParse(birthYearStr, out int birthYear))
            return "Invalid birth year";

        // hier de control check
        if (!RegisterCheck(username, password, fullName, email, phoneNumber, birthYear))
            return "Registration failed due to invalid input.";

        // hash het wachtwoord
        string[] hashed_password = Auth.Hash(password);
        string hash = hashed_password[0];
        string salt = hashed_password[1];

        // check of de username al bestaat in de database
        // deze method moet staan in CDatabase
        if (CDatabase.UserExists(username))
        {
            // response code hier (methods)
            return "";
        }

        // sla de gebruiker op in de database
        int id = CDatabase.SaveUser(username, hash, fullName, email, phoneNumber, birthYear);
        CDatabase.SaveUserSalt(id, salt);

        return "Registration successful.";
    }

    public static bool RegisterCheck(string? username, string? password, string? fullName, string? email, string? phoneNumber, int birthYear)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;
        if (password.Length < 8)
            return false;
        if (birthYear < 1900 || birthYear > DateTime.Now.Year)
            return false;

        // eventueel extra validatie, zoals emailformaat
        return true;
    }


    public static string Login(HttpRequest request)
    {
        string? username = request.Query["Username"];
        string? password = request.Query["Password"];

        // Login check, gebruik losse functies voor elke check.
        if (!LoginCheck(username, password))
            return "Missing credentials";

        User user = CDatabase.GetUser(username);
        if (user is null)
            return "User not found";

        if (!user.IsPasswordCorrect(password))
            return "Invalid credentials";

        // we hebben een token nodig (nieuwe method?)
        // Generate a UUID (GUID) and convert it to a string
        // via security (niet aankomen dankje)
        string token = Guid.NewGuid().ToString();

        // token opslaan in database
        SaveUserToken();
        return $"Login succesful. Token: {token}";
    }

    // Hier moeten we testen of de username/password niet leeg/null is
    public static bool LoginCheck(string? username, string? password)
        => !(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password));

    public static void SaveUserToken(string token)
    {
        // token in database zetten;
        return;
    }

    public static string Logout(HttpRequest request)
    {
        string? token = request.Query["Token"];

        //check of er wel iets van een token bestaat
        if (string.IsNullOrWhiteSpace(token)) // or get_session(token)
            return "No session token";

        //check session token (method) (here)
        if(!CDatabase.SessionExists(token))
            return "Session not found or expired";

        // succesful logout (here)
        // remove session token (methode in )
        if (RemoveSessionOnLogout(token))
            return "Logout succesful";
        else
            return "Logout failed";
    }

    public static bool RemoveSessionOnLogout(string token)
    {
        // return CDatabase.RemoveSession(token);
        return true;      
    }
}