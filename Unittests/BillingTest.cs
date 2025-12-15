using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Tests;

[TestClass]
public sealed class BillingControllerTest
{
    // ------------------------------
    // GET
    // ------------------------------
    [TestMethod]
    public void GetBilling_NullAuthorization()
    {
        var controller = new BillingController();
        var result = controller.GET_Billing(null!);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetBilling_EmptyAuthorization()
    {
        var controller = new BillingController();
        var result = controller.GET_Billing("");

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    // deze twee nog afmaken
    [TestMethod]
    public void GetBillingForSpecificUser_NullAuthorization()
    {
        var controller = new BillingController();
        var result = controller.GET_BillingForSpecificUser(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetBillingForSpecificUser_EmptyAuthorization()
    {
        var controller = new BillingController();
        var result = controller.GET_BillingForSpecificUser("", 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

}

[TestClass]
public sealed class BillingModelTests
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<BillingModelTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [TestMethod]
    public void CreateDB_InsertsBillingSuccessfully()
    {
        Billing billing = new Billing
        {
            ID = 1,
            SessionID = 1,
            ParkingLotID = 1,
            Amount = 10,
            tHash = "beep",
            Payed = 10,
            Balance = 0,
            CompletedDateTime = DateTime.Now,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = null
        };

        bool result = billing.CreateDB();

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetAllBillings_NoException()
    {
        try
        {
            List<Billing> billings = Billing.GetAll();
            Assert.IsNotNull(billings);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during GetAll: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
        }
    }

    [TestMethod]
    public void GetBillingById_ExistingId_ReturnsBilling()
    {
        // Arrange - Ensure there is a billing with ID 1 in the database
        int testBillingId = 1;

        try
        {
            // Act
            Billing? billing = Billing.GetByID(testBillingId);

            // Assert
            Assert.IsNotNull(billing, $"Billing with ID {testBillingId} should exist.");
            Assert.AreEqual(testBillingId, billing.ID, "Returned billing ID should match the requested ID.");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during GetByID: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
        }
    }  
}