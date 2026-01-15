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
    public void ToJSON_SingleBilling()
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
            UpdateDateTime = DateTime.Now
        };

        var json = Billing.ToJSON(billing);

        Assert.IsNotNull(json);
        Assert.AreEqual(billing.ID, json.Value<int>("id"));
    }

    [TestMethod]
    public void ToJSON_ListOfBillings()
    {
        List<Billing> billings = new List<Billing>
        {
            new Billing
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
                UpdateDateTime = DateTime.Now
            },
            new Billing
            {
                ID = 2,
                SessionID = 2,
                ParkingLotID = 2,
                Amount = 20,
                tHash = "boop",
                Payed = 15,
                Balance = 5,
                CompletedDateTime = DateTime.Now,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now
            }
        };

        var jsonArray = Billing.ToJSON(billings);

        Assert.IsNotNull(jsonArray);
        Assert.AreEqual(2, jsonArray.Count);
        Assert.AreEqual(billings[0].ID, jsonArray[0].Value<int>("id"));
        Assert.AreEqual(billings[1].ID, jsonArray[1].Value<int>("id"));
    }

    // gaat nogsteeds fout
    // [TestMethod]
    // public void CreateDB_InsertsBillingSuccessfully()
    // {
    //     Billing billing = new Billing
    //     {
    //         ID = 1,
    //         SessionID = 1,
    //         ParkingLotID = 1,
    //         Amount = 10.00m,
    //         tHash = "beep",
    //         Payed = 10.00m,
    //         Balance = 0.00m,
    //         CompletedDateTime = DateTime.Now,
    //         CreateDateTime = DateTime.Now,
    //         UpdateDateTime = DateTime.Now
    //     };

    //     bool result = billing.CreateDB();

    //     Assert.IsTrue(result);
    // }


    // [TestMethod]
    // public void GetAllBillings_NoException()
    // {
    //     Billing billing= new Billing
    //     {
    //         ID = 1,
    //         SessionID = 1,
    //         ParkingLotID = 1,
    //         Amount = 10,
    //         tHash = "beep",
    //         Payed = 10,
    //         Balance = 0,
    //         CompletedDateTime = DateTime.Now,
    //         CreateDateTime = DateTime.Now,
    //         UpdateDateTime = DateTime.Now
    //     };

    //     billing.CreateDB();

    //     List<Billing> billings = Billing.GetAll();
    //     Assert.IsNotNull(billings);
    //     Assert.IsTrue(billings.Count > 0);
    // }

    // [TestMethod]
    // public void GetBillingById_ExistingId_ReturnsBilling()
    // {
    //     var billing = new Billing
    //     {
    //         ID = 1,
    //         SessionID = 1,
    //         ParkingLotID = 1,
    //         Amount = 10,
    //         tHash = "beep",
    //         Payed = 10,
    //         Balance = 0,
    //         CompletedDateTime = DateTime.Now,
    //         CreateDateTime = DateTime.Now,
    //         UpdateDateTime = DateTime.Now
    //     };

    //     billing.CreateDB();

    //     List<Billing> billingList = Billing.GetAll();
    //     var retrievedBilling = Billing.GetByID(billingList[0].ID);
    //     Assert.IsNotNull(retrievedBilling);
    // }  

}