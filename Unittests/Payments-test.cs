using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Tests;

// [TestClass]
// public sealed class PaymentsControllerTests
// {
//     [TestMethod]
//     public void GetPayments_NullAuthorization()
//     {
//         var controller = new PaymentsController();
//         var result = controller.GET_Payments(null!);
//         Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
//     }

//     [TestMethod]
//     public void GetPayments_EmptyAuthorization()
//     {
//         var controller = new PaymentsController();
//         var result = controller.GET_Payments("");
//         Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
//     }

//     [TestMethod]
//     public void GetPaymentForId_NullAuthorization()
//     {
//         var controller = new PaymentsController();
//         var result = controller.GET_PaymentForID(null!, 1);
//         Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
//     }

//     [TestMethod]
//     public void PostCreatePayment_NullAuthorization()
//     {
//         var controller = new PaymentsController();
//         var dto = new createPaymentDto("ABC-001", 10.00m);
//         var result = controller.POST_CreatePayment(null!, dto);
//         Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
//     }
// }

[TestClass]
[DoNotParallelize]
public sealed class PaymentsControllerIntegrationTests
{
    private static string? _validAuthToken;
    private static int _testPaymentId = 0;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PaymentsControllerIntegrationTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();


        _validAuthToken = ???;
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Clean up the test session
        if (!string.IsNullOrEmpty(_validAuthToken))
        {
            var session = UserSession.GetUserSession(_validAuthToken);
            session?.DeleteDB();
        }

        // Clean up test payment if it was created
        if (_testPaymentId > 0)
        {
            var payment = Payment.GetByID(_testPaymentId);
            payment?.DeleteDB();
        }
    }

    // CREATE Tests
    [TestMethod]
    [Priority(0)]
    public void PostCreatePayment_ValidData_ReturnsCreated()
    {
        var controller = new PaymentsController();
        var dto = new createPaymentDto($"TEST", 25.50m);
        var result = controller.POST_CreatePayment(_validAuthToken!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));

        // Store payment ID for cleanup and later tests
        var payments = Payment.GetAllByUserID(1);
        var createdPayment = payments.FirstOrDefault(p => p.TransactionID == dto.transaction);
        if (createdPayment != null)
        {
            _testPaymentId = createdPayment.ID;
        }
    }

    [TestMethod]
    [Priority(1)]
    public void PostCreatePayment_InvalidAmount_ReturnsBadRequest()
    {
        var controller = new PaymentsController();
        var dto = new createPaymentDto("TEST-INVALID", -10.00m);
        var result = controller.POST_CreatePayment(_validAuthToken!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    // READ/GET Tests
    [TestMethod]
    [Priority(2)]
    public void GetPaymentForId_ValidId_ReturnsOk()
    {
        if (_testPaymentId == 0)
        {
            Assert.Inconclusive("Test payment was not created, skipping this test");
            return;
        }

        var controller = new PaymentsController();
        var result = controller.GET_PaymentForID(_validAuthToken!, _testPaymentId);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    [Priority(3)]
    public void GetPaymentForId_InvalidId_ReturnsNotFound()
    {
        var controller = new PaymentsController();
        var result = controller.GET_PaymentForID(_validAuthToken!, 999999);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    [Priority(4)]
    public void GetPayments_ValidAuthorization_ReturnsOk()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Payments(_validAuthToken!);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    [Priority(5)]
    public void GetPayments_InvalidAuthorization_ReturnsNotFound()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Payments("invalid-token-12345");

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    // UPDATE/CHANGE Tests
    [TestMethod]
    [Priority(6)]
    public void PutUpdatePayment_ValidData_ReturnsOk()
    {
        if (_testPaymentId == 0)
        {
            Assert.Inconclusive("Test payment was not created, skipping this test");
            return;
        }

        var controller = new PaymentsController();
        var dto = new paymentDto(
            50.00m,
            DateOnly.FromDateTime(DateTime.Now),
            "CreditCard",
            "Visa",
            "TestBank",
            1,
            1
        );
        var result = controller.PUT_UpdatePayment(_validAuthToken!, _testPaymentId, dto);

        Assert.IsInstanceOfType(result.Result, typeof(OkResult));
    }

    // Additional CREATE Tests (Refund)
    [TestMethod]
    [Priority(7)]
    public void PostCreatePaymentRefund_ValidAmount_ReturnsCreated()
    {
        var controller = new PaymentsController();
        decimal refundAmount = 15.00m;
        var result = controller.POST_CreatePaymentRefund(_validAuthToken!, refundAmount);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
    }

    [TestMethod]
    [Priority(8)]
    public void PostCreatePaymentRefund_InvalidAmount_ReturnsBadRequest()
    {
        var controller = new PaymentsController();
        decimal invalidAmount = -5.00m;
        var result = controller.POST_CreatePaymentRefund(_validAuthToken!, invalidAmount);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    // DELETE Tests
    [TestMethod]
    [Priority(9)]
    public void DeletePayment_ValidId_Success()
    {
        if (_testPaymentId == 0)
        {
            Assert.Inconclusive("Test payment was not created, skipping this test");
            return;
        }

        var payment = Payment.GetByID(_testPaymentId);
        Assert.IsNotNull(payment, "Payment should exist before deletion");

        bool result = payment.DeleteDB();
        Assert.IsTrue(result, "DeleteDB should return true when payment is deleted successfully");

        // Verify deletion
        var deletedPayment = Payment.GetByID(_testPaymentId);
        Assert.IsNull(deletedPayment, "Payment should not exist after deletion");

        // Reset ID so cleanup doesn't try to delete again
        _testPaymentId = 0;
    }
}

[TestClass]
[DoNotParallelize]
public sealed class PaymentModelTests
{
    // Static variable to store the created payment ID for use across tests
    private static int _createdPaymentId = 0;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PaymentModelTests>()
            .Build();

        // Initialize DBHelper with configuration from backend
        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [TestMethod]
    [Priority(0)]
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
    [Priority(1)]
    public void CreateDB_InsertsPaymentSuccessfully()
    {
        // Arrange - Use unique values to avoid conflicts
        Payment payment = new Payment
        {
            TransactionID = $"TEST",
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

        // Store the created payment ID for other tests to use
        List<Payment> allPayments = Payment.GetAll();
        Payment? createdPayment = allPayments.FirstOrDefault(p => p.TransactionID == payment.TransactionID);
        if (createdPayment != null)
        {
            _createdPaymentId = createdPayment.ID;
        }
    }

    [TestMethod]
    [Priority(2)]
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

    [TestMethod]
    [Priority(3)]
    public void GetPaymentById_ExistingId_ReturnsPayment()
    {
        try
        {
            // Act
            Payment? payment = Payment.GetByID(_createdPaymentId);

            // Assert
            Assert.IsNotNull(payment, $"Payment with ID {_createdPaymentId} should exist.");
            Assert.AreEqual(_createdPaymentId, payment.ID, "Returned payment ID should match the requested ID.");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during GetByID: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
        }
    }

    [TestMethod]
    [Priority(4)]
    public void GetAllByUserID_NoException()
    {
        try
        {
            int testUserId = 1;
            List<Payment> payments = Payment.GetAllByUserID(testUserId);
            Assert.IsNotNull(payments);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during GetAllByUserID: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
        }
    }

    [TestMethod]
    [Priority(5)]
    public void UpdateDB_NoException()
    {
        try
        {
            Payment payment = new Payment
            {
                ID = _createdPaymentId,
                TransactionID = "Test18",
                ParkingLotID = 2,
                SessionID = 3,
                UserID = 4,
                Amount = 50.50m,
                Date = DateTime.Now,
                Method = "CreditCard",
                Issuer = "Mastercard",
                Bank = "BankName",
                Hash = "SomeHash",
                CompletedDateTime = DateTime.Now,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = null
            };
            
            bool result = payment.UpdateDB();
            Assert.IsTrue(result, "UpdateDB should return true when payment is updated successfully");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during GetAllBySessionID: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
        }
    }

    [TestMethod]
    [Priority(6)]
    public void DeleteDB_NoException()
    {
        try
        {
            Payment payment = new Payment
            {
                ID = _createdPaymentId
            };
            
            bool result = payment.DeleteDB();
            Assert.IsTrue(result, "DeleteDB should return true when payment is deleted successfully");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during DeleteDB: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
        }
    }

}
