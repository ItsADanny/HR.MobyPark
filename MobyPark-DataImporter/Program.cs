public static class Program
{
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