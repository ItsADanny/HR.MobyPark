public static class CProfile
{
    // Get
    public static void GetProfile(HttpRequest request)
    {
        // token vragen, dit wordt hengyi headeer
        if (!Auth.IsTokenFilled(request))
        {
            System.Console.WriteLine("Invalid token");
            return;
        }

        // auf mthod, dit doet ook de sessie terug sturen
        Session? session = CSession.GetSession(Database.MainDBConfig, request.Headers["Token"]);

        // anders method maken dat je de user items krijgt
        //  met een query Session UserID gebruiken
        // gelijk readen
        User? user = CDatabase.GetUserByID(session.UserID);

        if (user is null)
        {
            System.Console.WriteLine("User not found");
            return;
        }
        System.Console.WriteLine($"User Profile:\nUsername: {user.Username}\nName: {user.Name}\nEmail: {user.Email}\nPhone: {user.Phone}\nBirth Year: {user.BirthYear}");

    }

    public static void UpdateProfile(HttpRequest request)
    {
        // token vragen 
        if (!Auth.IsTokenFilled(request))
        {
            System.Console.WriteLine("Invalid token");
            return;
        }

        //auf method hier
        Session? session = CSession.GetSession(Database.MainDBConfig, request.Headers["Token"]);

        // CHeck of er een sessie is
        if (session is null)
        {
            System.Console.WriteLine("Session not found or expired");
            return;
        }

        //mogen we wel updaten (session checken)
        // username, password, name, email, phoneNumber, birthdate
        User? user = CDatabase.GetUserByID(session.UserID);
        if (user is null)
        {
            System.Console.WriteLine("User not found");
            return;
        }
        
        // idk of dit goed is, nog ff checken
        // nu updaten
        string? newUserName = request.Query["UserName"];
        string? newEmail = request.Query["Email"];
        string? newName = request.Query["Name"];
        string? newPhone = request.Query["PhoneNumber"];
        string? newPassword = request.Query["Password"];
        string? newBirthYearStr = request.Query["BirthYear"];
        if (!string.IsNullOrWhiteSpace(newUserName))
            user.Username = newUserName;
        if (!string.IsNullOrWhiteSpace(newEmail))
            user.Email = newEmail;
        if (!string.IsNullOrWhiteSpace(newName))
            user.Name = newName;
        if (!string.IsNullOrWhiteSpace(newPhone))
            user.Phone = newPhone;
        //dze die andere method aanroepen
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            // hash het wachtwoord
            string[] hashed_password = Auth.Hash(newPassword);
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
        }
        if (!string.IsNullOrWhiteSpace(newBirthYearStr) && int.TryParse(newBirthYearStr, out int newBirthYear))
            user.BirthYear = newBirthYear;


        System.Console.WriteLine("User Updated succesfully");
    }
}