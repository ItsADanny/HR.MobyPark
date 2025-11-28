using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("")]
    public ActionResult<string> GET_Reservation()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpGet("{userID:int}")]
    public ActionResult<string> GET_PaymentForUser(int userID)
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // POST
    // ====================================================================================
    [HttpPost("")]
    public ActionResult<string> POST_CreatePayment()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    [HttpPost("")]
    public ActionResult<string> POST_CreatePaymentRefund()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("")]
    public ActionResult<string> PUT_UpdatePayment()
    {
        return Problem("NOT IMPLEMENTED, -Danny");
    }
}