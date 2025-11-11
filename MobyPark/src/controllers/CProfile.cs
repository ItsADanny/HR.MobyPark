public static class CProfile
{
    // Get
    public static void GetProfile(HttpRequest request)
    {
        // token vragen
        string token = request.Query["Token"];
        
        // token if statements / methods (hier)


        // maybe GetUser method gewoon gebruiken?
        // anders method maken dat je de user items krijgt
    }

    public static void UpdateProfile(HttpRequest request)
    {
        // token vragen 
        string token = request.Query["Token"];

        // token if statements / methods (hier)


        // miss GetUser weer hier?
        // wachtwoord kunnen aanpassen + 
        // hashen method hergebruiken?
        System.Console.WriteLine("User Updated succesfully");
    }
}