using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

[TestClass]
[DoNotParallelize]
public sealed class PaymentsControllerIntegrationTests
{
    private static string? _validAuthToken;
    private static int _testPaymentId = 0;
    private static int _testUserID = 1; // Use existing user ID with GlobalPayments permission
    private static int _testParkingLotID = 1; // Use existing parking lot
    private static int _testVehicleID = 1; // Use existing vehicle

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PaymentsControllerIntegrationTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();

        _validAuthToken = "test-payment-integration-token";
        
        // Create UserSession for existing user
        string insertSessionSql = $"INSERT INTO UserSessions (UserID, Token, CreateDateTime) VALUES ({_testUserID}, '{_validAuthToken}', NOW())";
        DBHelper.Create(insertSessionSql);

        // Create parking Session using existing user, vehicle, and parking lot
        string insertParkingSql = $"INSERT INTO Sessions (ParkingLotId, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus, CreateDateTime) VALUES ({_testParkingLotID}, 'TEST-PAY-INTEGRATION', {_testVehicleID}, {_testUserID}, NOW(), DATE_ADD(NOW(), INTERVAL 1 HOUR), 60, 10.00, 'unpaid', NOW())";
        DBHelper.Create(insertParkingSql);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Clean up test payment if it was created
        if (_testPaymentId > 0)
        {
            DBHelper.Delete($"DELETE FROM Transaction WHERE ID = {_testPaymentId}");
        }

        // Clean up in reverse order of creation
        DBHelper.Delete($"DELETE FROM Sessions WHERE UserID = {_testUserID} AND LicensePlate = 'TEST-PAY-INTEGRATION'");
        DBHelper.Delete($"DELETE FROM UserSessions WHERE Token = '{_validAuthToken}'");
    }

//     // CREATE Tests
//     [TestMethod]

    // CREATE Tests - POST Payment
    [TestMethod]
    [Priority(0)]
    public void PostCreatePayment_ValidData_ReturnsCreated()
    {
        var controller = new PaymentsController();
        var dto = new createPaymentDto("TEST-INTEGRATION-TXN", 25.50m);
        var result = controller.POST_CreatePayment(_validAuthToken!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));

        // Store payment ID for cleanup and later tests
        var payments = Payment.GetAllByUserID(_testUserID);
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

    [TestMethod]
    [Priority(2)]
    public void PostCreatePayment_NoAuth_ReturnsUnauthorized()
    {
        var controller = new PaymentsController();
        var dto = new createPaymentDto("TEST", 25.50m);
        var result = controller.POST_CreatePayment(null!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    // CREATE Tests - POST Refund
    [TestMethod]
    [Priority(3)]
    public void PostCreatePaymentRefund_ValidAmount_ReturnsCreated()
    {
        var controller = new PaymentsController();
        decimal refundAmount = 15.00m;
        var result = controller.POST_CreatePaymentRefund(_validAuthToken!, refundAmount);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
    }

    [TestMethod]
    [Priority(4)]
    public void PostCreatePaymentRefund_InvalidAmount_ReturnsBadRequest()
    {
        var controller = new PaymentsController();
        decimal invalidAmount = -5.00m;
        var result = controller.POST_CreatePaymentRefund(_validAuthToken!, invalidAmount);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    // READ/GET Tests - Get payment by ID
    [TestMethod]
    [Priority(5)]
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
    [Priority(6)]
    public void GetPaymentForId_InvalidId_ReturnsNotFound()
    {
        var controller = new PaymentsController();
        var result = controller.GET_PaymentForID(_validAuthToken!, 999999);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    [Priority(7)]
    public void GetPaymentForId_NoAuth_ReturnsUnauthorized()
    {
        var controller = new PaymentsController();
        var result = controller.GET_PaymentForID(null!, 1);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    // READ/GET Tests - Get all payments for user
    [TestMethod]
    [Priority(8)]
    public void GetPayments_ValidAuthorization_ReturnsOk()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Payments(_validAuthToken!);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    [Priority(9)]
    public void GetPayments_InvalidAuthorization_ReturnsNotFound()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Payments("invalid-token-xyz");

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    [Priority(10)]
    public void GetPayments_NoAuth_ReturnsUnauthorized()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Payments(null!);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    // UPDATE/PUT Tests
    [TestMethod]
    [Priority(11)]
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

    [TestMethod]
    [Priority(12)]
    public void PutUpdatePayment_InvalidData_ReturnsBadRequest()
    {
        var controller = new PaymentsController();
        var dto = new paymentDto(
            -10.00m, // Invalid amount
            DateOnly.FromDateTime(DateTime.Now),
            "CreditCard",
            "Visa",
            "TestBank",
            1,
            1
        );
        var result = controller.PUT_UpdatePayment(_validAuthToken!, 1, dto);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    [Priority(13)]
    public void PutUpdatePayment_NoAuth_ReturnsUnauthorized()
    {
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
        var result = controller.PUT_UpdatePayment(null!, 1, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }
}


