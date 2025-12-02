// namespace Unittests;
// using System;
// using Moq;
// using MUser = User;


// [TestClass]
// public class ReservationControllerTests
// {

//     private const string validToken = "validsessiontoken";
//     private const int userIdOwner = 100;
//     private const int reservationIdValid = 1;
//     private const int reservationIdNotFound = 99;

//     private Mock<UserSession> mockUserSession;
//     private Mock<MUser> mockUserOwner;
//     private Mock<Reservation> mockReservation;




//     [TestMethod]
//     public void TestMethod1()
//     {
//     }
// }

using Microsoft.VisualStudio.TestTools.UnitTesting;
// You need this using statement to access the classes in the main project
// Note: This assumes the main project's classes (like DBConfig) are in the global namespace.
// If your main project uses a namespace (e.g., "ParkingSystemApp"), you would use:
// using ParkingSystemApp; 

// --- Test Class to Verify Project Reference ---
namespace UnitTestProject
{
    [TestClass]
    public class ConnectionTest
    {
        [TestMethod]
        public void CanAccessDBConfigClass()
        {
            // ACT: Try to create an instance of a class from the main project (ParkingSystem.csproj).
            // This line will only compile if the <ProjectReference> in the .csproj is correct.
            DBConfig config = new DBConfig();

            // ASSERT 1: Verify the object was created (the reference is valid).
            Assert.IsNotNull(config, "Failed to instantiate DBConfig, connection may be broken.");

            // ASSERT 2: Simple check on a property to ensure the type is being used correctly.
            config.HST = "localhost";
            Assert.AreEqual("localhost", config.HST, "Property setting failed, indicating a deeper compilation issue.");
        }

        [TestMethod]
        public void CanAccessAuthClass()
        {
            // ACT: Try to call a static method from the 'Auth' class.
            // This verifies the connection to the static class.
            string token = Auth.GenAuthToken();

            // ASSERT: A generated token should be a non-empty string.
            Assert.IsFalse(string.IsNullOrEmpty(token), "Failed to call Auth.GenAuthToken(), connection may be broken.");
        }
    }
}