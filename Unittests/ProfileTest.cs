using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;


namespace MyProject.Tests;

[TestClass]
public class ProfileControllerTests
{
    // ------------------------------
    // GET
    // ------------------------------
    [TestMethod]
    public void GetProfile_NullAuthorization()
    {
        var controller = new ProfileController();
        var result = controller.GETProfile(null!);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void GetProfile_EmptyAuthorization()
    {
        var controller = new ProfileController();
        var result = controller.GETProfile("");

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    // ------------------------------
    // PUT
    // ------------------------------
    [TestMethod]
    public void PutProfile_NullAuthorization()
    {
        var controller = new ProfileController();
        var dto = new registerDto(
            "TestUser",
            "",
            "Test User",
            "testuser@testuser.nl",
            "06123456789",
            new DateOnly(1990, 1, 1)
        );

        var result = controller.PUTProfile(null!, dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void PutProfile_EmptyAuthorization()
    {
        var controller = new ProfileController();
        var dto = new registerDto(
            "TestUser",
            "",
            "Test User",
            "testuser@testuser.nl",
            "06123456789",
            new DateOnly(1990, 1, 1)
        );

        var result = controller.PUTProfile("", dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }
}

[TestClass]
public class ProfileModelTests
{
    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ProfileModelTests>()
            .Build();

        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }   

    // [TestMethod]
    // public void ToJSON_SingleProfile()
    // {
    //     var user = new User(
    //         "TestUser",
    //         "Test User",
    //         "testuser@testuser.nl",
    //         "06123456789",
    //         new DateOnly(1990, 1, 1)
    //     );
    //     user.Role = UserRole.GetByID(1);
        
    //     JObject obj = JObject.Parse(User.ToJSON(user));
    //     Assert.IsNotNull(obj);
    //     Assert.AreEqual("TestUser", obj.Value<string>("username"));
    //     Assert.AreEqual("Test User", obj.Value<string>("name"));

    // }

    // [TestMethod]
    // public void ToJSON_ListOfProfiles()
    // {
    //     List<User> profiles = new List<User>
    //     {
    //         new User("Alice", "Alice@alice.nl", "0612345678", "", new DateOnly()),
    //         new User("Bob", "Bob@bob.nl", "122112213", "", new DateOnly())
    //     };

    //     JArray arr = (JArray)User.ToJSON(profiles);

    //     Assert.IsNotNull(arr);
    //     Assert.AreEqual(2, arr.Count);
    // }

    // [TestMethod]
    // public void CreateDB_InsertsUserSuccessfully()
    // {
    //     User user = new User
    //     {
    //         Username = "TestUser",
    //         Name = "Test User",
    //         Email = "testuser@testuser.nl",
    //         Phone = "06123456789",
    //         BirthDate = new DateOnly(1990, 1, 1)
    //     };

    //     bool result = user.CreateDB();

    //     Assert.IsTrue(result);
    // }

    // [TestMethod]
    // public void UpdateDB_UpdatesProfileSuccessfully()
    // {
    //     User user = new User
    //     {
    //         Username = "TestUser",
    //         Name = "Test User",
    //         Email = "testuser@testuser.nl",
    //         Phone = "06123456789",
    //         BirthDate = new DateOnly(1990, 1, 1)
    //     };

    //     // nieuwe user maken zodat we deze zometeen kunnen wijzigen
    //     user.CreateDB();

    //     // update de net gemaakte user.
    //     user.Name = "Updated Test User";
    //     user.Email = "updatedtestuser@testuser.nl";
    //     user.Phone = "06987654321";
    //     user.BirthDate = new DateOnly(1990, 1, 2);
        
    //     bool result = user.UpdateDB();

    //     Assert.IsTrue(result);
    // }

    // [TestMethod]
    // public void GetAll_Users()
    // {
    //     User user = new User
    //     {
    //         Username = "TestUser",
    //         Name = "Test User",
    //         Email = "testuser@testuser.nl",
    //         Phone = "06123456789",
    //         BirthDate = new DateOnly(1990, 1, 1)
    //     };

    //     user.CreateDB();

    //     try
    //     {
    //         List<User> users = User.GetAll();
    //         Assert.IsNotNull(users);
    //     }
    //     catch (Exception ex)
    //     {
    //         Assert.Fail($"Exception thrown during GetAll: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\nInner Exception: {ex.InnerException?.Message}");
    //     }
    // }

    // [TestMethod]
    // public void GetByID_Users()
    // {
    //     User user = new User
    //     {
    //         Username = "TestUser",
    //         Name = "Test User",
    //         Email = "testuser@testuser.nl",
    //         Phone = "06123456789",
    //         BirthDate = new DateOnly(1990, 1, 1)
    //     };

    //     user.CreateDB();

    //     user = User.GetByID(user.ID);
    //     Assert.IsNotNull(user);
    // }
}