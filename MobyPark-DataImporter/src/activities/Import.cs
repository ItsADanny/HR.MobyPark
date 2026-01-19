using System.Globalization;
using Mysqlx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class Import
{
    private static string _header = @"" +
                                     "" +
                                     "" +
                                     "" +
                                     "";
    private static string[] _requiredFiles = [Program.JSONFile_Users, Program.JSoNFile_ParkingLots, Program.JSONFile_Vehicles, Program.JSONFile_payments, Program.JSONFile_reservations];
    private static string _importFolderLocation = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent}/Import";

    public static void StartActivity()
    {   
        bool exit = false;
        while (!exit)
        {
            string[] Options;
            string HeaderPart;
            string[] missingFiles = DetectImportFiles();
            if (missingFiles.Count() == 0)
            {
                Options = ["Import data", "Exit"];
                HeaderPart = "";
            } else
            {
                Options = ["Check required files", "Exit"];
                HeaderPart = "";
            }

            switch (AppUI.OptionSelector(_header + HeaderPart, Options))
            {
                case 0:
                    if (missingFiles.Count() == 0) {
                        StartImport();
                    }
                    break;
                case 1:
                    exit = true;
                    break;
            }
        }
    }

    private static void StartImport()
    {
        //User import file tasks
        // ImUsers();
        MergeUsers();

        //
    }
    
    private static void ImUsers()
    {
        List<User> users;

        Console.Clear();
        Console.WriteLine("Import USERS started");
        Console.WriteLine("Retrieving data from \"users.json\"");

        using (StreamReader r = new StreamReader(_importFolderLocation + "/" + Program.JSONFile_Users))
        {
            string json = r.ReadToEnd();
            users = JsonConvert.DeserializeObject<List<User>>(json);
            if (users is null)
            {
                throw new NullReferenceException("Unable to convert valid data into user objects, please check the import data file \"users.json\"");
            }
        }

        ImportToDB(users, "user", Program.JSONFile_Users);
    }

    // private static void ImVehicle()
    // {
    //     List<User> vehicles;

    //     Console.Clear();
    //     Console.WriteLine("Import USERS started");
    //     Console.WriteLine("Retrieving data from \"users.json\"");

    //     using (StreamReader r = new StreamReader(_importFolderLocation + "/" + Program.JSONFile_Vehicles))
    //     {
    //         string json = r.ReadToEnd();
    //         vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(json);
    //         if (vehicles is null)
    //         {
    //             throw new NullReferenceException("Unable to convert valid data into user objects, please check the import data file \"users.json\"");
    //         }
    //     }

    //     ImportToDB(vehicles, "vehicle", Program.JSONFile_Vehicles);
    // }

    private static void ImportToDB<T>(List<T> values, string type, string filename) where T : iSQL
    {
        Console.WriteLine("Data retrieved, data importing started");
        
        int count = 1;
        int errors = 0;
        foreach (var value in values)
        {
            Console.Clear();
            Console.WriteLine(AppUI.GenProgressBar(count, values.Count()));
            Console.WriteLine($"Successful imports: {count - errors}");
            Console.WriteLine($"Errors: {errors}\n");
            Console.WriteLine($"Importing : {count}");
            if(DatabaseHelper.INSERT(value.ToInsertSQL()) == 0)
            {
                errors++;
            }

            count++;
        }

        Console.Clear();
        Console.WriteLine($"Import {type} started");
        Console.WriteLine($"Retrieving data from \"{filename}\"");
        Console.WriteLine("Data retrieved, data importing started");
        Console.WriteLine($"Import complete, {count - errors} legacy {type}s imported");
        Console.WriteLine($"Failed imports: {errors}");
    }

    private static void MergeUsers()
    {
        //Write start merging users message
        Console.WriteLine("Merging users started");
        Thread.Sleep(2500);

        //Declare variables which will be displayed at the end
        int totalLegacyAccounts = 0;
        int newCreatedAccounts = 0;
        int totalMergedAccounts = 0;
        int totalUnMergableAccounts = 0;

        //Create a new Culture info for Year conversion
        CultureInfo format = new CultureInfo("en-US");

        //Retriev the LegacyUsers from the database
        List<User> LegacyUsers = DatabaseHelper.GetUsers();
        totalLegacyAccounts = LegacyUsers.Count();

        int count = 0;
        int errors = 0;
        foreach (User legacyUser in LegacyUsers)
        {
            Console.Clear();
            Console.WriteLine(AppUI.GenProgressBar(count, LegacyUsers.Count()));
            Console.WriteLine($"New accounts created: {newCreatedAccounts}");
            Console.WriteLine($"Accounts merged: {totalMergedAccounts}\n");
            Console.WriteLine($"Account : {count}");

            if (!DatabaseHelper.DoesUserExist(legacyUser.email))
            {
                Console.WriteLine("Kom ik hier?");

                NewUser newUser = new()
                {
                    Username = legacyUser.username,
                    Password = legacyUser.password,
                    Name = legacyUser.name,
                    Email = legacyUser.email,
                    Phone = legacyUser.phone,
                    BirthDate = DateOnly.TryParse($"{legacyUser.birth_year}", format, out DateOnly result) ? result : DateOnly.FromDateTime(DateTime.Now),
                };

                int response = DatabaseHelper.INSERT(newUser.ToInsertSQL());
                if(response != 0)
                {
                    newCreatedAccounts++;
                    legacyUser.NewUserID = response;

                    DatabaseHelper.Update(legacyUser.ToUpdateSQL());
                }

                newCreatedAccounts++;
            }
            else
            {
                User? foundUser = DatabaseHelper.GetUserByEmail(legacyUser.email);
                if (foundUser is not null)
                {
                    legacyUser.NewUserID = Convert.ToInt32(foundUser.id);
                    DatabaseHelper.Update(legacyUser.ToUpdateSQL());
                    totalMergedAccounts++;
                }
            }
        }
    }

    private static string[] DetectImportFiles()
    {
        string[] missingFiles = [];
        foreach (string filename in _requiredFiles)
        {
            if (!File.Exists(_importFolderLocation + "/" + filename))
            {
                missingFiles.Append(filename);
            }
        }
        return missingFiles;
    }
}