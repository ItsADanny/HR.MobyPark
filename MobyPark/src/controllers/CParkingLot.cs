public static class CParkingLot
{

    // Get all
    public static List<ParkingLot> GetAllParkingLots()
    {
        string token = request.headers.get["Authorization"];
        //fetch and return all parking lots
    }

    //Get by id
    public static object GetParkingLotById(int id)
    {
        string token = request.headers.get["Authorization"];

        string validationError = ValidateId(id);
        if (validationError != null)
        {
            return new { error = validationError };
        }

        //check if parking lot with the given id exists else return error message

        return new { error = $"Parking lot with ID {id} not found." };
    }

    //Delete
    public static bool DeleteParkingLot(int id)
    {
        string token = request.headers.get["Authorization"];
        //call a delete method and handle failed delete logic

        Log.WriteLine($"Parking lot with ID: {id} successfully deleted.");
        return true;
    }

    //Post
    public static int CreateParkingLot(string name, string location, string adress, int capacity, double tariff, double dayTariff)
    {
        string token = request.headers.get["Authorization"];
        //call a helper method to insert parking lot into database and return the id
        if (id == -1)
        {
            Log.WriteErrorLine(null, $"Could not create parking lot '{name}'");
        }
        return id;
    }


    //Validation
    private static string ValidateId(int id)
    {
        if (id <= 0)
        {
            return "Invalid ID. Must be a positive number.";
        }
        return null;
    }
}
