using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

[TestClass]
public sealed class ParkingLotModelTests
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ParkingLotModelTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [TestMethod]
    public void ToJSON_singleParkingLot()
    {
        var lot = new ParkingLot
        {
            ID = 1,
            Name = "lot1",
            Location = "abc",
            Address = "orange",
            Capacity = 100,
            Reserved = 20,
            Tariff = 2.5m,
            DayTariff = 15m,
            Latitude = 52.1,
            Longitude = 4.3,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = null
        };

        JObject json = ParkingLot.ToJSON(lot);
        Assert.IsNotNull(json);
        Assert.AreEqual(lot.ID, json.Value<int>("id"));
        Assert.AreEqual(lot.Capacity, json.Value<int>("capacity"));
        Assert.AreEqual(lot.Reserved, json.Value<int>("reserved"));
    }

    [TestMethod]
    public void ToJSON_listOfParkingLots()
    {
        var lots = new List<ParkingLot>
        {
            new ParkingLot { ID = 1, Name = "LotA", Capacity = 50, Reserved = 10, CreateDateTime = DateTime.Now },
            new ParkingLot { ID = 2, Name = "LotB", Capacity = 100, Reserved = 20, CreateDateTime = DateTime.Now }
        };

        JArray arr = ParkingLot.ToJSON(lots);
        Assert.IsNotNull(arr);
        Assert.AreEqual(2, arr.Count);
    }

    [TestMethod]
    public void CreateDB_insertsParkingLot()
    {
        var lot = new ParkingLot
        {
            Name = $"Test Lot",
            Location = "Test Location",
            Address = "Test Address",
            Capacity = 50,
            Tariff = 3.0m,
            DayTariff = 20m,
            Latitude = 52.2,
            Longitude = 4.4,
            CreateDateTime = DateTime.Now
        };

        bool result = lot.CreateDB();
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void UpdateDB_updatesParkingLot()
    {
        var lot = new ParkingLot
        {
            Name = $"Lot2",
            Location = "Test Location2",
            Address = "Test Address2",
            Capacity = 50,
            Tariff = 3.0m,
            DayTariff = 20m,
            Latitude = 52.2,
            Longitude = 4.4,
            CreateDateTime = DateTime.Now
        };

        lot.CreateDB();

        lot.Name = "Updated Lot Name";
        lot.Capacity = 60;
        lot.UpdateDateTime = DateTime.Now;

        bool result = lot.UpdateDB();
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void DeleteDB_deletesParkingLot()
    {
        var lot = new ParkingLot
        {
            Name = $"Lot3",
            Location = "Test Location3",
            Address = "Test Address3",
            Capacity = 50,
            Tariff = 3.0m,
            DayTariff = 20m,
            Latitude = 52.2,
            Longitude = 4.4,
            CreateDateTime = DateTime.Now
        };

        lot.CreateDB();
        bool result = lot.DeleteDB();
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetByID_returnsParkingLot()
    {
        //assuming lot with id exists in the database
        int existingLotId = 1;

        ParkingLot? testLot = ParkingLot.GetByID(existingLotId);
        Assert.IsNotNull(testLot);
        Assert.AreEqual(existingLotId, testLot.ID);
    }

    [TestMethod]
    public void GetAll_returnsListOfParkingLots()
    {
        List<ParkingLot> lots = ParkingLot.GetAll();
        Assert.IsNotNull(lots);
        Assert.IsTrue(lots.Count > 0);
    }

    [TestMethod]
    public void HasFreeSpots_returnsCorrectly()
    {
        var lot = new ParkingLot { Capacity = 100, Reserved = 50 };
        Assert.IsTrue(lot.HasFreeSpots());

        lot.Reserved = 100;
        Assert.IsFalse(lot.HasFreeSpots());
    }

    [TestMethod]
    public void GetAvailableSpots_returnsCorrectCount()
    {
        var lot = new ParkingLot { Capacity = 100, Reserved = 30 };
        Assert.AreEqual(70, lot.GetAvailableSpots());
    }

    [TestMethod]
    public void IsFull_returnsCorrectly()
    {
        var lot = new ParkingLot { Capacity = 100, Reserved = 100 };
        Assert.IsTrue(lot.IsFull());

        lot.Reserved = 99;
        Assert.IsFalse(lot.IsFull());
    }
}
