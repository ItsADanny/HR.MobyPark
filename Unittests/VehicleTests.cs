using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;


namespace MyProject.Tests;

[TestClass]
public sealed class VehicleControllerTests
{
    // ------------------------------
    // GET
    // ------------------------------
    [TestMethod]
    public void GetVehicle_NullAuthorization()
    {
        var controller = new VehicleController();
        var result = controller.GET_Vehicles(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetVehicle_EmptyAuthorization()
    {
        var controller = new VehicleController();
        var result = controller.GET_Vehicles("", 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetVehiclesReservations_NullAuthorization()
    {
        var controller = new VehicleController();
        var result = controller.GET_VehiclesReservations(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetVehiclesReservations_EmptyAuthorization()
    {
        var controller = new VehicleController();
        var result = controller.GET_VehiclesReservations("", 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetVehiclesHistory_NullAuthorization()
    {
        var controller = new VehicleController();
        var result = controller.GET_VehiclesHistory(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetVehiclesHistory_EmptyAuthorization()
    {
        var controller = new VehicleController();
        var result = controller.GET_VehiclesHistory("", 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }
}

[TestClass]
public class VehicleModelTests
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<VehicleModelTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [TestMethod]
    public void ToJSON_SingleVehicle()
    {
        JObject vehicleJson = Vehicle.ToJSON(new Vehicle
        {
            ID = 1,
            UserID = 1,
            LicensePlate = "ABC-123",
            Make = "Toyota",
            Model = "Corolla",
            Color = "Blue",
            ModelYear = 2020,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = DateTime.Now
        });
        Assert.IsNotNull(vehicleJson);
    }

    [TestMethod]
    public void ToJSON_MultipleVehicles()
    {
        List<Vehicle> vehicles = new List<Vehicle>
        {
            new Vehicle
            {
                ID = 1,
                UserID = 1,
                LicensePlate = "ABC-123",
                Make = "Toyota",
                Model = "Corolla",
                Color = "Blue",
                ModelYear = 2020,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now
            },
            new Vehicle
            {
                ID = 2,
                UserID = 1,
                LicensePlate = "DEF-456",
                Make = "Honda",
                Model = "Civic",
                Color = "Red",
                ModelYear = 2019,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now
            }
        };

        JArray vehiclesJson = Vehicle.ToJSON(vehicles);
        Assert.IsNotNull(vehiclesJson);
        Assert.AreEqual(2, vehiclesJson.Count);
    }


    // [TestMethod]
    // public void GetAllVehiclesByID()
    // {
    //     Vehicle? vehicle = new Vehicle
    //     {
    //         ID = 1,
    //         UserID = 1,
    //         LicensePlate = "ABC-123",
    //         Make = "Toyota",
    //         Model = "Corolla",
    //         Color = "Blue",
    //         ModelYear = 2020,
    //         CreateDateTime = DateTime.Now,
    //         UpdateDateTime = DateTime.Now
    //     };

    //     vehicle.CreateDB();

    //     Vehicle? fetchedVehicle = Vehicle.GetByID(vehicle.ID);
    //     Assert.IsNotNull(fetchedVehicle);
    //     Assert.AreEqual(vehicle.ID, fetchedVehicle.ID);
    // }

    // [TestMethod]
    // public void GetVehiclesByUserID()
    // {
    //     Vehicle? vehicle = new Vehicle
    //     {
    //         ID = 1,
    //         UserID = 1,
    //         LicensePlate = "ABC-123",
    //         Make = "Toyota",
    //         Model = "Corolla",
    //         Color = "Blue",
    //         ModelYear = 2020,
    //         CreateDateTime = DateTime.Now,
    //         UpdateDateTime = DateTime.Now
    //     };

    //     vehicle.CreateDB();

    //     List<Vehicle> vehicles = Vehicle.GetAll();
    //     Vehicle? fetchedVehicle = vehicles.FirstOrDefault(v => v.ID == vehicle.ID);
    //     Assert.IsNotNull(fetchedVehicle);

    //     List<Vehicle> vehiclesByUserID = Vehicle.GetByUserID(1);
    //     Assert.IsNotNull(vehiclesByUserID);
    // }

    // [TestMethod]
    // public void GetAllVehicles()
    // {
    //     Vehicle vehicle= new Vehicle
    //     {
    //         ID = 1,
    //         UserID = 1,
    //         LicensePlate = "ABC-123",
    //         Make = "Toyota",
    //         Model = "Corolla",
    //         Color = "Blue",
    //         ModelYear = 2020,
    //         CreateDateTime = DateTime.Now,
    //         UpdateDateTime = DateTime.Now
    //     };

    //     vehicle.CreateDB();

    //     List<Vehicle> fetchedVehicles = Vehicle.GetAll();
    //     Assert.IsNotNull(fetchedVehicles);
    //     Assert.IsTrue(fetchedVehicles.Count > 0);
    // }
}