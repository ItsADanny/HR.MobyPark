using Microsoft.AspNetCore.Mvc;
using MUser = User;

[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("{id:int}")]
    public ActionResult<string> GET_Reservation([FromHeader(Name = "authorization")] string authorization, [FromQuery(Name = "id")] int id)
    {
        //Check to see if the authorization has been filled, If it isn't filled. Return Unauthorized error
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        //Try and get the user session, if not return NotFound error
        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        //If the user can't be found return a server error message
        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        //See if there are any reservations matching the given ID
        Reservation? foundReservation = Reservation.GetByID(id);
        if (foundReservation is null) return NotFound($"No reservation found with ID {id}");

        //See if the user has the required premission, if the user does not have the required premission.
        //return a Unauthorized error
        if (!foundUser.Role.GlobalReservations.HasReadPremission() && 
            (!foundUser.Role.Reservation.HasReadPremission() || foundReservation.UserID != foundUser.ID))
            return Unauthorized();

        return Ok(Reservation.ToJSON(foundReservation));
    }

    // ====================================================================================
    // POST
    // ====================================================================================
    [HttpPost("")]
    public ActionResult<string> POST_CreateReservation([FromHeader(Name = "authorization")] string authorization, reservationDto reservationDto)
    {
        //Check to see if the authorization has been filled, If it isn't filled. Return Unauthorized error
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        //Try and get the user session, if not return NotFound error
        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        //If the user can't be found return a server error message
        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        //See if the user has the required premission, if the user does not have the required premission.
        //return a Unauthorized error
        if (!foundUser.Role.GlobalReservations.HasWritePremission() && !foundUser.Role.Reservation.HasWritePremission())
            return Unauthorized();

        Reservation newReservation = new ()
        {
            UserID = reservationDto.userID,
            ParkingLotID = reservationDto.parkingLotID,
            VehicleID = reservationDto.vehicleID,
            StartDateTime = reservationDto.startDateTime,
            EndDateTime = reservationDto.endDateTime
        };

        if (newReservation.CreateDB()) return Ok("Reservation created successfully");
        return Problem("Failed to create reservation, please try again later");
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("{id:int}")]
    public ActionResult<string> PUT_UpdateReservation([FromHeader(Name = "authorization")] string authorization, [FromRoute(Name = "id")] int id, updateReservationDto updateReservationDto)
    {
        //Check to see if the authorization has been filled, If it isn't filled. Return Unauthorized error
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        //Try and get the user session, if not return NotFound error
        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        //If the user can't be found return a server error message
        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        //See if there are any reservations matching the given ID
        Reservation? foundReservation = Reservation.GetByID(id);
        if (foundReservation is null) return NotFound($"No reservation found with ID {id}");

        //See if the user has the required premission, if the user does not have the required premission.
        //return a Unauthorized error
        if (!foundUser.Role.GlobalReservations.HasWritePremission() && 
            (!foundUser.Role.Reservation.HasWritePremission() || foundReservation.UserID != foundUser.ID))
            return Unauthorized();

        foundReservation.StartDateTime = updateReservationDto.startDateTime;
        foundReservation.EndDateTime = updateReservationDto.endDateTime;
        //If the user has global writes for Reservervation, then we will update the status
        if (!foundUser.Role.GlobalReservations.HasWritePremission()) foundReservation.Status = updateReservationDto.status;

        if (foundReservation.UpdateDB()) return Ok("Reservation with ID {reservationId} updates successfully");
        return Problem("Failed to update reservation with ID {reservationId}");
    }

    // ====================================================================================
    // DELETE
    // ====================================================================================
    [HttpDelete("{id:int}")]
    public ActionResult<string> DELETE_Reservation([FromHeader(Name = "authorization")] string authorization, int id)
    {
        //Check to see if the authorization has been filled, If it isn't filled. Return Unauthorized error
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        //Try and get the user session, if not return NotFound error
        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        //If the user can't be found return a server error message
        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        //See if there are any reservations matching the given ID
        Reservation? foundReservation = Reservation.GetByID(id);
        if (foundReservation is null) return NotFound($"No reservation found with ID {id}");

        //See if the user has the required premission, if the user does not have the required premission.
        //return a Unauthorized error
        if (!foundUser.Role.GlobalReservations.HasWritePremission() && 
            (!foundUser.Role.Reservation.HasWritePremission() || foundReservation.UserID != foundUser.ID))
            return Unauthorized();

        if (foundReservation.DeleteDB()) return Ok($"Reservation with ID {id} deleted successfully");
        return Problem($"Failed to delete reservation with ID {id}");
    }
}