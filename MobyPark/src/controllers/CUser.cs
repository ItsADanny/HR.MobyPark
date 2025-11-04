// register hierin

public static class CUser()
{
    public static string Register(HttpRequest request)
    {
        string? Username = request.Query["Username"];
        string? Password = request.Query["Password"];
        string? FullName = request.Query["FullName"];
        string? Email = request.Query["Email"];
        string? PhoneNumber = request.Query["PhoneNumber"];
        int? BirthYear = request.Query["BirthYear"];

        // hier de control check
        if (!RegisterCheck())
        {
            return "Registration failed due to invalid input.";
        }

        // hash het wachtwoord
        string[] hashed_password = Auth.Hash(password);
        string hash = hashed_password[0];
        string salt = hashed_password[1];

        // check of de username al bestaat in de database
        // deze method moet staan in CDatabase
        if (CDatabase.UserExists(Username))
        {
            // response code hier (methods)
            return;
        }

        // sla de gebruiker op in de database
        int id = CDatabase.SaveUser(Username, hash, FullName, Email, PhoneNumber);
        CDatabase.SaveUserSalt(id, salt);

        return "Registration successful.";
    }

    public static bool RegisterCheck()
    {
        // hier de test scenario's
        return true;
    }


    public static string Login(HttpRequest request)
    {
        string? Username = request.Query["Username"];
        string? Password = request.Query["Password"];

        // Login check, gebruik losse functies voor elke check.
        if (LoginCheck(Username, password))
        {
            System.Console.WriteLine("Missing creds");
            return;
        }

        CDatabase.getUser(username);
        User.IsPasswordCorrect(Password);
    }

    public static bool LoginCheck(username, password)
    {
        if ((username == "" || username == null) && (password == "" || password == null))
        {
            return false;
        }
        return true;
    }
}