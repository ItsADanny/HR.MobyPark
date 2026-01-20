public static class Program
{
    //File/Folder data configuration
    public static string JSONFile_Users = "users.json";
    public static string JSoNFile_ParkingLots = "parking-lots.json";
    public static string JSONFile_Vehicles = "vehicles.json";
    public static string JSONFile_payments = "payments.json";
    public static string JSONFile_reservations = "reservations.json";
    public static string JSONFolder_ParkingSessions = "pdata";

    public static void Main()
    {
        bool shutdown = false;
        while (!shutdown)
        {
            switch (AppUI.OptionSelector(AppUI.Header, ["Import", "Export", "Shutdown"]))
            {
                case 0:
                    Import.StartActivity();
                    break;
                case 1:
                    Export.StartActivity();
                    break;
                case 2:
                    shutdown = true;
                    break;
            }
        }

        Console.WriteLine("Shutting down.");
    }
}