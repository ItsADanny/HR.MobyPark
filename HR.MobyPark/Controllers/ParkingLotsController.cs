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
    public ActionResult<string> PUT_UpdateParkingLot([FromHeader(Name = "authorization")] string authorization,int id,[FromBody] parkingLotDto parkingLotDto)
    {
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was a problem while trying to retrieve your account, please try again later");

        // check admin permission
        if (!foundUser.Role.GlobalParkingLots.HasWritePremission() || !foundUser.Role.ParkingLots.HasWritePremission())
            return Unauthorized();

        ParkingLot? foundParkingLot = ParkingLot.GetByID(id);
        if (foundParkingLot is null) return NotFound($"ParkingLot with ID: {id} not found");

        // Validate dto
        if (string.IsNullOrEmpty(parkingLotDto.name) ||
            string.IsNullOrEmpty(parkingLotDto.location) ||
            string.IsNullOrEmpty(parkingLotDto.address) ||
            parkingLotDto.capacity <= 0 ||
            parkingLotDto.tariff < 0 ||
            parkingLotDto.dayTariff < 0 ||
            parkingLotDto.lat < -90 || parkingLotDto.lat > 90 ||
            parkingLotDto.lon < -180 || parkingLotDto.lon > 180)
        {
            return BadRequest("Required field missing or invalid");
        }

        // update parking lot fields
        foundParkingLot.Name = parkingLotDto.name;
        foundParkingLot.Location = parkingLotDto.location;
        foundParkingLot.Address = parkingLotDto.address;
        foundParkingLot.Capacity = parkingLotDto.capacity;
        foundParkingLot.Tariff = parkingLotDto.tariff;
        foundParkingLot.DayTariff = parkingLotDto.dayTariff;
        foundParkingLot.Latitude = parkingLotDto.lat;
        foundParkingLot.Longitude = parkingLotDto.lon;
        foundParkingLot.UpdateDateTime = DateTime.Now;

        if (!foundParkingLot.UpdateDB())
            return Problem($"There was a problem while trying to update the ParkingLot with ID: {id}");

        return Ok($"ParkingLot with ID: {id} has been successfully updated");
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
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        // check admin permission
        if (!foundUser.Role.GlobalParkingLots.HasWritePremission()) return Unauthorized();

        ParkingLot? foundParkingLot = ParkingLot.GetByID(id);
        if (foundParkingLot is null) return NotFound($"Parkinglot with ID: {id} not found");

        // get all sessions for this parking lot
        List<Session> sessions = Session.GetAllByParkingLotID(id);

        foreach (Session session in sessions)
        {
            session.DeleteDB();
        }

        return Ok($"All sessions for parkinglot with ID: {id} have been successfully deleted");
    }

    [HttpDelete("{id:int}/sessions/{sessionID:int}")]
    public ActionResult<string> DELETE_ParkingLotSession([FromHeader(Name = "authorization")] string authorization, int id, int sessionID)
    {
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        ParkingLot? foundParkingLot = ParkingLot.GetByID(id);
        if (foundParkingLot is null) return NotFound($"Parkinglot with ID: {id} not found");

        Session? foundSession = Session.GetByID(sessionID);
        if (foundSession is null) return NotFound($"Session with ID: {sessionID} not found");

        // check if session belongs this parking lot
        if (foundSession.ParkingLotId != id) return Unauthorized();

        // admin can delete any session, users only their own
        if (!foundUser.Role.GlobalParkingLots.HasWritePremission() && foundSession.UserID != foundUser.ID)
        {
            return Unauthorized();
        }

        if (foundSession.DeleteDB())
            return Ok($"Session with ID: {sessionID} has been successfully deleted");

        return Problem($"There was an problem while trying to delete the session with ID: {sessionID}, please try again later");
    }
}