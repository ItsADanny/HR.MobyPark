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
        string? amountstr = request.Query["Amount"];

        if (ValidateReservation(parkingLotIdstr, vehicleIdstr, startTimestr, endTimestr, amountstr) == false)
        {
            return "Reservation data invalid due to missing field(s)";
        }

        int parkingLotId = int.Parse(parkingLotIdstr);
        int vehicleId = int.Parse(vehicleIdstr);
        DateTime startTime = DateTime.ParseExact(startTimestr, "dd-MM-yyyy HH:mm", null);
        DateTime endTime = DateTime.ParseExact(endTimestr, "dd-MM-yyyy HH:mm", null);
        decimal Amount = decimal.Parse(amountstr);

        int userId = CDatabaseReservation.GetUserIdFromToken(token);

        int reservationId = CDatabaseReservation.SaveReservation(userId, parkingLotId, vehicleId, startTime, endTime, Amount);

        return $"Reservation created with ID: {reservationId}";



        //Nog toe te voegen:
        //Python: if the session user is an admin, they can specify the user for the reservation.
    }

    
    
    public static bool ValidateReservation(string parkingLotId, string vehicleId, string startTime, string endTime, string Amount)
    {
        if (string.IsNullOrWhiteSpace(parkingLotId) ||
            string.IsNullOrWhiteSpace(vehicleId) ||
            string.IsNullOrWhiteSpace(startTime) ||
            string.IsNullOrWhiteSpace(endTime) ||
            string.IsNullOrWhiteSpace(Amount))
        {
            return false;
        }
        return true;
    }
}