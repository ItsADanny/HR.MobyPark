namespace MyProject.Tests;

[TestClass]
public sealed class BillingControllerTest
{
    // Controller not yet made
}

public sealed class BillingModelTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        // Load configuration from backend's appsettings.Development.json
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\..\HR.MobyPark\HR.MobyPark"));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json")
            .Build();

        // Initialize DBHelper with configuration from backend
        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [TestMethod]
    public void CreateDB_InsertsBillingSuccessfully()
    {
        Billing billing = new Billing
        {
            BillingId = 1,
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