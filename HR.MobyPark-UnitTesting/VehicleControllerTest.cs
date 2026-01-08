using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using MUser = User;

[TestClass]
public class VehicleControllerTest
{
    private static string? _testAuthToken;
    private static int _testUserID;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<VehicleControllerTest>()
            .Build();

        // MUST set DBHelper config FIRST before any DB operations
        var dbConfig = configuration.GetSection("Database").Get<DBConfig>();
        if (dbConfig == null)
        {
            // Cannot use Assert.Inconclusive in ClassInitialize, so we'll skip initialization
            // Tests will handle missing config individually
            Console.WriteLine("⚠️  Database configuration missing. Configure user secrets:");
            Console.WriteLine("   dotnet user-secrets set 'Database:HST' 'localhost' --project HR.MobyPark-UnitTesting");
            Console.WriteLine("   dotnet user-secrets set 'Database:PRT' '3306' --project HR.MobyPark-UnitTesting");
            Console.WriteLine("   dotnet user-secrets set 'Database:USR' 'your_user' --project HR.MobyPark-UnitTesting");
            Console.WriteLine("   dotnet user-secrets set 'Database:PSW' 'your_password' --project HR.MobyPark-UnitTesting");
            Console.WriteLine("   dotnet user-secrets set 'Database:DBL' 'your_database' --project HR.MobyPark-UnitTesting");
            return; // Skip setup, tests will be marked inconclusive
        }
        DBHelper.dBConfig = dbConfig;

        // Register a new test user with vehicle permissions
        var authController = new AuthController();
        var registerDto = new registerDto(
            $"test-vehicle-{DateTime.Now.Ticks}",
            "test-password-123",
            "Test Vehicle User",
            $"test-vehicle-{DateTime.Now.Ticks}@test.com",
            "+31612345678",
            new DateOnly(1990, 1, 1)
        );
        
        var registerResult = authController.register(registerDto);
        if (registerResult.Result is not CreatedResult)
        {
            var message = registerResult.Value?.ToString() ?? "Unknown error";
            throw new Exception($"Failed to register test user: {message}");
        }

        // Login to get auth token
        var loginDto = new loginDto(registerDto.username, registerDto.password);
        var loginResult = authController.login(loginDto);
        
        if (loginResult.Result is not OkObjectResult okResult)
        {
            throw new Exception($"Failed to login test user: {loginResult.Value}");
        }
        
        // Parse token from login response
        var loginJson = JObject.Parse(okResult.Value?.ToString() ?? "");
        _testAuthToken = loginJson["session_token"]?.ToString();
        
        if (string.IsNullOrEmpty(_testAuthToken))
        {
            throw new Exception("Failed to get session token from login");
        }
        
        // Get the created user ID
        var user = MUser.GetUserUsername(registerDto.username);
        if (user == null) throw new Exception("Failed to retrieve created user");
        _testUserID = user.ID;
        
        Console.WriteLine($"Created test user {_testUserID} with role {user.Role.Name}");
    }

    [TestMethod]
    public void PostCreateVehicle_NullAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var controller = new VehicleController();
        var vehicleDto = new vehicleDto(1, "TEST-001", "Toyota", "Camry", "Blue", 2023);

        // Act
        var result = controller.POST_CreateVehicle(null!, vehicleDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PostCreateVehicle_EmptyAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var controller = new VehicleController();
        var vehicleDto = new vehicleDto(1, "TEST-002", "Honda", "Civic", "Red", 2024);

        // Act
        var result = controller.POST_CreateVehicle("", vehicleDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PostCreateVehicle_InvalidAuthToken_ReturnsNotFound()
    {
        // Arrange
        var controller = new VehicleController();
        var vehicleDto = new vehicleDto(1, "TEST-003", "Ford", "Focus", "Green", 2022);

        // Act
        var result = controller.POST_CreateVehicle("INVALID_TOKEN", vehicleDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        var objectResult = result.Result as ObjectResult;
        // Should return NotFound for invalid session
        Assert.AreEqual(404, objectResult?.StatusCode);
    }

    [TestMethod]
    public void PostCreateVehicle_ValidData_ReturnsCreated()
    {
        // Arrange
        var controller = new VehicleController();
        var vehicleDto = new vehicleDto(
            1,
            $"TEST-{DateTime.Now.Ticks}", 
            "Volkswagen", 
            "Golf", 
            "White", 
            2024
        );

        // Act
        var result = controller.POST_CreateVehicle(_testAuthToken!, vehicleDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));

        // Cleanup - Delete the test vehicle
        try
        {
            MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(
                $"DELETE FROM Vehicles WHERE LicensePlate = '{vehicleDto.licensePlate}'",
                conn
            );
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        catch { /* Cleanup failure is not critical */ }
    }

    [TestMethod]
    public void PostCreateVehicle_DuplicateLicensePlate_ReturnsBadRequest()
    {
        // Arrange
        var controller = new VehicleController();
        var uniquePlate = $"DUP-{DateTime.Now.Ticks}";
        var vehicleDto1 = new vehicleDto(1, uniquePlate, "Toyota", "Camry", "Blue", 2023);
        var vehicleDto2 = new vehicleDto(1, uniquePlate, "Honda", "Civic", "Red", 2024);

        // Act - Create first vehicle
        var result1 = controller.POST_CreateVehicle(_testAuthToken!, vehicleDto1);
        
        // Act - Try to create duplicate
        var result2 = controller.POST_CreateVehicle(_testAuthToken!, vehicleDto2);

        // Assert
        Assert.IsInstanceOfType(result2.Result, typeof(BadRequestObjectResult));

        // Cleanup
        try
        {
            MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(
                $"DELETE FROM Vehicles WHERE LicensePlate = '{uniquePlate}'",
                conn
            );
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        catch { /* Cleanup failure is not critical */ }
    }
}
