public static class AppUI
{
    public static string Header = @"" +
                                   "" +
                                   "" +
                                   "" +
                                   "";

    public static string GenProgressBar(int currentProgress, int maxProgress, int maxSize = 32)
    {
        double progressDone = Convert.ToDouble(currentProgress) / Convert.ToDouble(currentProgress);
        int BlocksDone = Convert.ToInt32(Convert.ToDouble(maxSize / 100) * progressDone);

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
            
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                return usrPOS;
            }
        }
    }
}