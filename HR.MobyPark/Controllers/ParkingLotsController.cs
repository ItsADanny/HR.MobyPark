using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ParkingLotsController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("")]
    public ActionResult<string> GET_AllParkingLots()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpGet("{id:int}")]
    public ActionResult<string> GET_ParkingLot(int id)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpGet("{id:int}/sessions")]
    public ActionResult<string> GET_ParkingLotSessions(int id)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // POST
    // ====================================================================================
    [HttpPost("")]
    public ActionResult<string> POST_CreateParkingLot()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("{id:int}")]
    public ActionResult<string> PUT_UpdateParkingLot(int id)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // DELETE
    // ====================================================================================
    [HttpDelete("{id:int}")]
    public ActionResult<string> DELETE_ParkingLot(int id)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpDelete("{id:int}/sessions")]
    public ActionResult<string> DELETE_ParkingLotSessions(int id)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpDelete("{id:int}/sessions/{sessionID:int}")]
    public ActionResult<string> DELETE_ParkingLotSession(int id, int sessionID)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }
}