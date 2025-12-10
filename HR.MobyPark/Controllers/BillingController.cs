using Microsoft.AspNetCore.Mvc;
using MUser = User;

[ApiController]
[Route("[controller]")]
public class BillingController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("")]
    public ActionResult<string> GET_Billing([FromHeader(Name = "authorization")] string authorization)
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
        if (!foundUser.Role.User.HasReadPremission()) return Unauthorized();

        // return all the billings from the user
        // wat moet ik hier doen?
        // ...

        //Return the result
        return Ok();
    }

    [HttpGet("{userID:int}")]
    public ActionResult<string> GET_BillingForSpecificUser([FromHeader(Name = "authorization")] string authorization, int userID)
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
        if (!foundUser.Role.User.HasReadPremission()) return Unauthorized();
        
        // hoe check ik admin rights?


        // misschien zoiets? maar dan hebben we ook UserID nodig nee?
        // Billing? foundBilling = Billing.GetByUserID(userID);
        // if (foundBilling is null) return NotFound($"No billing found matching ID: {id}");


        //Return the result
        // return Ok(Billing.ToJSON(foundBilling));
        return Ok();
    }
}