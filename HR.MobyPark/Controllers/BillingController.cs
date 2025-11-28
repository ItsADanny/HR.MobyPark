using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class BillingController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("")]
    public ActionResult<string> GET_Billing()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpGet("{userID:int}")]
    public ActionResult<string> GET_BillingForUser(int userID)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }
}