public static class CParkingLot
{
    // Get all
    public static List<ParkingLot> GetAllParkingLots(string token)
    {
        ValidateToken(token);
        return CDatabaseParkingLot.GetAll();
    }

    // Get by id
    public static ParkingLot GetParkingLotById(int id, string token)
    {
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
    public static object DeleteParkingLot(int id, string token)
    {
        ValidateToken(token);

        string validationError = ValidateId(id);
        if (validationError != null)
            return new { error = validationError };

        bool success = CDatabaseParkingLot.Delete(id);
        if (!success)
            return new { error = $"Failed to delete parking lot with ID {id}." };

        Log.WriteLine($"Parking lot with ID: {id} successfully deleted.");
        return new { success = true };
    }

    // Post
    public static object CreateParkingLot(string token, string name, string location, string address, int capacity, double tariff, double dayTariff)
    {
        ValidateToken(token);

        int id = CDatabaseParkingLot.Insert(name, location, address, capacity, tariff, dayTariff);
        if (id == -1)
        {
            Log.WriteErrorLine(null, $"Could not create parking lot '{name}'");
            return new { error = "Creation failed" };
        }

        return new { id = id };
    }

    // Validation
    private static string ValidateId(int id) =>
        id <= 0 ? "Invalid ID. It must be a positive number." : null;

    private static void ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("Missing or invalid Authorization token.");
    }
}
