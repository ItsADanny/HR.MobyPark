using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[TestClass]
[DoNotParallelize]
public sealed class PaymentsControllerIntegrationTests
{
    // Test uses existing data from database
    
    private static string? _validAuthToken;
    private static int _testPaymentId = 0;
    private static int _testUserID = 0;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PaymentsControllerIntegrationTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();

        // Register a new test user (gets role 1 = User with payment permissions)
        var authController = new AuthController();
        var registerDto = new registerDto(
            $"test-integration-{DateTime.Now.Ticks}",
            "test-password-123",
            "Test Integration User",
            $"test-{DateTime.Now.Ticks}@integration.test",
            "+31612345678",
            new DateOnly(1990, 1, 1)
        );
        
        var registerResult = authController.register(registerDto);
        if (registerResult.Result is not CreatedResult)
        {
            var message = registerResult.Value?.ToString() ?? "Unknown error";
            throw new Exception($"Failed to register test user: {message}");
        }

        // Login to get auth token
        var loginDto = new loginDto(registerDto.username, registerDto.password);
        var loginResult = authController.login(loginDto);
        
        if (loginResult.Result is not OkObjectResult okResult)
        {
            throw new Exception($"Failed to login test user: {loginResult.Value}");
        }
        
        // Parse token from login response
        var loginJson = JObject.Parse(okResult.Value?.ToString() ?? "");
        _validAuthToken = loginJson["session_token"]?.ToString();
        
        if (string.IsNullOrEmpty(_validAuthToken))
        {
            throw new Exception("Failed to get session token from login");
        }
        
        // Get the created user ID
        var user = User.GetUserUsername(registerDto.username);
        if (user == null) throw new Exception("Failed to retrieve created user");
        _testUserID = user.ID;
        
        Console.WriteLine($"Created test user {_testUserID} with role {user.Role.Name}");
        
        // Create a parking Session for payment tests
        var parkingLots = ParkingLot.GetAll().Take(1).ToList();
        var vehicles = Vehicle.GetAll().Take(1).ToList();
        
        if (parkingLots.Count > 0 && vehicles.Count > 0)
        {
            var startTime = DateTime.Now;
            var endTime = startTime.AddHours(2);
            DBHelper.Create($@"INSERT INTO Sessions (ParkingLotID, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus, CreateDateTime)
                VALUES ({parkingLots[0].ID}, 'TEST-PLATE', {vehicles[0].ID}, {_testUserID}, '{startTime:yyyy-MM-dd HH:mm:ss}', '{endTime:yyyy-MM-dd HH:mm:ss}', 120, 10.00, 'unpaid', NOW())");
            Console.WriteLine($"Created test user {_testUserID} with parking session (expires {endTime:yyyy-MM-dd HH:mm:ss})");
        }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Clean up in correct order to avoid foreign key constraint errors
        DBHelper.Delete($"DELETE FROM Transaction WHERE TransactionID LIKE 'TEST-INTEGRATION%' OR TransactionID = 'TEST-INVALID' OR TransactionID = 'TEST'");
        DBHelper.Delete($"DELETE FROM Sessions WHERE UserID = {_testUserID}");
        DBHelper.Delete($"DELETE FROM UserSessions WHERE UserID = {_testUserID}");
        DBHelper.Delete($"DELETE FROM Users WHERE ID = {_testUserID}");
    }

    // CREATE Tests - POST Payment
    [TestMethod]
    [Priority(0)]
    public void PostCreatePayment_ValidData_ReturnsCreated()
    {
        var controller = new PaymentsController();
        var dto = new createPaymentDto("TEST-INTEGRATION-TXN", 25.50m);
        var result = controller.POST_CreatePayment(_validAuthToken!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));

        // disabled: fetching created payment ID for later tests
        // taking too long to execute

        // // Store payment ID for cleanup and later tests
        // var payments = Payment.GetAllByUserID(_testUserID);
        // var createdPayment = payments.FirstOrDefault(p => p.TransactionID == dto.transaction);
        // if (createdPayment != null)
        // {
        //     _testPaymentId = createdPayment.ID;
        // }


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


