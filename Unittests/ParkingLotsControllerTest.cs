using Microsoft.AspNetCore.Mvc;

[TestClass]
public class ParkingLotsControllerTest
{
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
        var dto = new parkingLotDto();

        var result = controller.POST_CreateParkingLot("", dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void createParkingLot_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var dto = new parkingLotDto();

        var result = controller.POST_CreateParkingLot(null!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
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
        var dto = new parkingLotDto();
        var result = controller.PUT_UpdateParkingLot("", 1, dto);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void updateParkingLot_nullAuthorization_fail()
    {
        var controller = new ParkingLotsController();
        var dto = new parkingLotDto();
        var result = controller.PUT_UpdateParkingLot(null!, 1, dto);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
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
}
