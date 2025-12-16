using Microsoft.AspNetCore.Mvc;
using MUser = User;

[ApiController]
[Route("[controller]")]
public class VehicleController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("{userID:int}")]
    public ActionResult<string> GET_Vehicles([FromHeader(Name = "authorization")] string authorization, int userID)
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
        if (!foundUser.Role.Vehicle.HasReadPremission()) return Unauthorized();

        if (foundUser.Role.GlobalVehicle.HasReadPremission())
        {
            //Return all vehicles in the system
            return Ok(Vehicle.ToJSON(Vehicle.GetByID(userID)));
        }

        // niet sure of het zo moet?
        return Ok(Vehicle.ToJSON(Vehicle.GetByID(foundUser.ID)));

    }

    [HttpGet("reservations/{ID:int}")]
    public ActionResult<string> GET_VehiclesReservations([FromHeader(Name = "authorization")] string authorization, int ID)
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
        if (!foundUser.Role.Vehicle.HasReadPremission()) return Unauthorized();

        List<Vehicle> vehicles = Vehicle.GetByUserID(foundUser.ID);

        foreach (Vehicle vehicle in vehicles)
        {
            if (vehicle.ID == ID)
            {
                List<Reservation> reservations = Reservation.GetAllByVehicle(vehicle.ID);
                return Ok(Reservation.ToJSON(reservations));
            }
        }

        return NotFound("No reservations found for this vehicle");

    }

    [HttpGet("history/{ID:int}")]
    public ActionResult<string> GET_VehiclesHistory([FromHeader(Name = "authorization")] string authorization, int ID)
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
        if (!foundUser.Role.Vehicle.HasReadPremission()) return Unauthorized();

        List<Vehicle> vehicles = Vehicle.GetByUserID(foundUser.ID);

        foreach (Vehicle vehicle in vehicles)
        {
            if (vehicle.ID == ID)
            {
                List<Reservation> reservations = Reservation.GetAllByVehicle(vehicle.ID);
                foreach (Reservation reservation in reservations)
                {
                    if (reservation.EndDateTime > DateTime.Now)
                    {
                        reservations.Remove(reservation);
                    }
                }
                return Ok(Reservation.ToJSON(reservations));
            }
        }

        return NotFound("No reservations found for this vehicle");
    }

    // ====================================================================================
    // POST
    // ====================================================================================
    [HttpPost("")]
    public ActionResult<string> POST_CreateVehicle()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpPost("/")]
    public ActionResult<string> POST_CreateVehicleS()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("/")]
    public ActionResult<string> PUT_UpdateReservation()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // DELETE
    // ====================================================================================
    [HttpDelete("/")]
    public ActionResult<string> DELETE_Reservation()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }
}