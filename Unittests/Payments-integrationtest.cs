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
    
    // Billing user (reused across test runs for data accumulation)
    private static string? _billingAuthToken;
    private static int _billingUserID = 0;
    private const string BILLING_USERNAME = "test-billing-user";

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PaymentsControllerIntegrationTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();

        // ============================================================================
        // Setup 1: Create NEW user for payment integration tests (cleaned up after)
        // ============================================================================
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
            var startTime = DateTime.Now.AddDays(-1);
            var endTime = startTime.AddHours(2);
            DBHelper.Create($@"INSERT INTO Sessions (ParkingLotID, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus, CreateDateTime)
                VALUES ({parkingLots[0].ID}, 'TEST-PLATE', {vehicles[0].ID}, {_testUserID}, '{startTime:yyyy-MM-dd HH:mm:ss}', '{endTime:yyyy-MM-dd HH:mm:ss}', 120, 10.00, 'unpaid', NOW())");
            Console.WriteLine($"Created payment test session for user {_testUserID}");
        }

        // ============================================================================
        // Setup 2: Create or REUSE billing user (for data accumulation across runs)
        // ============================================================================
        var existingBillingUser = User.GetUserUsername(BILLING_USERNAME);
        
        if (existingBillingUser == null)
        {
            // Create new billing user
            var billingRegisterDto = new registerDto(
                BILLING_USERNAME,
                "billing-test-password",
                "Billing Test User",
                "billing-test@mobypark.test",
                "+31612345679",
                new DateOnly(1990, 1, 1)
            );
            
            var billingRegisterResult = authController.register(billingRegisterDto);
            if (billingRegisterResult.Result is not CreatedResult)
            {
                throw new Exception("Failed to create billing test user");
            }
            
            existingBillingUser = User.GetUserUsername(BILLING_USERNAME);
            Console.WriteLine($"Created NEW billing user: {existingBillingUser?.ID}");
        }
        else
        {
            Console.WriteLine($"Reusing EXISTING billing user: {existingBillingUser.ID}");
        }
        
        _billingUserID = existingBillingUser!.ID;
        
        // Login billing user
        var billingLoginDto = new loginDto(BILLING_USERNAME, "billing-test-password");
        var billingLoginResult = authController.login(billingLoginDto);
        
        if (billingLoginResult.Result is OkObjectResult billingOkResult)
        {
            var billingLoginJson = JObject.Parse(billingOkResult.Value?.ToString() ?? "");
            _billingAuthToken = billingLoginJson["session_token"]?.ToString();
            Console.WriteLine("Billing user logged in successfully");
        }
        
        // Create a new session + payment for billing user (accumulates across runs)
        if (parkingLots.Count > 0 && vehicles.Count > 0)
        {
            var startTime = DateTime.Now.AddDays(-1);
            var endTime = startTime.AddHours(2);
            DBHelper.Create($@"INSERT INTO Sessions (ParkingLotID, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus, CreateDateTime)
                VALUES ({parkingLots[0].ID}, 'BILLING-PLATE', {vehicles[0].ID}, {_billingUserID}, '{startTime:yyyy-MM-dd HH:mm:ss}', '{endTime:yyyy-MM-dd HH:mm:ss}', 120, 10.00, 'unpaid', NOW())");
            
            // Get the created session ID
            var sessions = Session.GetAllByUserID(_billingUserID);
            var latestSession = sessions.OrderByDescending(s => s.ID).FirstOrDefault();
            
            if (latestSession != null)
            {
                // Add test payment for billing endpoint testing
                var paymentDate = DateTime.Now.AddDays(-1);
                DBHelper.Create($@"INSERT INTO Transaction (TransactionID, ParkingLotID, SessionID, UserID, Amount, Date, Method, Issuer, Bank, Hash, CompletedDateTime, CreateDateTime)
                    VALUES ('TEST-BILLING-{DateTime.Now.Ticks}', {parkingLots[0].ID}, {latestSession.ID}, {_billingUserID}, 5.00, '{paymentDate:yyyy-MM-dd HH:mm:ss}', 'CreditCard', 'Visa', 'TestBank', 'test-hash-{DateTime.Now.Ticks}', '{paymentDate:yyyy-MM-dd HH:mm:ss}', NOW())");
                Console.WriteLine($"Created billing session {latestSession.ID} + payment (amount: 10.00, paid: 5.00, balance: 5.00)");
            }
        }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // Clean up ONLY the payment test user (created fresh each run)
        // Do NOT delete the billing user - it should persist across runs for data accumulation
        Console.WriteLine($"Cleaning up payment test user {_testUserID}, keeping billing user {_billingUserID}");
        
        // Delete in correct order to avoid foreign key constraint errors
        DBHelper.Delete($"DELETE FROM Transaction WHERE UserID = {_testUserID}");
        DBHelper.Delete($"DELETE FROM Sessions WHERE UserID = {_testUserID}");
        DBHelper.Delete($"DELETE FROM UserSessions WHERE UserID = {_testUserID}");
        DBHelper.Delete($"DELETE FROM Users WHERE ID = {_testUserID}");
        
        Console.WriteLine("Payment test user cleanup complete. Billing user data preserved.");
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

        // Fetch created payment ID for later tests - optimized with Take(10)
        var payments = Payment.GetAllByUserID(_testUserID).Take(10).ToList();
        var createdPayment = payments.FirstOrDefault(p => p.TransactionID == dto.transaction);
        if (createdPayment != null)
        {
            _testPaymentId = createdPayment.ID;
            Console.WriteLine($"Captured payment ID: {_testPaymentId}");
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

    // Billing Tests - GET /payments/billing (uses persistent billing user)
    [TestMethod]
    [Priority(14)]
    public void GetBilling_ValidAuth_ReturnsOkWithData()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Billing(_billingAuthToken!);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult?.Value);
        
        // Verify it returns a JArray
        var billingData = okResult.Value as JArray;
        Assert.IsNotNull(billingData);
        
        // Should have at least one billing record for our billing user
        Assert.IsTrue(billingData.Count > 0, "Should have at least one billing record");
        
        // Verify structure of first billing record
        var firstRecord = billingData[0] as JObject;
        Assert.IsNotNull(firstRecord);
        Assert.IsTrue(firstRecord.ContainsKey("session"));
        Assert.IsTrue(firstRecord.ContainsKey("parking"));
        Assert.IsTrue(firstRecord.ContainsKey("amount"));
        Assert.IsTrue(firstRecord.ContainsKey("thash"));
        Assert.IsTrue(firstRecord.ContainsKey("payed"));
        Assert.IsTrue(firstRecord.ContainsKey("balance"));
        
        Console.WriteLine($"Billing returned {billingData.Count} records for billing user {_billingUserID}");
        Console.WriteLine($"First record: {firstRecord}");
    }

    [TestMethod]
    [Priority(15)]
    public void GetBilling_NoAuth_ReturnsUnauthorized()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Billing(null!);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [Priority(16)]
    public void GetBilling_InvalidAuth_ReturnsUnauthorized()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Billing("invalid-token-12345");

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [Priority(19)]
    public void GetBilling_EmptyResult_ForUserWithNoSessions()
    {
        // Create a user with no sessions
        var authController = new AuthController();
        var registerDto = new registerDto(
            $"test-nosessions-{DateTime.Now.Ticks}",
            "test-password-123",
            "No Sessions User",
            $"nosessions-{DateTime.Now.Ticks}@test.com",
            "+31612345680",
            new DateOnly(1990, 1, 1)
        );
        
        var registerResult = authController.register(registerDto);
        Assert.IsInstanceOfType(registerResult.Result, typeof(CreatedResult));
        
        var loginDto = new loginDto(registerDto.username, registerDto.password);
        var loginResult = authController.login(loginDto);
        var loginJson = JObject.Parse((loginResult.Result as OkObjectResult)!.Value!.ToString()!);
        var noSessionsAuthToken = loginJson["session_token"]!.ToString();
        
        var controller = new PaymentsController();
        var result = controller.GET_Billing(noSessionsAuthToken);
        
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        var billingData = okResult!.Value as JArray;
        
        Assert.IsNotNull(billingData);
        Assert.AreEqual(0, billingData.Count, "User with no sessions should have empty billing data");
        
        // Cleanup
        var user = User.GetUserUsername(registerDto.username);
        if (user != null)
        {
            DBHelper.Delete($"DELETE FROM UserSessions WHERE UserID = {user.ID}");
            DBHelper.Delete($"DELETE FROM Users WHERE ID = {user.ID}");
        }
        
        Console.WriteLine("✓ Empty billing data returned correctly for user with no sessions");
    }

    // ============================================================================
    // ADMIN BILLING TESTS - GET /payments/billing/{username}
    // ============================================================================
    
    [TestMethod]
    [Priority(20)]
    public void GetBillingForUser_AdminUser_ReturnsOk()
    {
        // Create an admin user for testing
        var authController = new AuthController();
        var adminRegisterDto = new registerDto(
            $"test-admin-{DateTime.Now.Ticks}",
            "admin-password-123",
            "Admin Test User",
            $"admin-{DateTime.Now.Ticks}@test.com",
            "+31612345681",
            new DateOnly(1990, 1, 1)
        );
        
        var registerResult = authController.register(adminRegisterDto);
        Assert.IsInstanceOfType(registerResult.Result, typeof(CreatedResult));
        
        var adminUser = User.GetUserUsername(adminRegisterDto.username);
        Assert.IsNotNull(adminUser);
        
        // Update user role to ADMIN
        DBHelper.Update($"UPDATE Users SET RoleID = (SELECT ID FROM UserRole WHERE Name = 'ADMIN' LIMIT 1) WHERE ID = {adminUser.ID}");
        
        // Verify the admin user was updated
        var verifyUser = User.GetUser(adminUser.ID);
        Console.WriteLine($"Admin user role after update: {verifyUser?.Role?.Name ?? "NULL"}, RoleID: {verifyUser?.Role?.ID ?? -1}");
        
        var loginDto = new loginDto(adminRegisterDto.username, adminRegisterDto.password);
        var loginResult = authController.login(loginDto);
        var loginJson = JObject.Parse((loginResult.Result as OkObjectResult)!.Value!.ToString()!);
        var adminAuthToken = loginJson["session_token"]!.ToString();
        
        // Test getting billing for another user
        var controller = new PaymentsController();
        var result = controller.GET_BillingForUser(adminAuthToken, BILLING_USERNAME);
        
        // Check for OkObjectResult or generic ObjectResult with 200 status
        var okResult = result.Result as OkObjectResult;
        if (okResult == null)
        {
            var objResult = result.Result as ObjectResult;
            Assert.IsNotNull(objResult, "Result should be an ObjectResult");
            Assert.AreEqual(200, objResult.StatusCode ?? 0, "Status code should be 200");
            var billingData = objResult.Value as JArray;
            Assert.IsNotNull(billingData);
            Assert.IsTrue(billingData.Count > 0, "Billing user should have sessions");
        }
        else
        {
            var billingData = okResult.Value as JArray;
            Assert.IsNotNull(billingData);
            Assert.IsTrue(billingData.Count > 0, "Billing user should have sessions");
        }
        
        // Cleanup
        DBHelper.Delete($"DELETE FROM UserSessions WHERE UserID = {adminUser.ID}");
        DBHelper.Delete($"DELETE FROM Users WHERE ID = {adminUser.ID}");
        
        Console.WriteLine($"✓ Admin successfully retrieved billing data for {BILLING_USERNAME}");
    }

    [TestMethod]
    [Priority(21)]
    public void GetBillingForUser_NonAdminUser_ReturnsForbidden()
    {
        var controller = new PaymentsController();
        var result = controller.GET_BillingForUser(_billingAuthToken!, "some-other-user");
        
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        var objectResult = result.Result as ObjectResult;
        Assert.AreEqual(403, objectResult!.StatusCode);
        
        Console.WriteLine("✓ Non-admin user correctly denied access");
    }

    [TestMethod]
    [Priority(22)]
    public void GetBillingForUser_NonExistentUser_ReturnsNotFound()
    {
        // Create admin user
        var authController = new AuthController();
        var adminRegisterDto = new registerDto(
            $"test-admin-notfound-{DateTime.Now.Ticks}",
            "admin-password-123",
            "Admin Test User",
            $"admin-notfound-{DateTime.Now.Ticks}@test.com",
            "+31612345682",
            new DateOnly(1990, 1, 1)
        );
        
        authController.register(adminRegisterDto);
        var adminUser = User.GetUserUsername(adminRegisterDto.username);
        DBHelper.Update($"UPDATE Users SET RoleID = (SELECT ID FROM UserRole WHERE Name = 'ADMIN' LIMIT 1) WHERE ID = {adminUser!.ID}");
        
        var loginDto = new loginDto(adminRegisterDto.username, adminRegisterDto.password);
        var loginResult = authController.login(loginDto);
        var loginJson = JObject.Parse((loginResult.Result as OkObjectResult)!.Value!.ToString()!);
        var adminAuthToken = loginJson["session_token"]!.ToString();
        
        var controller = new PaymentsController();
        var result = controller.GET_BillingForUser(adminAuthToken, "nonexistent-user-xyz-12345");
        
        // Check for NotFoundObjectResult or generic ObjectResult with 404 status
        var notFoundResult = result.Result as NotFoundObjectResult;
        if (notFoundResult == null)
        {
            var objResult = result.Result as ObjectResult;
            Assert.IsNotNull(objResult, "Result should be an ObjectResult");
            Assert.AreEqual(404, objResult.StatusCode ?? 0, "Status code should be 404");
        }
        
        // Cleanup
        DBHelper.Delete($"DELETE FROM UserSessions WHERE UserID = {adminUser.ID}");
        DBHelper.Delete($"DELETE FROM Users WHERE ID = {adminUser.ID}");
        
        Console.WriteLine("✓ Non-existent user correctly returns NotFound");
    }

    [TestMethod]
    [Priority(23)]
    public void GetBillingForUser_NoAuth_ReturnsUnauthorized()
    {
        var controller = new PaymentsController();
        var result = controller.GET_BillingForUser(null!, BILLING_USERNAME);
        
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
        
        Console.WriteLine("✓ No auth token correctly returns Unauthorized");
    }
}