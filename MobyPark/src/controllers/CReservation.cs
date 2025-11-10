using System.Net.Cache;

public static class CReservation
{
    public static string CreateReservation(HttpRequest request)
    {
        string? token = request.Query["Token"];
        // check of er wel iets van een token bestaat, deze method moet nog in CDatabase komen
        if (string.IsNullOrWhiteSpace(token) || !CDatabase.SessionExists(token))
            return "No session token";

        string? parkingLotId = request.Query["ParkingLotId"];
        string? vehicleId = request.Query["VehicleId"];
        string? startTime = request.Query["StartTime"];
        string? endTime = request.Query["EndTime"];

        if (ValidateReservation(parkingLotId, vehicleId, startTime, endTime) == false)
        {
            return "Reservation data invalid due to missing field(s)";
        }
        
        

    }

    
    
    public static bool ValidateReservation(string parkingLotId, string vehicleId, string startTime, string endTime)
    {
        if (string.IsNullOrWhiteSpace(parkingLotId) ||
            string.IsNullOrWhiteSpace(vehicleId) ||
            string.IsNullOrWhiteSpace(startTime) ||
            string.IsNullOrWhiteSpace(endTime))
        {
            return false;
        }
        return true;
    }
}