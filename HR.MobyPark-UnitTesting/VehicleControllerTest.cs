using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MUser = User;

[TestClass]
public class VehicleControllerTest
{
    private static string? _testAuthToken;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<VehicleControllerTest>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();

        // Get or create a test user session for integration tests
        // Using user ID 1 for testing
        _testAuthToken = CreateTestUserSession(1);
    }

    private static string CreateTestUserSession(int userId)
    {
        // Check if session already exists
        var existingSessions = UserSession.GetByUserID(userId);
        if (existingSessions != null && existingSessions.Count > 0)
        {
            return existingSessions[0].Token;
        }

        // Create new session
        string token = Auth.GenAuthToken();
        string insertSQL = $"INSERT INTO UserSessions (UserID, Token) VALUES ({userId}, '{token}')";
        DBHelper.Create(insertSQL);
        return token;
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
