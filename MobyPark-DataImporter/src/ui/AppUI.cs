public static class AppUI
{
    // public static string Header = @"▌ ▌▛▀▖  ▙▗▌▞▀▖▛▀▖▌ ▌▛▀▖▞▀▖▛▀▖▌ ▌     ▛▀▖▞▀▖▀▛▘▞▀▖ ▀▛▘▞▀▖▞▀▖▌" +
    //                                "▌ ▌▙▄▌ ▌ ▙▄▌  ▌ ▌ ▌▌ ▌▌" +
    //                                "▌ ▌▌ ▌ ▌ ▌ ▌  ▌ ▌ ▌▌ ▌▌" +
    //                                "▀▀ ▘ ▘ ▘ ▘ ▘  ▘ ▝▀ ▝▀ ▀▀▘" +
    //                                " | |__| / ____ \| |/ ____ \     | | | |__| | |__| | |____" +
    //                                " |_____/_/    \_\_/_/    \_\    |_|  \____/ \____/|______|";

    public static string Header = "";
    
// ▙▄▌▙▄▘  ▌▘▌▌ ▌▙▄▘▝▞ ▙▄▘▙▄▌▙▄▘▙▞  ▄▄▖ ▌ ▌▙▄▌ ▌ ▙▄▌  ▌ ▌ ▌▌ ▌▌  
// ▌ ▌▌▚ ▗▖▌ ▌▌ ▌▌ ▌ ▌ ▌  ▌ ▌▌▚ ▌▝▖     ▌ ▌▌ ▌ ▌ ▌ ▌  ▌ ▌ ▌▌ ▌▌  
// ▘ ▘▘ ▘▝▘▘ ▘▝▀ ▀▀  ▘ ▘  ▘ ▘▘ ▘▘ ▘     ▀▀ ▘ ▘ ▘ ▘ ▘  ▘ ▝▀ ▝▀ ▀▀▘
  
                                                      

    public static string GenProgressBar(int currentProgress, int maxProgress, int maxSize = 32)
    {
        double progressDone = Convert.ToDouble(currentProgress) / Convert.ToDouble(maxProgress);
        int BlocksDone = Convert.ToInt32(Convert.ToDouble(maxSize) * progressDone);

        string returnValue = "";
        for (int i = 0; i < BlocksDone; i++)
        {
            returnValue += "█";
        }

        for (int i = returnValue.Count(); i < maxSize; i++)
        {
            returnValue += "░";
        }

        return returnValue;
    }
    
    public static int OptionSelector(string header, string[] options)
    {
        int usrPOS = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine(header);
            
            for (int i = 0; i < options.Count(); i++)
            {
                if (i == usrPOS)
                {
                    Console.WriteLine($"> {options[i]}");
                }
                else
                {
                    Console.WriteLine($"  {options[i]}");
                }
            }
            
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Enter:
                    return usrPOS;
                case ConsoleKey.DownArrow:
                    if (usrPOS == options.Count() - 1)
                    {
                        usrPOS = 0;
                    } else
                    {
                        usrPOS++;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (usrPOS == 0)
                    {
                        usrPOS = options.Count() - 1;
                    } else
                    {
                        usrPOS--;
                    }
                    break;
            }
        }
    }
}