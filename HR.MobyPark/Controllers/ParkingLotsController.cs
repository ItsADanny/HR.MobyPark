using Microsoft.AspNetCore.Mvc;
using MUser = User;

[ApiController]
[Route("[controller]")]
public class ParkingLotsController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("")]
    public ActionResult<string> GET_AllParkingLots([FromHeader(Name = "authorization")] string authorization)
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
        if (!foundUser.Role.ParkingLots.HasReadPremission() || !foundUser.Role.GlobalReservations.HasReadPremission()) return Unauthorized();

        return Ok(ParkingLot.ToJSON(ParkingLot.GetAll()));
    }

    [HttpGet("{id:int}")]
    public ActionResult<string> GET_ParkingLot([FromHeader(Name = "authorization")] string authorization, int id)
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
        if (!foundUser.Role.ParkingLots.HasReadPremission() || !foundUser.Role.GlobalReservations.HasReadPremission()) return Unauthorized();

        ParkingLot? foundParkingLot = ParkingLot.GetByID(id);
        if (foundParkingLot is null) return NotFound($"No parkinglot found matching ID: {id}");

        return Ok(ParkingLot.ToJSON(foundParkingLot));
    }

    [HttpGet("{id:int}/sessions")]
    public ActionResult<string> GET_ParkingLotSessions([FromHeader(Name = "authorization")] string authorization, int id)
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
        if (!foundUser.Role.GlobalReservations.HasReadPremission()) return Unauthorized();

        return Ok(Session.ToJSON(Session.GetAllByParkingLotID(id)));
    }

    // ====================================================================================
    // POST
    // ====================================================================================
    [HttpPost("")]
    public ActionResult<string> POST_CreateParkingLot([FromHeader(Name = "authorization")] string authorization, parkingLotDto parkingLotDto)
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
        if (!foundUser.Role.GlobalParkingLots.HasWritePremission()) return Unauthorized();

        ParkingLot newParkingLot = new()
        {
            Name = parkingLotDto.name,
            Location = parkingLotDto.location,
            Address = parkingLotDto.address,
            Capacity = parkingLotDto.capacity,
            Tariff = parkingLotDto.tariff,
            DayTariff = parkingLotDto.dayTariff,
            Latitude = parkingLotDto.lat,
            Longitude = parkingLotDto.lon
        };

        if (newParkingLot.CreateDB()) return Created();
        return Problem("There was an problem while trying to create a new parkinglot, please try again later");
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("{id:int}")]
    public ActionResult<string> PUT_UpdateParkingLot([FromHeader(Name = "authorization")] string authorization, int id)
    {
        return Problem("NOT IMPLEMENTED");
    }

    // ====================================================================================
    // DELETE
    // ====================================================================================
    [HttpDelete("{id:int}")]
    public ActionResult<string> DELETE_ParkingLot([FromHeader(Name = "authorization")] string authorization, int id)
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
        if (!foundUser.Role.GlobalParkingLots.HasWritePremission()) return Unauthorized();

        ParkingLot? foundParkingLot = ParkingLot.GetByID(id);
        if (foundParkingLot is null) return NotFound($"Parkinglot with ID: {id} not found");

        if (foundParkingLot.DeleteDB()) return Ok($"Parkinglot with ID: {id} has been successfully deleted");
        return Problem($"There was an problem while trying to delete the parkinglot with ID: {id}, please try again later");
    }

    [HttpDelete("{id:int}/sessions")]
    public ActionResult<string> DELETE_ParkingLotSessions([FromHeader(Name = "authorization")] string authorization, int id)
    {
        return Problem("NOT IMPLEMENTED");
    }

    [HttpDelete("{id:int}/sessions/{sessionID:int}")]
    public ActionResult<string> DELETE_ParkingLotSession([FromHeader(Name = "authorization")] string authorization, int id, int sessionID)
    {
        return Problem("NOT IMPLEMENTED");
    }
}