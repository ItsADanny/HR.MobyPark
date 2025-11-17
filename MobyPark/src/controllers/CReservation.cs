using System.Data;
using System.Net.Cache;

public static class CReservation
{

    //POST
    public static string CreateReservation(HttpRequest request, string token)
    {
        // check of er wel iets van een token bestaat, deze method moet nog in CDatabase komen
        if (!ValidateToken(token))
        {
            return "Invalid session token";
        }
        
        string? userIdstr = request.Query["UserId"];
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

        // int userId = CDatabaseReservation.GetUserIdFromToken(token);
        int userId = int.Parse(userIdstr!);

        bool saveSatatus = CDatabaseReservation.SaveReservation(userId, parkingLotId, vehicleId, startTime, endTime, Amount);

        if (saveSatatus)
        {
            return $"Reservation created successfully";
        }
        return $"Failed to create reservation";



        //Nog toe te voegen:
        //Python: if the session user is an admin, they can specify the user for the reservation.
    }

    //PUT

    public static string UpdateReservation(HttpRequest request, string token)
    {
        // if (string.IsNullOrWhiteSpace(token) || !CDatabase.SessionExists(token))
        if (!ValidateToken(token))
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
    
    //GET

    public static string GetReservation(HttpRequest request, string token)
    {

        if(!ValidateToken(token))
        {
            return "Invalid session token";
        }
        string? reservationIdStr = request.Query["Id"];
        string? userId = request.Query["UserId"];
        
        //hier moet nog een token check komen
        if (string.IsNullOrWhiteSpace(reservationIdStr))
        {
            return "Reservation ID is missing";
        }


        //deze onderste methods moeten nog bestaan. Het checks of de user admin is of de eigenaar van de reservation
        if(!CReservation.CheckIfAdmin(int.Parse(userId)) || !CReservation.CheckIfOwner(int.Parse(userId), int.Parse(reservationIdStr)))
        {
            return "User is not authorized to view this reservation";
        }


        Reservation pulledReservation = CDatabaseReservation.GetReservationById(int.Parse(reservationIdStr));
        if (pulledReservation == null)
        {
            return $"No reservation found with ID {reservationIdStr}";
        }
        
        return ".";
    }

    //DELETE
    public static string DeleteReservation(HttpRequest request, string token)
    {
        if(!ValidateToken(token))
        {
            return "Invalid session token";
        }

        string? reservationIdStr = request.Query["Id"];
        if (string.IsNullOrWhiteSpace(reservationIdStr))
        {
            return "Reservation ID is missing";
        }
        if (!CDatabaseReservation.ReservationExists(int.Parse(reservationIdStr)))
        {
            return $"Reservation with ID {reservationIdStr} does not exist";
        }
        bool isDeleted = CDatabaseReservation.DeleteReservation(int.Parse(reservationIdStr));
        if(isDeleted)
        {
            return $"Reservation with ID {reservationIdStr} deleted successfully";
        }
        else
        {
            return $"Failed to delete reservation with ID {reservationIdStr}";
        }
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

    public static bool ValidateToken(string token) //hier moeten meer checks komen
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }
        return true;
    }


}

