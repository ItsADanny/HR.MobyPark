using Microsoft.AspNetCore.Mvc;
using MUser = User;

[ApiController]
[Route("[controller]")]
public class ProfileController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("")]
    public ActionResult<string> GETProfile([FromHeader(Name = "authorization")] string authorization)
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

        //Return the result
        return Ok(MUser.ToJSON(foundUser));
    }

    // ====================================================================================
    // PUT
    // ====================================================================================
    [HttpPut("")]
    public ActionResult<string> PUTProfile([FromHeader(Name = "authorization")] string authorization, registerDto registerDto)
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
        if (!foundUser.Role.User.HasWritePremission()) return Unauthorized();

        foundUser.Username = registerDto.username;
        foundUser.Name = registerDto.fullname;
        foundUser.Email = registerDto.email;
        foundUser.Phone = registerDto.phonenumber;
        foundUser.BirthDate = registerDto.birthdate;

        //Update the password
        //This method will handle the new salt and hashing for the password since this is an update
        foundUser.SetPassword(registerDto.password);

        if (foundUser.UpdateDB()) return Ok("Profile successfully updated");
        return Problem("Unable to update profile, please try again later");
    }
}