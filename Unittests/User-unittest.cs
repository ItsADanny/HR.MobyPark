using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;

namespace MyProject.Tests;

[TestClass]
[DoNotParallelize]
public sealed class UserModelTests
{
    // Static variable to store the created user ID for use across tests
    private static int _createdUserId = 0;
    private static string _testUsername = "";
    private static string _testPassword = "TestPassword123!";
    private static string _testEmail = "";

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<UserModelTests>()
            .Build();

        // Initialize DBHelper with configuration from backend
        DBHelper.dBConfig = configuration.GetSection("Database").Get<DBConfig>();
    }

    [TestMethod]
    [Priority(0)]
    public void User_CreateDB_CreatesUserSuccessfully()
    {
        // Arrange
        _testUsername = $"test-user-unit-{DateTime.Now.Ticks}";
        _testEmail = $"test-user-{DateTime.Now.Ticks}@unittest.com";
        User newUser = new User(
            _testUsername,
            "Test User Model",
            _testEmail,
            "+31612345678",
            new DateOnly(1995, 5, 15)
        );

        // Act
        bool result = newUser.CreateDB(_testPassword);

        // Assert
        Assert.IsTrue(result, "CreateDB should return true when user is created successfully");

        // Store the created user ID for other tests
        User? createdUser = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(createdUser, "Created user should be retrievable from database");
        _createdUserId = createdUser.ID;
    }

    [TestMethod]
    [Priority(1)]
    public void User_GetByID_ReturnsUser()
    {
        // Act
        User? user = User.GetByID(_createdUserId);

        // Assert
        Assert.IsNotNull(user, "GetByID should return the created user");
        Assert.AreEqual(_createdUserId, user.ID, "User ID should match");
        Assert.AreEqual(_testUsername, user.Username, "Username should match");
    }

    [TestMethod]
    [Priority(2)]
    public void User_GetUserUsername_ReturnsUser()
    {
        // Act
        User? user = User.GetUserUsername(_testUsername);

        // Assert
        Assert.IsNotNull(user, "GetUserUsername should return the created user");
        Assert.AreEqual(_testUsername, user.Username, "Username should match");
    }

    [TestMethod]
    [Priority(3)]
    public void User_GetUserEmail_ReturnsUser()
    {
        // Act
        User? user = User.GetUserEmail(_testEmail);

        // Assert
        Assert.IsNotNull(user, "GetUserEmail should return the created user");
        Assert.AreEqual(_testEmail, user.Email, "Email should match");
    }

    [TestMethod]
    [Priority(4)]
    public void User_GetUser_ReturnsUser()
    {
        // Act
        User? user = User.GetUser(_createdUserId);

        // Assert
        Assert.IsNotNull(user, "GetUser should return the created user");
        Assert.AreEqual(_createdUserId, user.ID, "User ID should match");
    }

    [TestMethod]
    [Priority(5)]
    public void User_DoesUsernameExist_ReturnsTrue()
    {
        // Act
        bool exists = User.DoesUsernameExist(_testUsername);

        // Assert
        Assert.IsTrue(exists, "DoesUsernameExist should return true for existing username");
    }

    [TestMethod]
    [Priority(6)]
    public void User_DoesUsernameExist_NonExistentUsername_ReturnsFalse()
    {
        // Act
        bool exists = User.DoesUsernameExist("nonexistent-user-12345");

        // Assert
        Assert.IsFalse(exists, "DoesUsernameExist should return false for non-existent username");
    }

    [TestMethod]
    [Priority(7)]
    public void User_DoesEmailExist_ReturnsTrue()
    {
        // Act
        bool exists = User.DoesEmailExist(_testEmail);

        // Assert
        Assert.IsTrue(exists, "DoesEmailExist should return true for existing email");
    }

    [TestMethod]
    [Priority(8)]
    public void User_DoesEmailExist_NonExistentEmail_ReturnsFalse()
    {
        // Act
        bool exists = User.DoesEmailExist("nonexistent@email.com");

        // Assert
        Assert.IsFalse(exists, "DoesEmailExist should return false for non-existent email");
    }

    [TestMethod]
    [Priority(9)]
    public void User_IsPasswordCorrect_ValidPassword_ReturnsTrue()
    {
        // Arrange
        User? user = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(user);

        // Act
        bool isCorrect = user.IsPasswordCorrect(_testPassword);

        // Assert
        Assert.IsTrue(isCorrect, "IsPasswordCorrect should return true for correct password");
    }

    [TestMethod]
    [Priority(10)]
    public void User_IsPasswordCorrect_InvalidPassword_ReturnsFalse()
    {
        // Arrange
        User? user = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(user);

        // Act
        bool isCorrect = user.IsPasswordCorrect("WrongPassword123!");

        // Assert
        Assert.IsFalse(isCorrect, "IsPasswordCorrect should return false for incorrect password");
    }

    [TestMethod]
    [Priority(11)]
    public void User_ToJSON_ProducesValidJSON()
    {
        // Arrange
        User? user = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(user);

        // Act
        string json = User.ToJSON(user);

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("id"), "JSON should contain id field");
        Assert.IsTrue(json.Contains("username"), "JSON should contain username field");
        Assert.IsTrue(json.Contains("email"), "JSON should contain email field");
        Assert.IsTrue(json.Contains(_testUsername), "JSON should contain the username value");
    }

    [TestMethod]
    [Priority(12)]
    public void User_GetAll_NoException()
    {
        try
        {
            // Act
            List<User> users = User.GetAll();

            // Assert
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count > 0, "GetAll should return at least one user");
            
            // Verify our created user is in the list
            bool foundUser = users.Any(u => u.ID == _createdUserId);
            Assert.IsTrue(foundUser, "Created user should be in the GetAll results");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception thrown during GetAll: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}");
        }
    }

    [TestMethod]
    [Priority(13)]
    public void User_UpdateDB_UpdatesUserSuccessfully()
    {
        // Arrange
        User? user = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(user);
        
        string newName = "Updated Test User Model";
        string newPhone = "+31698765432";
        user.Name = newName;
        user.Phone = newPhone;

        // Act
        bool result = user.UpdateDB();

        // Assert
        Assert.IsTrue(result, "UpdateDB should return true when user is updated successfully");

        // Verify the update
        User? updatedUser = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(updatedUser);
        Assert.AreEqual(newName, updatedUser.Name, "User name should be updated");
        Assert.AreEqual(newPhone, updatedUser.Phone, "User phone should be updated");
    }

    [TestMethod]
    [Priority(14)]
    public void User_SetPassword_UpdatesPassword()
    {
        // Arrange
        User? user = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(user);
        string newPassword = "NewPassword456!";

        // Act
        user.SetPassword(newPassword);
        user.UpdateDB();

        // Verify new password works
        User? updatedUser = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(updatedUser);
        bool isCorrect = updatedUser.IsPasswordCorrect(newPassword);

        // Assert
        Assert.IsTrue(isCorrect, "New password should be correct after SetPassword");
        Assert.IsFalse(updatedUser.IsPasswordCorrect(_testPassword), "Old password should no longer work");
    }

    [TestMethod]
    [Priority(15)]
    public void User_Active_IsSet()
    {
        // Arrange
        User? user = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(user);

        // Assert
        Assert.IsNotNull(user.Active, "Active property should be set");
    }

    [TestMethod]
    [Priority(16)]
    public void User_Role_IsAssigned()
    {
        // Arrange
        User? user = User.GetUserUsername(_testUsername);
        Assert.IsNotNull(user);

        // Assert
        Assert.IsNotNull(user.Role, "User should have a role assigned");
        Assert.IsNotNull(user.Role.ID, "Role should have an ID");
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.Role.Name), "Role should have a name");
    }
}
