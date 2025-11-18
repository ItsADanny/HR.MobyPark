using System.Data;
using System.Net.Cache;
using System.Reflection.Metadata.Ecma335;

public static class CReservation
{

    //POST
    public static string CreateReservation(HttpRequest request)
    {

        string ?token = request.Headers["Authorization"].FirstOrDefault();
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

    public static string UpdateReservation(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault()!;
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

        //hier moet eigenlijk nog komen WAT er gewijzigt moet worden. 
        //Nu kan alles geupdate worden, maar voor user en admin moet alleen starttime endtime en status.

        // string? parkingLotIdstr = request.Query["ParkingLotId"];
        // string? vehicleIdstr = request.Query["VehicleId"];
        string? startTimestr = request.Query["StartTime"];
        string? endTimestr = request.Query["EndTime"];
        // string? amountstr = request.Query["Amount"];
        string? status = request.Query["Status"];

        if (ValidateReservation(startTimestr, endTimestr, status) == false)
        {
            return "Reservation data invalid due to missing field(s)";
        }



        int reservationId = int.Parse(reservationIdstr);
        // int parkingLotId = int.Parse(parkingLotIdstr);
        // int vehicleId = int.Parse(vehicleIdstr);
        DateTime startTime = DateTime.ParseExact(startTimestr, "dd-MM-yyyy HH:mm", null);
        DateTime endTime = DateTime.ParseExact(endTimestr, "dd-MM-yyyy HH:mm", null);
        // decimal Amount = decimal.Parse(amountstr);
        // int userId = CDatabaseReservation.GetUserIdFromToken(token);

        if(IsUserOwner(CDatabase.GetUserIdFromToken(token), reservationId) == false && !CReservation.CheckIfAdminWithToken(token))
        {
            return "User is not authorized to update this reservation";
        }


        bool reservationUpdated = CDatabaseReservation.UpdateReservation(reservationId, startTime, endTime, status);



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

    public static string GetReservation(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault()!;
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
    public static string DeleteReservation(HttpRequest request)
    {
        string token = request.Headers["Authorization"].FirstOrDefault()!;
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


        int userId = CDatabase.GetUserIdFromToken(token);

        if(!CReservation.CheckIfAdmin(userId) && !CReservation.CheckIfOwner(userId, int.Parse(reservationIdStr)))
        {
            return "User is not authorized to delete this reservation";
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
    public static bool ValidateReservation(string startTime, string endTime, string Status)
    {
        if (
            // string.IsNullOrWhiteSpace(parkingLotId) ||
            // string.IsNullOrWhiteSpace(vehicleId) ||
            string.IsNullOrWhiteSpace(startTime) ||
            string.IsNullOrWhiteSpace(endTime) ||
            // string.IsNullOrWhiteSpace(Amount))
            string.IsNullOrWhiteSpace(Status))
        {
            return false;
        }
        return true;
    }

    public static bool ValidateReservation(string parkingLotIdstr, string vehicleIdstr, string startTimestr, string endTimestr, string amountstr)
    {
        if (
            string.IsNullOrWhiteSpace(parkingLotIdstr) ||
            string.IsNullOrWhiteSpace(vehicleIdstr) ||
            string.IsNullOrWhiteSpace(startTimestr) ||
            string.IsNullOrWhiteSpace(endTimestr) ||
            string.IsNullOrWhiteSpace(amountstr))
        {
            return false;
        }
        return true;
    }

    public static bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || Auth.GetUserSession(token) == null)
        {
            return false;
        }
        return true;
    }

    // public int GetUserIdFromToken(string token)
    // {
    //     UserSession userSession = Auth.GetUserSession(token);
    //     if (userSession != null)
    //     {
    //         return userSession.UserId;
    //     }
    //     return -1; 
    // }
    
    public bool IsUserOwner(int userId, int reservationId)
    {
        Reservation reservation = CDatabaseReservation.GetReservationById(reservationId);
        if(reservation != null && reservation.UserId == userId)
        {
            return true;
        }
        return false;
    }
    public static bool CheckIfAdminWithToken(string token)
    {
        UserSession userSession = Auth.GetUserSession(token);
        if (userSession == null)
        {
            return false;
        }

        int userId = userSession(token);

        if(CDatabase.IsUserAdmin(userId))
        {
            return true;
        }
        return false;

    }



}

