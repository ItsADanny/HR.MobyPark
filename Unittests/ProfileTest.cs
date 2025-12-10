[TestClass]
public sealed class ProfileControllerTests
{
    // ------------------------------
    // GET
    // ------------------------------
    [TestMethod]
    public void GetProfile_NullAuthorization()
    {
        var controller = new ProfileController();
        var result = controller.GETProfile(null!);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetProfile_EmptyAuthorization()
    {
        var controller = new BillingController();
        var result = controller.GETProfile("");

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    // ------------------------------
    // PUT
    // ------------------------------
    [TestMethod]
    public void PutProfile_NullAuthorization()
    {
        var controller = new ProfileController();
        var dto = new registerDto();

        var result = controller.PUTProfile(null!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PutProfile_EmptyAuthorization()
    {
        var controller = new ProfileController();
        var dto = new registerDto();

        var result = controller.PUTProfile("", dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }
}