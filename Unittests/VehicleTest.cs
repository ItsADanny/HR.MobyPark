using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[TestClass]
public sealed class VehicleTest
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<VehicleTest>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [TestMethod]
    public void UpdateDB_updatesVehicleSuccessfully()
    {
        var vehicle = new Vehicle
        {
            UserID = 10,
            LicensePlate = "12-df-9",
            Make = "KIA",
            Model = "Picanto",
            Color = "Blue",
            ModelYear = 2006,
            CreateDateTime = DateTime.Now
        };
        vehicle.CreateDB();

        List<Vehicle> vehicles = Vehicle.GetAll();
        Vehicle? createdVehicle = vehicles
            .FirstOrDefault(v => v.UserID == 10 && v.LicensePlate == "12-df-9");

        Assert.IsNotNull(createdVehicle, "vehicle was not inserted");

        createdVehicle.Make = "updateMake";
        createdVehicle.Model = "updateModel";
        createdVehicle.Color = "updateColor";
        createdVehicle.ModelYear = 2025;
        createdVehicle.UpdateDateTime = DateTime.Now;
        bool result = createdVehicle.UpdateDB();

        Assert.IsTrue(result);

        // reload vehicle to double check
        Vehicle? updatedVehicle = Vehicle.GetByID(createdVehicle.ID);
        Assert.IsNotNull(updatedVehicle);

        Assert.AreEqual("updateMake", updatedVehicle.Make);
        Assert.AreEqual("updateModel", updatedVehicle.Model);
        Assert.AreEqual("updateColor", updatedVehicle.Color);
        Assert.AreEqual(2025, updatedVehicle.ModelYear);
    }

    //=============================================================
    //PUT sad flow tests
    //=============================================================
    [TestMethod]
    public void updateVehicle_emptyAuthorization_fail()
    {
        var controller = new VehicleController();

        var dto = new updateVehicleDto
        (
            licensePlate: "76-ABC-7",
            make: "BMW",
            model: "X5",
            color: "Black",
            modelYear: 2024
        );

        var result = controller.PUT_UpdateVehicle(1, "", dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public void updateVehicle_nullAuthorization_fail()
    {
        var controller = new VehicleController();

        var dto = new updateVehicleDto
        (
            licensePlate: "76-ABC-7",
            make: "BMW",
            model: "X5",
            color: "Black",
            modelYear: 2024
        );

        var result = controller.PUT_UpdateVehicle(1, null!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public void updateVehicle_validAuth_pass()
    {
        var testToken = "update-test-123";
        var testUserId = 10;

        string insertSql = $"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserId}, '{testToken}', NOW())";
        DBHelper.Create(insertSql);

        var testVehicle = new Vehicle
        {
            UserID = 11,
            LicensePlate = "67-tyt-3",
            Make = "abc",
            Model = "bca",
            Color = "white",
            ModelYear = 1990,
            CreateDateTime = DateTime.Now
        };
        testVehicle.CreateDB();

        List<Vehicle> vehiclesInDb = Vehicle.GetAll();
        Vehicle? createdVehicle = vehiclesInDb.FirstOrDefault(v => v.UserID == 11 && v.LicensePlate == "67-tyt-3");
        Assert.IsNotNull(createdVehicle, "test vehicle failed to create");

        var controller = new VehicleController();
        var dto = new updateVehicleDto
        (
            licensePlate: "12-dfi-3",
            make: "updatedMake",
            model: "updatedModel",
            color: "updatedColor",
            modelYear: 2024
        );
        var result = controller.PUT_UpdateVehicle(createdVehicle.ID, testToken, dto);
        Assert.IsFalse(result.Result is UnauthorizedResult, "PUT should not return unauthorized ");

        testVehicle.DeleteDB();
        DBHelper.Delete($"DELETE FROM UserSessions WHERE Token = '{testToken}'");
        
    }
}
