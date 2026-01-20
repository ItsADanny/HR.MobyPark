// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Microsoft.Extensions.Configuration;
// using MySql.Data.MySqlClient;

// [TestClass]
// [DoNotParallelize]
// public class VehicleModelTests
// {
//     private static int _createdVehicleId = 0;

//     [ClassInitialize]
//     public static void ClassSetup(TestContext context)
//     {
//         var configuration = new ConfigurationBuilder()
//             .AddUserSecrets<VehicleModelTests>()
//             .Build();

//         var dbConfig = configuration.GetSection("Database").Get<DBConfig>();
//         if (dbConfig == null)
//         {
//             Assert.Inconclusive("Database configuration missing. Run: dotnet user-secrets set 'Database:HST' 'localhost' --project HR.MobyPark-UnitTesting");
//             return;
//         }
//         DBHelper.dBConfig = dbConfig;
//     }

//     [TestMethod]
//     [Priority(0)]
//     public void CreateDB_InsertsVehicleSuccessfully()
//     {
//         // Arrange
//         string uniqueLicensePlate = $"TEST-{DateTime.Now.Ticks}";
//         Vehicle vehicle = new Vehicle(
//             0, // ID will be set by DB
//             1, // UserID
//             uniqueLicensePlate,
//             "TestMake",
//             "TestModel",
//             "TestColor",
//             2024,
//             DateTime.Now
//         );

//         // Act
//         bool result = vehicle.CreateDB();

//         // Assert
//         Assert.IsTrue(result, "CreateDB should return true when vehicle is created successfully");

//         // Get the created vehicle to store ID
//         List<Vehicle> allVehicles = Vehicle.GetAll();
//         Vehicle? createdVehicle = allVehicles.FirstOrDefault(v => v.LicensePlate == uniqueLicensePlate);
//         if (createdVehicle != null)
//         {
//             _createdVehicleId = createdVehicle.ID;
            
//             // Verify properties
//             Assert.AreEqual(uniqueLicensePlate, createdVehicle.LicensePlate);
//             Assert.AreEqual("TestMake", createdVehicle.Make);
//             Assert.AreEqual("TestModel", createdVehicle.Model);
//             Assert.AreEqual("TestColor", createdVehicle.Color);
//             Assert.AreEqual(2024, createdVehicle.ModelYear);
//         }
//     }

//     [TestMethod]
//     [Priority(1)]
//     public void GetByID_RetrievesCorrectVehicle()
//     {
//         // Skip if no vehicle was created
//         if (_createdVehicleId == 0)
//         {
//             Assert.Inconclusive("No vehicle was created in previous test");
//             return;
//         }

//         // Act
//         Vehicle? vehicle = Vehicle.GetByID(_createdVehicleId);

//         // Assert
//         Assert.IsNotNull(vehicle, "GetByID should return the vehicle");
//         Assert.AreEqual(_createdVehicleId, vehicle.ID);
//     }

//     [TestMethod]
//     [Priority(2)]
//     public void GetAll_ReturnsVehicleList()
//     {
//         // Act
//         List<Vehicle> vehicles = Vehicle.GetAll();

//         // Assert
//         Assert.IsNotNull(vehicles, "GetAll should return a list");
//         Assert.IsTrue(vehicles.Count > 0, "GetAll should return at least one vehicle");

//         // If we created a vehicle, it should be in the list
//         if (_createdVehicleId != 0)
//         {
//             bool found = vehicles.Any(v => v.ID == _createdVehicleId);
//             Assert.IsTrue(found, "Created vehicle should be in the GetAll results");
//         }
//     }

//     [TestMethod]
//     [Priority(3)]
//     public void ToJSON_ConvertsVehicleCorrectly()
//     {
//         // Skip if no vehicle was created
//         if (_createdVehicleId == 0)
//         {
//             Assert.Inconclusive("No vehicle was created in previous test");
//             return;
//         }

//         // Arrange
//         Vehicle? vehicle = Vehicle.GetByID(_createdVehicleId);
//         Assert.IsNotNull(vehicle);

//         // Act
//         var json = Vehicle.ToJSON(vehicle);

//         // Assert
//         Assert.IsNotNull(json);
//         Assert.AreEqual(vehicle.ID, (int)json["id"]!);
//         Assert.AreEqual(vehicle.LicensePlate, (string)json["licensePlate"]!);
//         Assert.AreEqual(vehicle.Make, (string)json["make"]!);
//         Assert.AreEqual(vehicle.Model, (string)json["model"]!);
//         Assert.AreEqual(vehicle.Color, (string)json["color"]!);
//         Assert.AreEqual(vehicle.ModelYear, (int)json["modelYear"]!);
//     }

//     [ClassCleanup]
//     public static void Cleanup()
//     {
//         // Clean up created test vehicle after all tests
//         if (_createdVehicleId != 0)
//         {
//             try
//             {
//                 MySqlConnection conn = new MySqlConnection(DBHelper.GetConnectionSTR());
//                 conn.Open();
//                 MySqlCommand cmd = new MySqlCommand(
//                     $"DELETE FROM Vehicles WHERE ID = {_createdVehicleId}",
//                     conn
//                 );
//                 cmd.ExecuteNonQuery();
//                 conn.Close();
//             }
//             catch { /* Cleanup failure is not critical */ }
//         }
//     }
// }
