using Microsoft.AspNetCore.Mvc;

[TestClass]
public sealed class ReservationControllerTests
{
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

    // [TestMethod]
    // public void PostCreateReservation_NullDto_ReturnsBadRequest()
    // {
    //     var controller = new ReservationController();

    //     // Here we simulate invalid model binding (null body)
    //     var result = controller.POST_CreateReservation("auth", null!);

    //     Assert.IsInstanceOfType(result.Result, typeof(BadRequestResult));
    // }

 
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
}
