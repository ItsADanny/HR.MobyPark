using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Tests;

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
        // Get existing data from database to use as foreign keys
        var existingUsers = User.GetAll();
        var existingSessions = Session.GetAll();
        var existingParkingLots = ParkingLot.GetAll();

        if (existingUsers.Count == 0 || existingSessions.Count == 0 || existingParkingLots.Count == 0)
        {
            Assert.Inconclusive("Test requires at least one User, Session, and ParkingLot in the database");
            return;
        }

        // Arrange - Use existing IDs from database
        Payment payment = new Payment
        {
            TransactionID = $"TEST-{DateTime.Now.Ticks}",
            ParkingLotID = existingParkingLots[0].ID,
            SessionID = existingSessions[0].ID,
            UserID = existingUsers[0].ID,
            Amount = 25.75m,
            Date = DateTime.Now,
            Method = "CreditCard",
            Issuer = "Visa",
            Bank = "TestBank",
            Hash = $"HASH-{DateTime.Now.Ticks}",
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
            // Get existing data from database to use as foreign keys
            var existingUsers = User.GetAll();
            var existingSessions = Session.GetAll();
            var existingParkingLots = ParkingLot.GetAll();

            Payment payment = new Payment
            {
                ID = _createdPaymentId,
                TransactionID = "Test18",
                ParkingLotID = existingParkingLots.Count > 0 ? existingParkingLots[0].ID : 1,
                SessionID = existingSessions.Count > 0 ? existingSessions[0].ID : 1,
                UserID = existingUsers.Count > 0 ? existingUsers[0].ID : 1,
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
