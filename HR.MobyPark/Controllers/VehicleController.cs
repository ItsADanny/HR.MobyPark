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
    public ActionResult<string> POST_CreateVehicle([FromHeader(Name = "authorization")] string authorization, [FromBody] vehicleDto createvehicleDto)
    {
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        // Check if user has permission to create vehicles
        if (!foundUser.Role.Vehicle.HasWritePremission()) return Unauthorized();

        // Check for duplicate license plate for this user
        List<Vehicle> userVehicles = Vehicle.GetAll().Where(v => v.UserID == userSession.UserID).ToList();
        Vehicle? existingVehicle = userVehicles.FirstOrDefault(v => v.LicensePlate.Replace("-", "").Equals(createvehicleDto.licensePlate.Replace("-", ""), StringComparison.OrdinalIgnoreCase));
        
        if (existingVehicle != null)
            return BadRequest(new { error = "Vehicle already exists", data = Vehicle.ToJSON(existingVehicle) });

        // Create new vehicle with authenticated user's ID
        Vehicle newVehicle = new Vehicle(0, userSession.UserID, createvehicleDto.licensePlate, createvehicleDto.make, createvehicleDto.model, createvehicleDto.color, createvehicleDto.modelYear, DateTime.Now);

        if (newVehicle.CreateDB())
        {
           return Created();
        }
        
        return Problem("Failed to create vehicle in database");
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