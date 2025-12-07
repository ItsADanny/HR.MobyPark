using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

[TestClass]
public sealed class ReservationModelTests
{
    [TestMethod]
    public void ToJSON_SingleReservation()
    {
        try
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
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during Reservation.ToJSON: {ex.Message}");
        }
    }

    [TestMethod]
    public void ToJSON_ListOfReservations()
    {
        try
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
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during Reservation.ToJSON list: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
        }
    }

    [TestMethod]
    public void ToInsertSQL_ContainsInsertKeyword()
    {
        Reservation r = new Reservation
        {
            UserID = 5,
            ParkingLotID = 2,
            VehicleID = 3,
            StartDateTime = DateTime.Now,
            EndDateTime = DateTime.Now.AddHours(1)
        };

        string sql = r.ToInsertSQL();
        Assert.IsFalse(string.IsNullOrEmpty(sql));
        StringAssert.Contains(sql, "INSERT INTO Reservations");
    }
}
