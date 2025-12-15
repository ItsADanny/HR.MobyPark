using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Tests;

[TestClass]
public sealed class VehicleControllerTests
{
    [TestMethod]
    public void PostCreatePayment_NullAuthorization()
    {
        // Arrange
        var controller = new VehicleController();
        string authorization = null;
        var createvehicleDto = new vehicleDto(
            1,
            "TEST-123",
            "TestMake",
            "TestModel",
            "Red",
            2024
        );

        // Act
        var result = controller.POST_CreateVehicle(authorization, createvehicleDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }
}

[TestClass]
[DoNotParallelize]
public sealed class VehicleModelTests
{
    // Static variable to store the created payment ID for use across tests
    private static int _createdPaymentId = 0;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PaymentModelTests>()
            .Build();

        // Initialize DBHelper with configuration from backend
        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [TestMethod]
    [Priority(0)]
    public void CreateDB_InsertsPaymentSuccessfully()
    {
        // Arrange - Use unique values to avoid conflicts
        Vehicle vehicleobject = new Vehicle(
            0, // ID will be set by DB
            1, 
            "TEST-123",
            "TestMake",
            "TestModel",
            "Red",
            2024,
            DateTime.Now
        );

        // Act
        bool result = vehicleobject.CreateDB();
        // Assert
        Assert.IsTrue(result, "CreateDB should return true when vehicle is created successfully");
    }
}
