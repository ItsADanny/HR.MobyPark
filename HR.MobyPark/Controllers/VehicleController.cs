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
    public ActionResult<string> GET_Vehicles(int userID)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpGet("reservations")]
    public ActionResult<string> GET_VehiclesReservations()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpGet("history")]
    public ActionResult<string> GET_VehiclesHistory()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
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
    [HttpDelete("{id:int}")]
    public ActionResult<string> DELETE_Vehicle([FromHeader(Name = "authorization")] string authorization, int id)
    {
        //Controleer of de autorisatie is ingevuld, als dit niet het geval is, retourneer Unauthorized error
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        //Probeer de user sessie te halen, als dit niet lukt retourneer NotFound error
        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        //Als de user niet kan worden gevonden retourneer een server error
        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        //Zoek naar auto's met het gegeven ID
        Vehicle? foundVehicle = Vehicle.GetByID(id);
        if (foundVehicle is null) return NotFound($"No vehicle found with ID {id}");

        //Controleer of de auto van de user is
        if (foundVehicle.UserID != foundUser.ID) return Unauthorized();

        if (foundVehicle.DeleteDB()) return Ok($"Vehicle with ID {id} deleted successfully");
        return Problem($"Failed to delete vehicle with ID {id}");
    }
}