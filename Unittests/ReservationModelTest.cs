using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;

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
            // database credentials laden vanuit user secrets
            .AddUserSecrets("HR.MobyPark")
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
            StartDateTime = GeneralMethods.ParseDBDateTime("07/12/2025 10:00:00"),
            EndDateTime = GeneralMethods.ParseDBDateTime("07/12/2025 11:00:00"),
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
            StartDateTime = GeneralMethods.ParseDBDateTime("08/12/2025 09:00:00"),
            EndDateTime = GeneralMethods.ParseDBDateTime("08/12/2025 10:00:00"),
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

    [TestMethod]
    public void DeleteDB_DeletesReservationSuccessfully()
    {
        // First create a reservation to delete
        Reservation reservation = new Reservation
        {
            UserID = 97,
            ParkingLotID = 1,
            VehicleID = 1,
            StartDateTime = GeneralMethods.ParseDBDateTime("09/12/2025 14:00:00"),
            EndDateTime = GeneralMethods.ParseDBDateTime("09/12/2025 15:00:00"),
            Status = "active",
            Amount = 45.00m,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = DateTime.Now
        };

        // Insert it first
        reservation.CreateDB();

        // Then delete it
        bool result = reservation.DeleteDB();

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetAll_ReturnsListOfReservations()
    {
        // First create a reservation
        Reservation reservation = new Reservation
        {
            UserID = 95,
            ParkingLotID = 2,
            VehicleID = 5,
            StartDateTime = GeneralMethods.ParseDBDateTime("11/12/2025 12:00:00"),
            EndDateTime = GeneralMethods.ParseDBDateTime("11/12/2025 13:00:00"),
            Status = "active",
            Amount = 65.00m,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = DateTime.Now
        };

        reservation.CreateDB();

        // Alle reservations krijgen
        List<Reservation> reservations = Reservation.GetAll();

        // Check dat de lijst niet null is en reservering heeft
        Assert.IsNotNull(reservations);
        Assert.IsTrue(reservations.Count > 0);
        
        // check dat de gemaakte reservering in de lijst is,
        bool found = reservations.Any(r => r.UserID == 95 && r.ParkingLotID == 2 && r.VehicleID == 5);
        Assert.IsTrue(found);
    }

    [TestMethod]
    public void GetAllByParkingLot_ReturnsReservationsForParkingLot()
    {
        // reservation maken voor parking lot 7
        Reservation reservation = new Reservation
        {
            UserID = 94,
            ParkingLotID = 7,
            VehicleID = 6,
            StartDateTime = GeneralMethods.ParseDBDateTime("12/12/2025 14:00:00"),
            EndDateTime = GeneralMethods.ParseDBDateTime("12/12/2025 15:00:00"),
            Status = "active",
            Amount = 70.00m,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = DateTime.Now
        };

        reservation.CreateDB();

        // alle reservations voor parking lot 7 krijgen
        List<Reservation> reservations = Reservation.GetAllByParkingLot(7);

        // Check dat de lijst niet null is
        Assert.IsNotNull(reservations);

        // Check dat de  reservering in de lijst is,
        bool found = reservations.Any(r => r.UserID == 94 && r.ParkingLotID == 7 && r.VehicleID == 6);
        Assert.IsTrue(found);

        // Verify all returned reservations are for parking lot 7
        bool allGood = reservations.All(r => r.ParkingLotID == 7);
        Assert.IsTrue(allGood);
    }

    [TestMethod]
    public void GetByID_ReturnsReservationWhenExists()
    {
        //reservation met hardcoded ID make
        Reservation reservation = new Reservation
        {
            ID = 9999,  
            UserID = 93,
            ParkingLotID = 4,
            VehicleID = 7,
            StartDateTime = GeneralMethods.ParseDBDateTime("13/12/2025 16:00:00"),
            EndDateTime = GeneralMethods.ParseDBDateTime("13/12/2025 17:00:00"),
            Status = "active",
            Amount = 80.00m,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = DateTime.Now
        };

        reservation.CreateDB();

        Reservation? retrieved = Reservation.GetByID(9999);

        Assert.IsNotNull(retrieved);
        Assert.AreEqual(9999, retrieved.ID);

    }
}
