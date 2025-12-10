using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

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
        // vehicle with id must be in database
        int existingVehicleId = 1;

        Vehicle? vehicle = Vehicle.GetByID(existingVehicleId);
        Assert.IsNotNull(vehicle);

        vehicle.Make = "updatedMakeTest";
        vehicle.Model = "updatedModelTest";
        vehicle.Color = "updatedColorTest";
        vehicle.ModelYear = 2025;
        vehicle.UpdateDateTime = DateTime.Now;

        bool result = vehicle.UpdateDB();

        Assert.IsTrue(result);

        // reload vehicle to double check
        Vehicle? updatedVehicle = Vehicle.GetByID(existingVehicleId);
        Assert.IsNotNull(updatedVehicle);

        Assert.AreEqual("updatedMakeTest", updatedVehicle.Make);
        Assert.AreEqual("updatedModelTest", updatedVehicle.Model);
        Assert.AreEqual("updatedColorTest", updatedVehicle.Color);
        Assert.AreEqual(2025, updatedVehicle.ModelYear);
    }

    //=============================================================
    //PUT sad flow tests
    //=============================================================
    [TestMethod]
    public void updateVehicle_emptyAuthorization_fail()
    {
        var controller = new VehicleController();

        var dto = new vehicleDto
        (
            userID: 1,
            licensePlate: "76-ABC-7",
            make: "BMW",
            model: "X5",
            color: "Black",
            modelYear: 2024
        );

        var result = controller.PUT_UpdateVehicle(1, "", dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void updateVehicle_nullAuthorization_fail()
    {
        var controller = new VehicleController();

        var dto = new vehicleDto
        (
            userID: 1,
            licensePlate: "76-ABC-7",
            make: "BMW",
            model: "X5",
            color: "Black",
            modelYear: 2024
        );

        var result = controller.PUT_UpdateVehicle(1, null!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }
}
