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

    [HttpGet("reservations")]
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

    [HttpGet("history")]
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