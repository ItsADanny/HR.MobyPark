using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[TestClass]
public class VehicleAuthenticationTest
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<VehicleAuthenticationTest>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    // DELETE 
    [TestMethod]
    public void DeleteWithNull_ReturnsUnauthorized()
    {
        var controller = new VehicleController();

        var result = controller.DELETE_Vehicle(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void DeleteWithEmpty_ReturnsUnauthorized()
    {
        var controller = new VehicleController();

        var result = controller.DELETE_Vehicle("", 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void DeleteWithValidAuth()
    {
        // session maken
        var testToken = "test-token-123";
        var testUserID = 99;

        string insertSql = $"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserID}, '{testToken}', NOW())";
        DBHelper.Create(insertSql);

        // vehicle maken die we ook weer zullen verwijderen (direct via SQL)
        string insertVehicleSql = $"INSERT INTO Vehicles (UserID, LicensePlate, Make, Model, Color, Year) VALUES ({testUserID}, 'TEST-123', 'TestMake', 'TestModel', 'TestColor', 2020)";
        DBHelper.Create(insertVehicleSql);

        // Retrieve from database
        List<Vehicle> allVehicles = Vehicle.GetAll();
        Vehicle? createdVehicle = allVehicles.FirstOrDefault(v => v.UserID == testUserID && v.LicensePlate == "TEST-123");
        Assert.IsNotNull(createdVehicle, "test vehicle failed to create");

        var controller = new VehicleController();
        var result = controller.DELETE_Vehicle(testToken, createdVehicle.ID);
        Assert.IsFalse(result.Result is UnauthorizedResult, "DELETE should not return Unauthorized");

        // session verwijderen
        string deleteSql = $"DELETE FROM UserSessions WHERE Token = '{testToken}'";
        bool deleted = DBHelper.Delete(deleteSql);
        Assert.IsTrue(deleted, "failed to delete test sessie");
    }
}
