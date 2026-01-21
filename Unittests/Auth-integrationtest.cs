using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[TestClass]
[DoNotParallelize]
public sealed class AuthControllerIntegrationTests
{
    private static string? _validAuthToken;
    private static string _testUsername = "";
    private static string _testEmail = "";
    private static int _testUserID = 0;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AuthControllerIntegrationTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Clean up test data
        if (_testUserID > 0)
        {
            DBHelper.Delete($"DELETE FROM UserSessions WHERE UserID = {_testUserID}");
            DBHelper.Delete($"DELETE FROM UserSalts WHERE UserID = {_testUserID}");
            DBHelper.Delete($"DELETE FROM Users WHERE ID = {_testUserID}");
        }
    }

    // REGISTER Tests
    [TestMethod]
    [Priority(0)]
    public void Register_ValidData_ReturnsCreated()
    {
        // Arrange
        var controller = new AuthController();
        _testUsername = $"test-auth-integration-{DateTime.Now.Ticks}";
        _testEmail = $"test-auth-{DateTime.Now.Ticks}@integration.test";
        
        var registerDto = new registerDto(
            _testUsername,
            "TestPassword123!",
            "Test Integration User",
            _testEmail,
            "+31612345678",
            new DateOnly(1990, 1, 1)
        );

        // Act
        var result = controller.register(registerDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult), "Should return 201 Created");
        
        // Store user ID for cleanup
        var user = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(user);
        _testUserID = user.ID;
    }

    [TestMethod]
    [Priority(1)]
    public void Register_DuplicateUsername_ReturnsOkWithMessage()
    {
        // Arrange
        var controller = new AuthController();
        var registerDto = new registerDto(
            _testUsername, // Same username as test above
            "AnotherPassword123!",
            "Another User",
            $"different-{DateTime.Now.Ticks}@integration.test",
            "+31698765432",
            new DateOnly(1992, 6, 15)
        );

        // Act
        var result = controller.register(registerDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), "Should return 200 OK");
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue(okResult.Value?.ToString()?.Contains("Username already taken") ?? false, 
            "Message should indicate username is taken");
    }

    [TestMethod]
    [Priority(2)]
    public void Register_DuplicateEmail_ReturnsOkWithMessage()
    {
        // Arrange
        var controller = new AuthController();
        var registerDto = new registerDto(
            $"different-user-{DateTime.Now.Ticks}",
            "AnotherPassword123!",
            "Another User",
            _testEmail, // Same email as first test
            "+31698765432",
            new DateOnly(1992, 6, 15)
        );

        // Act
        var result = controller.register(registerDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), "Should return 200 OK");
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue(okResult.Value?.ToString()?.Contains("Email already used") ?? false, 
            "Message should indicate email is used");
    }

    [TestMethod]
    [Priority(3)]
    public void Register_BothDuplicateUsernameAndEmail_ReturnsOkWithMessage()
    {
        // Arrange
        var controller = new AuthController();
        var registerDto = new registerDto(
            _testUsername, // Same username
            "AnotherPassword123!",
            "Another User",
            _testEmail, // Same email
            "+31698765432",
            new DateOnly(1992, 6, 15)
        );

        // Act
        var result = controller.register(registerDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), "Should return 200 OK");
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue(okResult.Value?.ToString()?.Contains("Email") ?? false, 
            "Message should indicate both are taken");
        Assert.IsTrue(okResult.Value?.ToString()?.Contains("username") ?? false, 
            "Message should indicate both are taken");
    }

    // LOGIN Tests
    [TestMethod]
    [Priority(4)]
    public void Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var controller = new AuthController();
        var loginDto = new loginDto(_testUsername, "TestPassword123!");

        // Act
        var result = controller.login(loginDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), "Should return 200 OK");
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        // Parse JSON response
        var jsonResponse = JObject.Parse(okResult.Value?.ToString() ?? "");
        Assert.IsTrue(jsonResponse.ContainsKey("session_token"), "Response should contain session_token");
        Assert.IsTrue(jsonResponse.ContainsKey("message"), "Response should contain message");
        
        _validAuthToken = jsonResponse["session_token"]?.ToString();
        Assert.IsFalse(string.IsNullOrWhiteSpace(_validAuthToken), "Session token should not be empty");
    }

    [TestMethod]
    [Priority(5)]
    public void Login_InvalidUsername_ReturnsNotFound()
    {
        // Arrange
        var controller = new AuthController();
        var loginDto = new loginDto("nonexistent-user-12345", "SomePassword123!");

        // Act
        var result = controller.login(loginDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult), "Should return 404 Not Found");
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.IsTrue(notFoundResult.Value?.ToString()?.Contains("User not found") ?? false,
            "Message should indicate user not found");
    }

    [TestMethod]
    [Priority(6)]
    public void Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var controller = new AuthController();
        var loginDto = new loginDto(_testUsername, "WrongPassword123!");

        // Act
        var result = controller.login(loginDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult), "Should return 401 Unauthorized");
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.IsTrue(unauthorizedResult.Value?.ToString()?.Contains("Invalid credentials") ?? false,
            "Message should indicate invalid credentials");
    }

    [TestMethod]
    [Priority(7)]
    public void Login_MissingUsername_ReturnsBadRequest()
    {
        // Arrange
        var controller = new AuthController();
        var loginDto = new loginDto("", "SomePassword123!");

        // Act
        var result = controller.login(loginDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult), "Should return 400 Bad Request");
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsTrue(badRequestResult.Value?.ToString()?.Contains("Missing credentials") ?? false,
            "Message should indicate missing credentials");
    }

    [TestMethod]
    [Priority(8)]
    public void Login_MissingPassword_ReturnsBadRequest()
    {
        // Arrange
        var controller = new AuthController();
        var loginDto = new loginDto(_testUsername, "");

        // Act
        var result = controller.login(loginDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult), "Should return 400 Bad Request");
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsTrue(badRequestResult.Value?.ToString()?.Contains("Missing credentials") ?? false,
            "Message should indicate missing credentials");
    }

    // LOGOUT Tests
    [TestMethod]
    [Priority(9)]
    public void Logout_ValidToken_ReturnsOk()
    {
        // Arrange
        var controller = new AuthController();
        Assert.IsFalse(string.IsNullOrWhiteSpace(_validAuthToken), "Valid auth token should exist from login test");

        // Act
        var result = controller.logout(_validAuthToken!);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), "Should return 200 OK");
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue(okResult.Value?.ToString()?.Contains("Logout successful") ?? false,
            "Message should indicate successful logout");
        
        // Verify session no longer exists
        Assert.IsFalse(UserSession.SessionExist(_validAuthToken!), 
            "Session should no longer exist after logout");
    }

    [TestMethod]
    [Priority(10)]
    public void Logout_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var controller = new AuthController();
        string invalidToken = "invalid-token-12345";

        // Act
        var result = controller.logout(invalidToken);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult), "Should return 401 Unauthorized");
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.IsTrue(unauthorizedResult.Value?.ToString()?.Contains("Session not found") ?? false,
            "Message should indicate session not found");
    }

    [TestMethod]
    [Priority(11)]
    public void Logout_MissingToken_ReturnsBadRequest()
    {
        // Arrange
        var controller = new AuthController();

        // Act
        var result = controller.logout("");

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult), "Should return 400 Bad Request");
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsTrue(badRequestResult.Value?.ToString()?.Contains("Missing session token") ?? false,
            "Message should indicate missing token");
    }

    // Session Flow Tests
    [TestMethod]
    [Priority(12)]
    public void SessionFlow_LoginAndLogoutMultipleTimes_WorksCorrectly()
    {
        // Arrange
        var controller = new AuthController();
        var loginDto = new loginDto(_testUsername, "TestPassword123!");

        // First login
        var loginResult1 = controller.login(loginDto);
        Assert.IsInstanceOfType(loginResult1.Result, typeof(OkObjectResult));
        var token1 = JObject.Parse(((OkObjectResult)loginResult1.Result!).Value?.ToString() ?? "")["session_token"]?.ToString();
        Assert.IsFalse(string.IsNullOrWhiteSpace(token1));

        // First logout
        var logoutResult1 = controller.logout(token1!);
        Assert.IsInstanceOfType(logoutResult1.Result, typeof(OkObjectResult));

        // Second login
        var loginResult2 = controller.login(loginDto);
        Assert.IsInstanceOfType(loginResult2.Result, typeof(OkObjectResult));
        var token2 = JObject.Parse(((OkObjectResult)loginResult2.Result!).Value?.ToString() ?? "")["session_token"]?.ToString();
        Assert.IsFalse(string.IsNullOrWhiteSpace(token2));

        // Verify tokens are different
        Assert.AreNotEqual(token1, token2, "Each login should generate a unique token");

        // Second logout
        var logoutResult2 = controller.logout(token2!);
        Assert.IsInstanceOfType(logoutResult2.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    [Priority(13)]
    public void SessionFlow_DoubleLogout_SecondLogoutFails()
    {
        // Arrange
        var controller = new AuthController();
        var loginDto = new loginDto(_testUsername, "TestPassword123!");

        // Login
        var loginResult = controller.login(loginDto);
        var token = JObject.Parse(((OkObjectResult)loginResult.Result!).Value?.ToString() ?? "")["session_token"]?.ToString();

        // First logout
        var logoutResult1 = controller.logout(token!);
        Assert.IsInstanceOfType(logoutResult1.Result, typeof(OkObjectResult));

        // Second logout with same token
        var logoutResult2 = controller.logout(token!);
        Assert.IsInstanceOfType(logoutResult2.Result, typeof(UnauthorizedObjectResult), 
            "Second logout should fail with Unauthorized");
    }

    [TestMethod]
    [Priority(14)]
    public void SessionFlow_MultipleSimultaneousSessions_AllValid()
    {
        // Arrange
        var controller = new AuthController();
        var loginDto = new loginDto(_testUsername, "TestPassword123!");

        // Create multiple sessions
        var loginResult1 = controller.login(loginDto);
        var token1 = JObject.Parse(((OkObjectResult)loginResult1.Result!).Value?.ToString() ?? "")["session_token"]?.ToString();

        var loginResult2 = controller.login(loginDto);
        var token2 = JObject.Parse(((OkObjectResult)loginResult2.Result!).Value?.ToString() ?? "")["session_token"]?.ToString();

        var loginResult3 = controller.login(loginDto);
        var token3 = JObject.Parse(((OkObjectResult)loginResult3.Result!).Value?.ToString() ?? "")["session_token"]?.ToString();

        // Assert all tokens are valid and unique
        Assert.IsTrue(UserSession.SessionExist(token1!), "First session should exist");
        Assert.IsTrue(UserSession.SessionExist(token2!), "Second session should exist");
        Assert.IsTrue(UserSession.SessionExist(token3!), "Third session should exist");
        
        Assert.AreNotEqual(token1, token2, "Tokens should be unique");
        Assert.AreNotEqual(token2, token3, "Tokens should be unique");
        Assert.AreNotEqual(token1, token3, "Tokens should be unique");

        // Cleanup - logout all sessions
        controller.logout(token1!);
        controller.logout(token2!);
        controller.logout(token3!);
    }
}
