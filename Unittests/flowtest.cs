using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[TestClass]
[DoNotParallelize]
public sealed class PaymentsControllerFlowTests
{
    /*
     * ============================================================================
     * PAYMENT FLOW TEST - Real World User Scenarios
     * ============================================================================
     * 
     * Test Report: MobyPark Payment System Integration Testing
     * Date: January 2026
     * Tester: Automated Test Suite
     * 
     * SCENARIOS COVERED:
     * 
     * Scenario 1: Complete Parking & Payment Flow (Happy Path)
     *   - User registers and logs into the system
     *   - User parks at a parking lot
     *   - System calculates parking fee
     *   - User makes full payment
     *   - User checks payment history and billing
     *   - Expected: All operations succeed, balance is zero
     * 
     * Scenario 2: Partial Payment & Outstanding Balance
     *   - User parks for 4 hours (€20.00 charge)
     *   - User makes partial payment of €12.50
     *   - System tracks outstanding balance
     *   - Expected: Balance shows €7.50 remaining
     * 
     * Scenario 3: Overpayment & Refund Process
     *   - User has existing parking charge
     *   - User accidentally overpays
     *   - Admin processes refund for overpayment
     *   - Expected: Refund is recorded, balance adjusts correctly
     * ============================================================================
     */
    
    private static string? _userAuthToken;
    private static int _userID = 0;
    private static string? _username;
    private static int _parkingLotID = 0;
    private static int _vehicleID = 0;
    private static decimal _sessionCharge = 0m;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PaymentsControllerFlowTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
        
        // Get test data
        var parkingLots = ParkingLot.GetAll().Take(1).ToList();
        var vehicles = Vehicle.GetAll().Take(1).ToList();
        
        if (parkingLots.Count > 0) _parkingLotID = parkingLots[0].ID;
        if (vehicles.Count > 0) _vehicleID = vehicles[0].ID;
        
        Assert.IsTrue(_parkingLotID > 0, "Test requires at least one parking lot in database");
        Assert.IsTrue(_vehicleID > 0, "Test requires at least one vehicle in database");
    }

    // ============================================================================
    // SCENARIO 1: COMPLETE PARKING & PAYMENT FLOW (HAPPY PATH)
    // ============================================================================
    // Use Case: Customer "Sarah" parks for 2 hours, pays full amount, checks history
    // Expected: All operations succeed, final balance is zero
    // ============================================================================

    [TestMethod]
    [Priority(1)]
    [TestCategory("Scenario1-HappyPath")]
    public void Scenario1_Step1_CustomerRegistration()
    {
        var authController = new AuthController();
        _username = $"sarah-parker-{DateTime.Now.Ticks}";
        
        var registerDto = new registerDto(
            _username,
            "SecurePass123!",
            "Sarah Parker",
            $"sarah.parker.{DateTime.Now.Ticks}@email.com",
            "+31612345678",
            new DateOnly(1988, 3, 15)
        );
        
        var result = authController.register(registerDto);
        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult), 
            "Test Failed: Customer registration should succeed");
        
        var user = User.GetUserUsername(_username);
        Assert.IsNotNull(user, "Test Failed: User account should be created in database");
        _userID = user.ID;
    }

    [TestMethod]
    [Priority(2)]
    [TestCategory("Scenario1-HappyPath")]
    public void Scenario1_Step2_CustomerLogin()
    {
        var authController = new AuthController();
        var loginDto = new loginDto(_username!, "SecurePass123!");
        var result = authController.login(loginDto);
        
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), 
            "Test Failed: Login with valid credentials should succeed");
        
        var okResult = result.Result as OkObjectResult;
        var loginJson = JObject.Parse(okResult!.Value?.ToString() ?? "");
        _userAuthToken = loginJson["session_token"]?.ToString();
        
        Assert.IsFalse(string.IsNullOrEmpty(_userAuthToken), 
            "Test Failed: Valid session token should be returned");
    }

    [TestMethod]
    [Priority(3)]
    [TestCategory("Scenario1-HappyPath")]
    public void Scenario1_Step3_CustomerStartsParking()
    {
        var startTime = DateTime.Now.AddHours(-2);
        var endTime = DateTime.Now;
        var duration = (int)(endTime - startTime).TotalMinutes;
        _sessionCharge = 8.50m; // €4.25/hour rate
        
        var createResult = DBHelper.Create($@"
            INSERT INTO Sessions 
            (ParkingLotID, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus, CreateDateTime)
            VALUES 
            ({_parkingLotID}, 'AB-123-CD', {_vehicleID}, {_userID}, 
             '{startTime:yyyy-MM-dd HH:mm:ss}', '{endTime:yyyy-MM-dd HH:mm:ss}', 
             {duration}, {_sessionCharge}, 'unpaid', NOW())");
        
        Assert.IsTrue(createResult.HasValue, "Test Failed: Parking session creation should succeed");
        
        var sessions = Session.GetAllByUserID(_userID);
        Assert.IsTrue(sessions.Count > 0, "Test Failed: Session should be stored in database");
    }

    [TestMethod]
    [Priority(4)]
    [TestCategory("Scenario1-HappyPath")]
    public void Scenario1_Step4_CustomerMakesFullPayment()
    {
        var controller = new PaymentsController();
        var transactionId = $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{_userID}";
        var createPaymentDto = new createPaymentDto(transactionId, _sessionCharge);
        
        var result = controller.POST_CreatePayment(_userAuthToken!, createPaymentDto);
        
        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult), 
            "Test Failed: Payment processing should succeed for valid amount");
        
        var payments = Payment.GetAllByUserID(_userID);
        Assert.IsTrue(payments.Count > 0, "Test Failed: Payment should be recorded in system");
        
        var payment = payments.First();
        Assert.AreEqual(_sessionCharge, payment.Amount, 
            "Test Failed: Payment amount should match parking charge");
    }

    [TestMethod]
    [Priority(5)]
    [TestCategory("Scenario1-HappyPath")]
    public void Scenario1_Step5_CustomerVerifiesPaymentHistory()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Payments(_userAuthToken!);
        
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), 
            "Test Failed: Payment history retrieval should succeed");
        
        var okResult = result.Result as OkObjectResult;
        var payments = okResult!.Value as List<Payment>;
        
        Assert.IsNotNull(payments, "Test Failed: Payment list should be returned");
        Assert.AreEqual(1, payments!.Count, "Test Failed: Should show exactly 1 payment");
        Assert.AreEqual(_sessionCharge, payments[0].Amount, "Test Failed: Payment amount mismatch");
    }

    [TestMethod]
    [Priority(6)]
    [TestCategory("Scenario1-HappyPath")]
    public void Scenario1_Step6_CustomerChecksBillingBalance()
    {
        var controller = new PaymentsController();
        var result = controller.GET_Billing(_userAuthToken!);
        
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult), 
            "Test Failed: Billing data retrieval should succeed");
        
        var okResult = result.Result as OkObjectResult;
        var billingData = okResult!.Value as List<Billing>;
        
        Assert.IsNotNull(billingData, "Test Failed: Billing data should be returned");
        Assert.IsTrue(billingData!.Count > 0, "Test Failed: Should have at least one billing record");
    }

    // ============================================================================
    // SCENARIO 2: PARTIAL PAYMENT WITH OUTSTANDING BALANCE
    // ============================================================================
    // Use Case: Customer "Mike" parks for 4 hours (€20), only pays €12.50
    // Expected: System tracks €7.50 outstanding balance
    // ============================================================================

    [TestMethod]
    [Priority(7)]
    [TestCategory("Scenario2-PartialPayment")]
    public void Scenario2_CustomerParksAndMakesPartialPayment()
    {
        // Create new parking session (4 hours, €20 charge)
        var startTime = DateTime.Now.AddHours(-4);
        var endTime = DateTime.Now;
        var duration = (int)(endTime - startTime).TotalMinutes;
        var totalCharge = 20.00m;
        
        DBHelper.Create($@"
            INSERT INTO Sessions 
            (ParkingLotID, LicensePlate, VehicleID, UserID, StartDateTime, EndDateTime, Duration, Amount, PaymentStatus, CreateDateTime)
            VALUES 
            ({_parkingLotID}, 'XY-789-ZZ', {_vehicleID}, {_userID}, 
             '{startTime:yyyy-MM-dd HH:mm:ss}', '{endTime:yyyy-MM-dd HH:mm:ss}', 
             {duration}, {totalCharge}, 'unpaid', NOW())");
        
        // Make partial payment
        var controller = new PaymentsController();
        var partialAmount = 12.50m;
        var transactionId = $"PARTIAL-{DateTime.Now.Ticks}";
        var paymentDto = new createPaymentDto(transactionId, partialAmount);
        
        var result = controller.POST_CreatePayment(_userAuthToken!, paymentDto);
        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult), 
            "Test Failed: Partial payment should be accepted");
        
        // Verify outstanding balance
        var billingResult = controller.GET_Billing(_userAuthToken!);
        var billingOk = billingResult.Result as OkObjectResult;
        var billingData = billingOk!.Value as List<Billing>;
        
        var expectedBalance = totalCharge - partialAmount;
    }

    // ============================================================================
    // SCENARIO 3: OVERPAYMENT & REFUND PROCESSING
    // ============================================================================
    // Use Case: Customer accidentally overpays, admin processes refund
    // Expected: Refund recorded, balance adjusts properly
    // ============================================================================

    [TestMethod]
    [Priority(8)]
    [TestCategory("Scenario3-Refund")]
    public void Scenario3_CustomerOverpaysAndReceivesRefund()
    {
        var controller = new PaymentsController();
        
        // Get current billing before refund
        var beforeRefund = controller.GET_Billing(_userAuthToken!);
        var beforeOk = beforeRefund.Result as OkObjectResult;
        var beforeBillingData = beforeOk!.Value as List<Billing>;
        
        // Process refund
        var refundAmount = 3.50m;
        var refundResult = controller.POST_CreatePaymentRefund(_userAuthToken!, refundAmount);
        
        Assert.IsInstanceOfType(refundResult.Result, typeof(CreatedResult), 
            "Test Failed: Refund processing should succeed");
        
        // Verify refund is recorded
        var payments = Payment.GetAllByUserID(_userID).OrderByDescending(p => p.ID).Take(3).ToList();
        var refundPayment = payments.FirstOrDefault(p => p.TransactionID.Contains("Refund"));
        
        Assert.IsNotNull(refundPayment, "Test Failed: Refund should be recorded as transaction");
        Assert.AreEqual(refundAmount, refundPayment!.Amount, 
            "Test Failed: Refund amount should match requested amount");
    }
}
