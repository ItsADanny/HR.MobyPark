using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[TestClass]
public class ReservationAuthenticationTest
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ReservationAuthenticationTest>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }
    // GET
    [TestMethod]
    public void GetWithNull_ReturnsUnauthorized()
    {
        var controller = new ReservationController();

        var result = controller.GET_Reservation(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetWithEmpty_ReturnsUnauthorized()
    {
        var controller = new ReservationController();

        var result = controller.GET_Reservation("", 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetWithValidAuth()
    {
        // eerst inserten met een token die we kennen
        var testToken = "test-token-123";
        var testUserID = 99;

        string insertSql = $"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserID}, '{testToken}', NOW())";
        DBHelper.Create(insertSql);

        // test reservation maken
        var testReservation = new Reservation
        {
            UserID = testUserID,
            ParkingLotID = 1,
            VehicleID = 1,
            StartDateTime = DateTime.Now,
            EndDateTime = DateTime.Now.AddHours(1)
        };
        testReservation.CreateDB();

        // Get the created reservation's ID
        List<Reservation> allReservations = Reservation.GetAll();
        Reservation? createdReservation = allReservations.FirstOrDefault(r => r.UserID == testUserID && r.ParkingLotID == 1 && r.VehicleID == 1);
        Assert.IsNotNull(createdReservation, "test reservation failed to create");

        var controller = new ReservationController();
        var result = controller.GET_Reservation(testToken, createdReservation.ID);
        Assert.IsFalse(result.Result is UnauthorizedResult, "GET should not return Unauthorized");

        // de ttest reservation weer verwijderen zodat de test meerdere malen kan runnen

        createdReservation.DeleteDB();

        string deleteSql = $"DELETE FROM UserSessions WHERE Token = '{testToken}'";
        bool deleted = DBHelper.Delete(deleteSql);
        Assert.IsTrue(deleted, "failed to delete test sessie");
    }


    // POST 
    [TestMethod]
    public void PostWithNull_ReturnsUnauthorized()
    {
        var controller = new ReservationController();

        var dto = new reservationDto
        (
            userID: 1,
            parkingLotID: 20,
            vehicleID: 5,
            startDateTime: DateTime.Now,
            endDateTime: DateTime.Now.AddHours(1)
        );
        var result = controller.POST_CreateReservation(null!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PostWithEmpty_ReturnsUnauthorized()
    {
        var controller = new ReservationController();

        var dto = new reservationDto
        (
            userID: 1,
            parkingLotID: 20,
            vehicleID: 5,
            startDateTime: DateTime.Now,
            endDateTime: DateTime.Now.AddHours(1)
        );
        var result = controller.POST_CreateReservation("", dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PostWithValidAuth()
    {
        // session maken
        var testToken = "test-token-123";
        var testUserID = 99;

        string insertSql = $"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserID}, '{testToken}', NOW())";
        DBHelper.Create(insertSql);

        // reservationDTO voor de post
        var controller = new ReservationController();
        var dto = new reservationDto
        (
            userID: testUserID,
            parkingLotID: 1,
            vehicleID: 1,
            startDateTime: DateTime.Now,
            endDateTime: DateTime.Now.AddHours(1)
        );
        var result = controller.POST_CreateReservation(testToken, dto);
        Assert.IsFalse(result.Result is UnauthorizedResult, "POST should not return Unauthorized");

        // session verwijderen
        string deleteSql = $"DELETE FROM UserSessions WHERE Token = '{testToken}'";
        bool deleted = DBHelper.Delete(deleteSql);
        Assert.IsTrue(deleted, "failed to delete test sessie");
    }
 
    // PUT
  

    [TestMethod]
    public void PutWithNull_ReturnsUnauthorized()
    {
        var controller = new ReservationController();
        var dto = new updateReservationDto(
            startDateTime: DateTime.Now,
            endDateTime: DateTime.Now.AddHours(3),
            status: "pending"
        );
        var result = controller.PUT_UpdateReservation(null!, 1, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PutWithEmpty_ReturnsUnauthorized()
    {
        var controller = new ReservationController();

            var dto = new updateReservationDto(
            startDateTime: DateTime.Now,
            endDateTime: DateTime.Now.AddHours(3),
            status: "pending"
        );
        var result = controller.PUT_UpdateReservation("", 1, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PutWithValidAuth()
    {
        // session maken
        var testToken = "test-token-123";
        var testUserID = 99;

        string insertSql = $"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserID}, '{testToken}', NOW())";
        DBHelper.Create(insertSql);

        // Reservation maken die we zullen updaten
        var testReservation = new Reservation
        {
            UserID = testUserID,
            ParkingLotID = 1,
            VehicleID = 1,
            StartDateTime = DateTime.Now,
            EndDateTime = DateTime.Now.AddHours(1)
        };
        testReservation.CreateDB();

        // get from database
        List<Reservation> allReservations = Reservation.GetAll();
        Reservation? createdReservation = allReservations.FirstOrDefault(r => r.UserID == testUserID && r.ParkingLotID == 1 && r.VehicleID == 1);
        Assert.IsNotNull(createdReservation, "test reservation failed to create");

        var controller = new ReservationController();
        var dto = new updateReservationDto(
            startDateTime: DateTime.Now,
            endDateTime: DateTime.Now.AddHours(3),
            status: "pending"
        );
        var result = controller.PUT_UpdateReservation(testToken, createdReservation.ID, dto);
        Assert.IsFalse(result.Result is UnauthorizedResult, "PUT should not return Unauthorized");

        // session verwijderen
        string deleteSql = $"DELETE FROM UserSessions WHERE Token = '{testToken}'";
        bool deleted = DBHelper.Delete(deleteSql);
        Assert.IsTrue(deleted, "failed to delete test sessie");
    }

    // DELETE

    [TestMethod]
    public void DeleteWithNull_ReturnsUnauthorized()
    {
        var controller = new ReservationController();

        var result = controller.DELETE_Reservation(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void DeleteWithEmpty_ReturnsUnauthorized()
    {
        var controller = new ReservationController();

        var result = controller.DELETE_Reservation("", 1);

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

        // reservation maken die we ook weer zullen verwijderen
        var testReservation = new Reservation
        {
            UserID = testUserID,
            ParkingLotID = 1,
            VehicleID = 1,
            StartDateTime = DateTime.Now,
            EndDateTime = DateTime.Now.AddHours(1)
        };
        testReservation.CreateDB();

        // Retrieve from database
        List<Reservation> allReservations = Reservation.GetAll();
        Reservation? createdReservation = allReservations.FirstOrDefault(r => r.UserID == testUserID && r.ParkingLotID == 1 && r.VehicleID == 1);
        Assert.IsNotNull(createdReservation, "test reservation failed to create");

        var controller = new ReservationController();
        var result = controller.DELETE_Reservation(testToken, createdReservation.ID);
        Assert.IsFalse(result.Result is UnauthorizedResult, "DELETE should not return Unauthorized");

        // session verwijderen
        string deleteSql = $"DELETE FROM UserSessions WHERE Token = '{testToken}'";
        bool deleted = DBHelper.Delete(deleteSql);
        Assert.IsTrue(deleted, "failed to delete test sessie");
    }
}
