using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Tests;

[TestClass]
public sealed class PaymentsControllerTests
{
    [TestMethod]
    public void GetPayments_NullAuthorization()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Payments(null!);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetPayments_EmptyAuthorization()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Payments("");
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetPaymentForId_NullAuthorization()
    {
        var controller = new PaymentsController();
        var result = controller.GET_PaymentForID(null!, 1);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PostCreatePayment_NullAuthorization()
    {
        var controller = new PaymentsController();
        var dto = new createPaymentDto("ABC-001", 10.00m);
        var result = controller.POST_CreatePayment(null!, dto);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }
}

[TestClass]
public sealed class PaymentModelTests
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
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
    public void GetToJSON_NoException()
    {
        try
        {
            Payment payment = new Payment
            {
                ID = 1,
                TransactionID = "TX123",
                ParkingLotID = 2,
                SessionID = 3,
                UserID = 4,
                Amount = 20.50m,
                Date = DateTime.Now,
                Method = "CreditCard",
                Issuer = "Visa",
                Bank = "BankName",
                Hash = "SomeHash",
                CompletedDateTime = DateTime.Now,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = null
            };

            var json = Payment.ToJSON(payment);
            Assert.IsNotNull(json);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during ToJSON: {ex.Message}");
        }
    }

    [TestMethod]
    public void CreateDB_InsertsPaymentSuccessfully()
    {
        // Arrange - Use unique values to avoid conflicts
        Payment payment = new Payment
        {
            TransactionID = $"TEST1",
            ParkingLotID = 1,
            SessionID = 1,
            UserID = 1,
            Amount = 25.75m,
            Date = DateTime.Now,
            Method = "CreditCard",
            Issuer = "Visa",
            Bank = "TestBank",
            Hash = $"HASH",
            CompletedDateTime = DateTime.Now,
            CreateDateTime = DateTime.Now,
            UpdateDateTime = null
        };

        // Act
        bool result = payment.CreateDB();

        // Assert
        Assert.IsTrue(result, "CreateDB should return true when payment is created successfully");
    }

    [TestMethod]
    public void GetAllPayments_NoException()
    {
        try
        {
            List<Payment> payments = Payment.GetAll();
            Assert.IsNotNull(payments);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during GetAll: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
        }
    }
}
