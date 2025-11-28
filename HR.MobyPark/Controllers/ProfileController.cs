using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ProfileController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("")]
    public ActionResult<string> GETProfile()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("")]
    public ActionResult<string> PUTProfile()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }
}