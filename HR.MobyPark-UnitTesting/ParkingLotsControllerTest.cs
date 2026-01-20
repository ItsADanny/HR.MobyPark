using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[TestClass]
public class ParkingLotsControllerTest
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ParkingLotsControllerTest>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }
    [TestMethod]
    public void getAll_noAuthorization_fail()
    {
        var controller = new ParkingLotsController();

        var result = controller.GET_AllParkingLots("");

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void getAll_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();

        var result = controller.GET_AllParkingLots(null!);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void get_withValidAuthorization_pass()
    {
        var testToken = "parkinglot-test-token";
        var testUserID = 99;

        string insertSessionSql =$"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserID}, '{testToken}', NOW())";
        DBHelper.Create(insertSessionSql);

        var controller = new ParkingLotsController();

        var result = controller.GET_AllParkingLots(testToken);

        Assert.IsFalse(result.Result is UnauthorizedResult, "GET should not return Unauthorized");

        string deleteSessionSql = $"DELETE FROM UserSessions WHERE Token = '{testToken}'";
        DBHelper.Delete(deleteSessionSql);
    }


    [TestMethod]
    public void getById_emptyAuthorization_fail()
    {
        var controller = new ParkingLotsController();

        var result = controller.GET_ParkingLot("", 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void getById_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();

        var result = controller.GET_ParkingLot(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void createParkingLot_emptyAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var dto = new parkingLotDto
        (
            name: "Lot1",
            location: "abc",
            address: "bca",
            capacity: 200,
            tariff: 1.5m,
            dayTariff: 7.0m,
            lat: 2.1,
            lon: 4.3 
        );

        var result = controller.POST_CreateParkingLot("", dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void createParkingLot_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var dto = new parkingLotDto
        (
            name: "Lot1",
            location: "abc",
            address: "bca",
            capacity: 200,
            tariff: 1.5m,
            dayTariff: 7.0m,
            lat: 2.1,
            lon: 4.3 
        );
        var result = controller.POST_CreateParkingLot(null!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

        [TestMethod]
    public void createParkingLot_withValidAuthorization_pass()
    {
        var testToken = "test-token-post";
        var testUserID = 75;

        string insertSessionSql =$"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserID}, '{testToken}', NOW())";
        DBHelper.Create(insertSessionSql);

        var controller = new ParkingLotsController();

        var dto = new parkingLotDto
        (
            name: "auth lot",
            location: "auth location",
            address: "auth address",
            capacity: 100,
            tariff: 2.0m,
            dayTariff: 10.0m,
            lat: 52.1,
            lon: 4.3
        );

        var result = controller.POST_CreateParkingLot(testToken, dto);

        Assert.IsFalse(result.Result is UnauthorizedResult, "POST should not return Unauthorized");

        var allLots = ParkingLot.GetAll();
        var createdLot = allLots.FirstOrDefault(l => l.Name == "auth lot");
        createdLot?.DeleteDB();

        string deleteSessionSql = $"DELETE FROM UserSessions WHERE Token = '{testToken}'";
        DBHelper.Delete(deleteSessionSql);
    }


    [TestMethod]
    public void deleteParkingLot_emptyAuthorization_fail()
    {
        var controller = new ParkingLotsController();

        var result = controller.DELETE_ParkingLot("", 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void deleteParkingLot_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();

        var result = controller.DELETE_ParkingLot(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void getParkingLotSessions_emptyAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var result = controller.GET_ParkingLotSessions("", 1);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void getParkingLotSessions_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var result = controller.GET_ParkingLotSessions(null!, 1);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void updateParkingLot_emptyAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var dto = new parkingLotDto
        (
            name: "Lot1",
            location: "abc",
            address: "bca",
            capacity: 200,
            tariff: 1.5m,
            dayTariff: 7.0m,
            lat: 2.1,
            lon: 4.3 
        );
        var result = controller.PUT_UpdateParkingLot("", 1, dto);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void updateParkingLot_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var dto = new parkingLotDto
        (
            name: "Lot1",
            location: "abc",
            address: "bca",
            capacity: 200,
            tariff: 1.5m,
            dayTariff: 7.0m,
            lat: 2.1,
            lon: 4.3 
        );
        var result = controller.PUT_UpdateParkingLot(null!, 1, dto);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void update_withValidAuthorization_pass()
    {
        var testToken = "parkinglot-test-token";
        var testUserID = 31;

        DBHelper.Create($"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserID}, '{testToken}', NOW())");

        var lot = new ParkingLot
        {
            Name = "test update",
            Location = "loc",
            Address = "addr",
            Capacity = 50,
            Tariff = 2.5m,
            DayTariff = 15m,
            Latitude = 52.2,
            Longitude = 4.4,
            CreateDateTime = DateTime.Now
        };
        lot.CreateDB();

        var controller = new ParkingLotsController();

        var dto = new parkingLotDto
        (
            name: "update lot",
            location: "new loc",
            address: "new addr",
            capacity: 60,
            tariff: 3.0m,
            dayTariff: 18m,
            lat: 52.3,
            lon: 4.5
        );

        var result = controller.PUT_UpdateParkingLot(testToken, lot.ID, dto);

        Assert.IsFalse(result.Result is UnauthorizedResult, "PUT should not return Unauthorized");

        lot.DeleteDB();
        DBHelper.Delete($"DELETE FROM UserSessions WHERE Token = '{testToken}'");
    }


    [TestMethod]
    public void deleteParkingLotSessions_emptyAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var result = controller.DELETE_ParkingLotSessions("", 1);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void deleteParkingLotSessions_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var result = controller.DELETE_ParkingLotSessions(null!, 1);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void deleteParkingLotSession_emptyAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var result = controller.DELETE_ParkingLotSession("", 1, 1);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void deleteParkingLotSession_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var result = controller.DELETE_ParkingLotSession(null!, 1, 1);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void delete_withValidAuthorization_pass()
    {
        var testToken = "parkinglot-delete-token";
        var testUserID = 46;

        string insertSessionSql =$"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({testUserID}, '{testToken}', NOW())";
        DBHelper.Create(insertSessionSql);

        var testLot = new ParkingLot
        {
            Name = "del test",
            Location = "del loc",
            Address = "del addr",
            Capacity = 100,
            Tariff = 2.0m,
            DayTariff = 10.0m,
            Latitude = 52.1,
            Longitude = 4.3,
            CreateDateTime = DateTime.Now
        };
        testLot.CreateDB();

        List<ParkingLot> allLots = ParkingLot.GetAll();
        ParkingLot? createdLot = allLots.FirstOrDefault(l => l.Name == "del test" && l.Location == "del loc");

        Assert.IsNotNull(createdLot, "Test lot failed to create");

        var controller = new ParkingLotsController();

        var result = controller.DELETE_ParkingLot(testToken, createdLot.ID);

        Assert.IsFalse(result.Result is UnauthorizedResult, "DELETE should not return Unauthorized");

        DBHelper.Delete($"DELETE FROM UserSessions WHERE Token = '{testToken}'");
    }

}
