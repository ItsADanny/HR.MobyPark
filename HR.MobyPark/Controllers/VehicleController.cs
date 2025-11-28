using Microsoft.AspNetCore.Mvc;

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
    [HttpDelete("/")]
    public ActionResult<string> DELETE_Reservation()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }
}