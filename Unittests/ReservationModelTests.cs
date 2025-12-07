using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

[TestClass]
public sealed class ReservationModelTests
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            "..", "..", "..", "..", "..", "HR.MobyPark", "HR.MobyPark"));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var dbSection = configuration.GetSection("Database");
        if (dbSection.Exists())
        {
            DBHelper.dBConfig = dbSection.Get<DBConfig>();
        }
    }

    [TestMethod]
    public void ToJSON_SingleReservation()
    {
        var reservation = new Reservation
        {
            ID = 42,
            UserID = 10,
            ParkingLotID = 2,
            VehicleID = 3,
            StartDateTime = DateTime.Parse("2025-12-05T08:00:00"),
            EndDateTime = DateTime.Parse("2025-12-05T09:00:00"),
            Status = "active",
            Amount = 15.50m,
            CreateDateTime = DateTime.Parse("2025-12-05T07:00:00"),
            UpdateDateTime = DateTime.Parse("2025-12-05T07:30:00")
        };

        JObject json = Reservation.ToJSON(reservation);
        Assert.IsNotNull(json);
        Assert.AreEqual(reservation.ID, json.Value<int>("id"));
        Assert.AreEqual(reservation.UserID, json.Value<int>("userID"));
        Assert.AreEqual(reservation.Amount, json.Value<decimal>("Amount"));
    }

    [TestMethod]
    public void ToJSON_ListOfReservations()
    {

        var reservations = new List<Reservation>
        {
            new Reservation { ID = 1, UserID = 10, ParkingLotID = 2, VehicleID = 3, StartDateTime = DateTime.Now, EndDateTime = DateTime.Now.AddHours(1), Amount = 10.0m, CreateDateTime = DateTime.Now, UpdateDateTime = DateTime.Now },
            new Reservation { ID = 2, UserID = 11, ParkingLotID = 2, VehicleID = 4, StartDateTime = DateTime.Now, EndDateTime = DateTime.Now.AddHours(2), Amount = 20.0m, CreateDateTime = DateTime.Now, UpdateDateTime = DateTime.Now }
        };

        JArray arr = Reservation.ToJSON(reservations);
        Assert.IsNotNull(arr);
        Assert.AreEqual(2, arr.Count);
    }

    [TestMethod]
    public void CreateDB_InsertsReservationSuccessfully()
    {
        Reservation reservation = new Reservation
        {
            UserID = 99,
            ParkingLotID = 1,
            VehicleID = 1,
            StartDateTime = DateTime.Parse("2025-12-07T10:00:00"),
            EndDateTime = DateTime.Parse("2025-12-07T11:00:00"),
            Status = "active",
            Amount = 50.00m,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = DateTime.Now
        };

        bool result = reservation.CreateDB();

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void UpdateDB_UpdatesReservationSuccessfully()
    {
        Reservation reservation = new Reservation
        {
            UserID = 98,
            ParkingLotID = 2,
            VehicleID = 2,
            StartDateTime = DateTime.Parse("2025-12-08T09:00:00"),
            EndDateTime = DateTime.Parse("2025-12-08T10:00:00"),
            Status = "active",
            Amount = 60.00m,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = DateTime.Now
        };

        // nieuwe reservation maken zodat we deze zometeen kunnen wijzigen
        reservation.CreateDB();

        // update de net gemaakte reservation. 
        reservation.Amount = 75.00m;
        reservation.Status = "completed";
        reservation.UpdateDateTime = DateTime.Now;
        
        bool result = reservation.UpdateDB();

        Assert.IsTrue(result);
    }
}
