using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("{id:int}")]
    public ActionResult<string> GET_Reservation(int id)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // POST
    // ====================================================================================
    [HttpPost("")]
    public ActionResult<string> POST_CreateReservation()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("")]
    public ActionResult<string> PUT_UpdateReservation()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // DELETE
    // ====================================================================================
    [HttpDelete("{id:int}")]
    public ActionResult<string> DELETE_Reservation(int id)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }
}