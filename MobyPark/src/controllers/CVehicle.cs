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
        List<Vehicle> vehicles = GetVehiclesByUserID(session.UserID);
        if (vehicles.Count == 0)
        {
            System.Console.WriteLine("No vehicles found for this user");
            return;
        }
        // voertuigen weergeven
        foreach (Vehicle vehicle in vehicles)
        {
            System.Console.WriteLine($"Vehicle:\nID: {vehicle.ID}\nUserID: {vehicle.UserID}\nLicense Plate: {vehicle.LicensePlate}\nMake: {vehicle.Make}\nModel: {vehicle.Model}\nColor: {vehicle.Color}\nYear: {vehicle.Year}\n");
        }
    }

    // public static void GetVehiclesByUserID(int userID)
    // {
    //     // hoe vraag ik dat hier op
    //     List<Vehicle> vehicles = CDatabase.GetVehiclesByUserID(Database.MainDBConfig, userID);
    //     return vehicles;
    // }

    public static void GetVehicleReservations(HttpRequest request)
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
        List<Reservation> reservations = CDatabase.GetReservationsByUserID(Database.MainDBConfig, session.UserID);
        if (reservations.Count == 0)
        {
            System.Console.WriteLine("No reservations found for this user");
            return;
        }
        // voertuigen weergeven
        foreach (Reservation reservation in reservations)
        {
            System.Console.WriteLine($"Reservation:\nID: {reservation.ID}\nUserID: {reservation.UserID}\nVehicleID: {reservation.VehicleID}\nStartTime: {reservation.StartTime}\nEndTime: {reservation.EndTime}\n");
        }
    }

    public static void GetVehicleHistory(HttpRequest request)
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