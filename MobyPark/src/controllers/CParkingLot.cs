public static class CParkingLot
{
    // Get all
    public static List<ParkingLot> GetAllParkingLots(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();
        ValidateToken(token);
        return CDatabaseParkingLot.GetAll();
    }

    // Get by id
    public static ParkingLot GetParkingLotById(int id, HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();
        ValidateToken(token);

        string validationError = ValidateId(id);
        if (validationError != null)
            return new { error = validationError };

        ParkingLot lot = CDatabaseParkingLot.GetById(id);
        if (lot == null)
            return new { error = $"Parking lot with ID {id} not found." };

        return lot;
    }

    // Delete
    public static object DeleteParkingLot(int id, HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();
        UserSession session = Auth.GetUserSession(db, token);//db is a database object
        ValidateToken(token);

        if(CDatabase.IsUserAdmin(session.UserID))
        {
            string validationError = ValidateId(id);
            if (validationError != null)
                return new { error = validationError };

            bool success = CDatabaseParkingLot.Delete(id);
            if (!success)
                return new { error = $"Failed to delete parking lot with ID {id}." };

            Log.WriteLine($"Parking lot with ID: {id} successfully deleted.");
            return new { success = true };
        }
        return null;
    }

    // Post
    public static object CreateParkingLot(HttpRequest request, string name, string location, string address, int capacity, decimal tariff, decimal dayTariff, double lat, double lon)
    {
        string token = request.Headers["Authorization"].FirstOrDefault();
        UserSession session = Auth.GetUserSession(db, token);
        ValidateToken(token);

        if(CDatabase.IsUserAdmin(session.UserID))
        {           
            int id = CDatabaseParkingLot.Insert(name, location, address, capacity, tariff, dayTariff, lat, lon);
            if (id == -1)
            {
                Log.WriteErrorLine(null, $"Could not create parking lot '{name}'");
                return new { error = "Creation failed" };
            }

            return new { id = id };
        }
        return null;
    }

    // Validation
    private static string ValidateId(int id) =>
        id <= 0 ? "Invalid ID. It must be a positive number." : null;

    private static void ValidateToken(HttpRequest request)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("Missing or invalid Authorization token.");
    }
}
