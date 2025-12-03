using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
//This is added because it would otherwise interfere with the ControllerBase's User
using MUser = User;

[ApiController]
// [Route("[controller]")]
public class AuthController : ControllerBase
{
    // ====================================================================================
    // GET
    // ====================================================================================
    [HttpGet("logout")]
    public ActionResult<string> logout([FromHeader(Name = "authorization")] string authorization)
    {
        if (string.IsNullOrWhiteSpace(authorization)) return BadRequest("Missing session token");
        if (!UserSession.SessionExist(authorization)) return Unauthorized("Session not found or expired");
        if (UserSession.RemoveSession(authorization)) return Ok("Logout successful");
        
        return Problem("There was an problem while trying to log you out of the application, please try again later");
    }

    // ====================================================================================
    // POST
    // ====================================================================================
    [HttpPost("register")]
    public ActionResult<string> register([FromBody] registerDto registerDto)
    {
        if (MUser.DoesEmailExist(registerDto.email) && MUser.DoesUsernameExist(registerDto.username)) 
            return Ok("Email & username already taken/used");
        else if (MUser.DoesUsernameExist(registerDto.username)) return Ok("Username already taken");
        else if (MUser.DoesEmailExist(registerDto.email)) return Ok("Email already used");
        else
        {
            User newAccount = new(registerDto.username, registerDto.fullname, registerDto.email, registerDto.phonenumber, registerDto.birthdate);
            newAccount.SetPassword(registerDto.password);

            if (newAccount.CreateDB())
            {
                return Created((string?)null, "User created");
            }
            return Problem("There was an error while trying to create a new account, please try again later");
        }
    }

    [HttpPost("login")]
    public ActionResult<string> login([FromBody] loginDto loginDto)
    {
        //See if the required data has been inputted to preform the user check
        if (string.IsNullOrWhiteSpace(loginDto.username) || string.IsNullOrWhiteSpace(loginDto.password)) return BadRequest("Missing credentials");
        
        //Retriev user
        MUser? foundUser = MUser.GetUserUsername(loginDto.username);

        //If the foundUser is null, return the message "User not found"
        if (foundUser is null) return NotFound("User not found");

        //Preform the check to see if the users password is valid
        if (!foundUser.IsPasswordCorrect(loginDto.password))
        {
            if (foundUser.IsLegacyPassword(loginDto.password))
            {
                //Update the password in the user object
                foundUser.UpdatePasswordFromLegacy(loginDto.password);
                //Update DB and check if its successful, if not return an internal error
                if (!foundUser.UpdateDB())
                {
                    return Problem("Internal password updating legacy account password, please contact support");
                }
            } 
            else
            {
                return Unauthorized("Invalid credentials");
            }
        }

        //Create a new session and if successful, return a login JSON
        string? token = UserSession.CreateSession(foundUser);
        if (token is not null)
        {
            JObject returnJSON = new JObject(
                new JProperty("message", "User logged in"),
                new JProperty("session_token", token)
            );

            return Ok(returnJSON);
        }
        //Otherwise, return a login error
        return Problem("There was an problem while trying to login, please try again later");
    }
}