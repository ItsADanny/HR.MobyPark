public static class CProfile
{
    // Get
    public static void GetProfile(HttpRequest request)
    {
        // token vragen, dit wordt hengyi headeer
        string token = request.Query["Token"];

        // token if statements / methods (hier)
        if (token.IsNullOrWhiteSpace())
        {
            System.Console.WriteLine("Invalid token");
            return;
        }

        // auf mthod, dit doet ook de sessie terug sturen

        // anders method maken dat je de user items krijgt
        //  met een query Session UserID gebruiken
        // gelijk readen

        
    }

    public static void UpdateProfile(HttpRequest request)
    {
        // token vragen 
        string token = request.Query["Token"];

        // token if statements / methods (hier)
        if (string.IsNullOrWhiteSpace(token))
        {
            System.Console.WriteLine("Invalid token");
            return;
        }

        //auf method hier


        //mogen we wel updaten (session checken)
        // username, password, name, email, phoneNumber, birthdate


        // hashen method hergebruiken?

        System.Console.WriteLine("User Updated succesfully");
    }
}