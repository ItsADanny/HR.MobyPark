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

    [HttpPost("{licensePlateId}/entry")]
    public ActionResult<string> POST_VehicleEntry([FromHeader(Name = "authorization")] string authorization, string licensePlateId, [FromBody] int parkinglot)
    {
        if (string.IsNullOrEmpty(authorization)) return Unauthorized("Unauthorized: Invalid or missing session token");

        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return Unauthorized("Unauthorized: Invalid or missing session token");

        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("There was an problem while trying to retrieve your account, please try again later");

        // Validate required field ????????????
        if (parkinglot == 0)
            return BadRequest(new { error = "Required field missing", field = "parkinglot" });

        // Get user's vehicles and find by license plate (without hyphens)
        List<Vehicle> userVehicles = Vehicle.GetAll().Where(v => v.UserID == userSession.UserID).ToList();
        Vehicle? vehicle = userVehicles.FirstOrDefault(v => v.LicensePlate.Replace("-", "").Equals(licensePlateId.Replace("-", ""), StringComparison.OrdinalIgnoreCase));

        if (vehicle == null)
        {
            return Problem("Vehicle not found");
        }
        return Ok(Vehicle.ToJSON(vehicle));
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("{id:int}")]
    public ActionResult<string> PUT_UpdateVehicle(int id,[FromHeader(Name = "authorization")] string authorization,[FromBody] updateVehicleDto dto)
    {
        if (string.IsNullOrEmpty(authorization)) return Unauthorized("Missing authorization header");

        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return Unauthorized("Invalid session");

        MUser? foundUser = MUser.GetUser(userSession.UserID);
        if (foundUser is null) return Problem("User account not found");

        // check if fields are missing
        if (string.IsNullOrEmpty(dto.make) ||
            string.IsNullOrEmpty(dto.model) ||
            string.IsNullOrEmpty(dto.color) ||
            dto.modelYear <= 1900)
        {
            return BadRequest("Required field missing or invalid");
        }

        // load vehicle
        Vehicle? vehicle = Vehicle.GetByID(id);
        if (vehicle is null) return NotFound("Vehicle not found");

        // check owner
        if (vehicle.UserID != userSession.UserID) return Unauthorized("You do not own this vehicle");

        // apply changes
        vehicle.LicensePlate = dto.licensePlate;
        vehicle.Make = dto.make;
        vehicle.Model = dto.model;
        vehicle.Color = dto.color;
        vehicle.ModelYear = dto.modelYear;
        vehicle.UpdateDateTime = DateTime.Now;

        bool updated = vehicle.UpdateDB();
        if (!updated) return Problem("Failed to update vehicle");

        return Ok(new{status = "Success", vehicle = Vehicle.ToJSON(vehicle)});
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