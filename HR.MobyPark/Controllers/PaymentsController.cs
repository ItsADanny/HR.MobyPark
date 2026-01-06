using Microsoft.AspNetCore.Mvc;
using MUser = User;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("")]
    public ActionResult<string> GET_Payments([FromHeader(Name = "authorization")] string authorization)
    {
        //Check to see if the authorization has been filled, If it isn't filled. Return Unauthorized error
        if (string.IsNullOrEmpty(authorization)) return Unauthorized();

        //Try and get the user session, if not return NotFound error
        UserSession? userSession = UserSession.GetUserSession(authorization);
        if (userSession is null) return NotFound("Session not found");

        //Get the user from the database
        MUser? user = MUser.GetUser(userSession.UserID);
        //Check to see if the user could be found in the database
        if (user is null) return Problem();

        //Check to see if the user has the required premission
        if(user.Role.Payments.HasReadPremission()) {
            List<Payment> payments = Payment.GetAllByUserID(userSession.UserID);
            //Clean the payments from sensitive data
            foreach (Payment payment in payments)
            {
                payment.SessionID = 0;
                payment.Hash = null;
            }

            return Ok(payments);
        }

        return Problem("There was an error while trying to retriev the payments, please try again later");
    }

    [HttpGet("{id:int}")]
    public ActionResult<string> GET_PaymentForID([FromHeader(Name = "authorization")] string authorization, int id)
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
        if (!foundUser.Role.GlobalPayments.HasReadPremission()) return Unauthorized();

        //Try and get the payment, if the payment doesn't exist return a NotFound error
        Payment? foundPayment = Payment.GetByID(id);
        if (foundPayment is null) return NotFound("Payment not found");

        foundPayment.SessionID = 0;
        foundPayment.Hash = null;

        return Ok(foundPayment);
    }

    // ====================================================================================
    // POST
    // ====================================================================================
    [HttpPost("")]
    public ActionResult<string> POST_CreatePayment([FromHeader(Name = "authorization")] string authorization, [FromBody] createPaymentDto createPaymentDto)
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
        if (!foundUser.Role.GlobalPayments.HasWritePremission()) return Unauthorized();

        //Check to see if the transaction is filled and the amount is equal or above 0
        if (string.IsNullOrEmpty(createPaymentDto.transaction) || createPaymentDto.amount <= 0) return BadRequest("Require field missing");

        //Get the current parking session for the user
        Session? CurrentSession = Session.GetByUserID(userSession.UserID);
        if (CurrentSession is null) return Problem("There was an problem while trying to retrieve your current session, please try again later");

        Payment payment = new()
        {
            TransactionID = createPaymentDto.transaction,
            ParkingLotID = CurrentSession.ParkingLotId,
            SessionID = CurrentSession.ID,
            UserID = CurrentSession.UserID,
            Amount = createPaymentDto.amount,
            Method = "Unknown", // method is not provided in the request, assuming a default value of "Unknown"
            Issuer = "Unknown", // issuer is not provided in the request, assuming a default value of "
            Bank = "Unknown" // bank is not provided in the request, assuming a default value of "Unknown"
        };
        if (payment.CreateDB()) return Problem("There was an problem while trying to create a payment, try again later");

        return Created(); //Originally Ok(), but Created() is more logical here
    }

    [HttpPost("refund")]
    public ActionResult<string> POST_CreatePaymentRefund([FromHeader(Name = "authorization")] string authorization, [FromBody] decimal amount)
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
        if (!foundUser.Role.GlobalPayments.HasWritePremission()) return Unauthorized();

        //Check to see if the transaction is filled and the amount is equal or above 0
        if (amount <= Convert.ToDecimal(0.0)) return BadRequest("Require field missing");

        //Get the current parking session for the user
        Session? CurrentSession = Session.GetByUserID(userSession.UserID);
        if (CurrentSession is null) return Problem("There was an problem while trying to retrieve your current session, please try again later");

        string transaction = $"Refund {CurrentSession.UserID}"; // to be implemented

        // add payment creation logic here
        Payment refundPayment = new () {
            TransactionID = transaction, 
            ParkingLotID = CurrentSession.ParkingLotId,
            SessionID = CurrentSession.ID,
            UserID = userSession.UserID, 
            Amount = amount, 
            Method = "Unknown", // method is not provided in the request, assuming a default value of "Unknown"
            Issuer = "Unknown", // issuer is not provided in the request, assuming a default value of "
            Bank = "Unknown"  // bank is not provided in the request, assuming a default value of "Unknown"
        };
        if (refundPayment.CreateDB()) return Problem("There was an problem while trying to create a refund, try again later");

        return Created(); //Originally Ok(), but Created() is more logical here
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("{id:int}")]
    public ActionResult<string> PUT_UpdatePayment([FromHeader(Name = "authorization")] string authorization, int id, [FromBody] paymentDto paymentDto)
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
        if (!foundUser.Role.GlobalPayments.HasWritePremission()) return Unauthorized();

        //Check to see if the transaction is filled and the amount is equal or above 0
        if (paymentDto.amount <= Convert.ToDecimal(0.0) || 
            paymentDto.date == default || 
            string.IsNullOrEmpty(paymentDto.method) || 
            string.IsNullOrEmpty(paymentDto.issuer) || 
            string.IsNullOrEmpty(paymentDto.bank) || 
            paymentDto.sessionID <= 0 || 
            paymentDto.parkingLotID <= 0) return BadRequest("Require field missing");

        //Get the current parking session for the user
        Session? CurrentSession = Session.GetByUserID(userSession.UserID);
        if (CurrentSession is null) return Problem("There was an problem while trying to retrieve your current session, please try again later");

        //Try and get the payment, if the payment doesn't exist return a NotFound error
        Payment? foundPayment = Payment.GetByID(id);
        if (foundPayment is null) return NotFound("Payment not found");
        
        if (foundPayment.UpdateDB()) return Problem("There was an error while trying to update the transaction, please try again later");

        return Ok();
    }
}