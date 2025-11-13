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


        //mogen we wel updaten (session checken)
        // username, password, name, email, phoneNumber, birthdate


        // hashen method hergebruiken?

        System.Console.WriteLine("User Updated succesfully");
    }
}