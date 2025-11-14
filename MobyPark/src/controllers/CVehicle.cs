public static class CVehicle
{
    // Get vehicle for the logged-in user
    public static void GetVehicles(HttpRequest request)
    {
        // token vragen 
        if (!Auth.IsTokenFilled(request))
        {
            System.Console.WriteLine("Invalid token");
            return;
        }

        //auf method hier
        Session? session = CSession.GetSession(Database.MainDBConfig, request.Headers["Token"]);

        // Check of er een sessie is
        if (session is null)
        {
            System.Console.WriteLine("Session not found or expired");
            return;
        }

        // voertuigen ophalen
    }    


    // POST vehicle for the logged-in user
    public static void PostVehicle(HttpRequest request)
    {
        // token vragen 
        if (!Auth.IsTokenFilled(request))
        {
            System.Console.WriteLine("Invalid token");
            return;
        }

        //auf method hier
        Session? session = CSession.GetSession(Database.MainDBConfig, request.Headers["Token"]);

        // Check of er een sessie is
        if (session is null)
        {
            System.Console.WriteLine("Session not found or expired");
            return;
        }

        // voertuig toevoegen aan database
        Vehicle? vehicle = CDatabase.AddVehicle(Database.MainDBConfig, session.UserID, request);
        if (vehicle is null)
        {
            System.Console.WriteLine("Failed to add vehicle");
            return;
        }
        // check if the vehicle already exists with the license plate
        if (VehicleExists(session.UserID, vehicle.LicensePlate))
        {
            System.Console.WriteLine("Vehicle with this license plate already exists");
            return;
        }
        

        System.Console.WriteLine("Vehicle added successfully");
    }
}