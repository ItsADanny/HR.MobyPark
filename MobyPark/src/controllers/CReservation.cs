using System.Data;
using System.Net.Cache;

public static class CReservation
{
    public static string CreateReservation(HttpRequest request)
    {
        string? token = request.Query["Token"];
        // check of er wel iets van een token bestaat, deze method moet nog in CDatabase komen
        if (string.IsNullOrWhiteSpace(token) || !CDatabase.SessionExists(token))
            return "No session token";

        string? parkingLotIdstr = request.Query["ParkingLotId"];
        string? vehicleIdstr = request.Query["VehicleId"];
        string? startTimestr = request.Query["StartTime"];
        string? endTimestr = request.Query["EndTime"];
        string? coststr = request.Query["Cost"];

        if (ValidateReservation(parkingLotIdstr, vehicleIdstr, startTimestr, endTimestr, coststr) == false)
        {
            return "Reservation data invalid due to missing field(s)";
        }

        int parkingLotId = int.Parse(parkingLotIdstr);
        int vehicleId = int.Parse(vehicleIdstr);
        DateTime startTime = DateTime.Parse(startTimestr);
        DateTime endTime = DateTime.Parse(endTimestr);
        decimal cost = decimal.Parse(coststr);

        int userId = CDatabaseReservation.GetUserIdFromToken(token);

        int reservationId = CDatabaseReservation.SaveReservation(userId, parkingLotId, vehicleId, startTime, endTime, cost);

        return $"Reservation created with ID: {reservationId}";

    }

    
    
    public static bool ValidateReservation(string parkingLotId, string vehicleId, string startTime, string endTime, string cost)
    {
        if (string.IsNullOrWhiteSpace(parkingLotId) ||
            string.IsNullOrWhiteSpace(vehicleId) ||
            string.IsNullOrWhiteSpace(startTime) ||
            string.IsNullOrWhiteSpace(endTime) ||
            string.IsNullOrWhiteSpace(cost))
        {
            return false;
        }
        return true;
    }
}