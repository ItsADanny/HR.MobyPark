using System.Data;
using System.Net.Cache;

public static class CReservation
{
    public static string CreateReservation(HttpRequest request)
    {
        string? token = request.Query["Token"];
        // check of er wel iets van een token bestaat, deze method moet nog in CDatabase komen
        // if (string.IsNullOrWhiteSpace(token) || !CDatabase.SessionExists(token))
        if (string.IsNullOrWhiteSpace(token) || CDatabase.SessionExists(token) == false)
            return "Invalid session token";

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

        bool saveSatatus = CDatabaseReservation.SaveReservation(userId, parkingLotId, vehicleId, startTime, endTime, Amount);

        if (saveSatatus)
        {
            return $"Reservation created successfully";
        }
        return $"Failed to create reservation";



        //Nog toe te voegen:
        //Python: if the session user is an admin, they can specify the user for the reservation.
    }

    public static string UpdateReservation(HttpRequest request)
    {
        string? token = request.Query["Token"];
        // if (string.IsNullOrWhiteSpace(token) || !CDatabase.SessionExists(token))
        if (string.IsNullOrWhiteSpace(token) || CDatabase.SessionExists(token) == false)
        {
            return "Invalid Session Token";
        }

        string? reservationIdstr = request.Query["ReservationId"];
        if (string.IsNullOrWhiteSpace(reservationIdstr))
        {
            return "Reservation ID missing";
        }

        if (!CDatabaseReservation.ReservationExists(int.Parse(reservationIdstr)))
        {
            return $"Reservation with ID {reservationIdstr} does not exist";
        }




        string? parkingLotIdstr = request.Query["ParkingLotId"];
        string? vehicleIdstr = request.Query["VehicleId"];
        string? startTimestr = request.Query["StartTime"];
        string? endTimestr = request.Query["EndTime"];
        string? amountstr = request.Query["Amount"];

        if (ValidateReservation(parkingLotIdstr, vehicleIdstr, startTimestr, endTimestr, amountstr) == false)
        {
            return "Reservation data invalid due to missing field(s)";
        }



        int reservationId = int.Parse(reservationIdstr);
        int parkingLotId = int.Parse(parkingLotIdstr);
        int vehicleId = int.Parse(vehicleIdstr);
        DateTime startTime = DateTime.ParseExact(startTimestr, "dd-MM-yyyy HH:mm", null);
        DateTime endTime = DateTime.ParseExact(endTimestr, "dd-MM-yyyy HH:mm", null);
        decimal Amount = decimal.Parse(amountstr);

        // int userId = CDatabaseReservation.GetUserIdFromToken(token);

        bool reservationUpdated = CDatabaseReservation.UpdateReservation(reservationId, parkingLotId, vehicleId, startTime, endTime, Amount);


        if (reservationUpdated)
        {
            return $"Reservation with ID {reservationId} updates successfully";
        }
        else
        {
            return $"Failed to update reservation with ID {reservationId}";
        }
    }

    public static string GetReservation(HttpRequest request)
    {
        string? reservationIdStr = request.Query["Id"];
        //hier moet nog een token check komen later
        if (string.IsNullOrWhiteSpace(reservationIdStr))
        {
            return "Reservation ID is missing";
        }
        //hier moet nog admin check komen dat het of een admin is, of de eigenaar van de reservation

        Reservation pulledReservation = CDatabaseReservation.GetReservationById(int.Parse(reservationIdStr));
        if (pulledReservation == null)
        {
            return $"No reservation found with ID {reservationIdStr}";
        }
        
        return ".";
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

