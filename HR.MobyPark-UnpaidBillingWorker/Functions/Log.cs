public static class Log
{
    private static string _fileLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + $"/Logs/{_mGetFileTimeStamp()}_log.txt";
    private static string _timeStampFormat = "MM\\/dd\\/yyyy h\\:mm tt";
    private static string _timeStampFormat_File = "MM-dd-yyyy h\\:mm";

    private static string _mGetFileTimeStamp() => DateTime.Now.ToString(_timeStampFormat_File);
    private static string _mGetTimeStamp() => DateTime.Now.ToString(_timeStampFormat);
    private static string _mGenMessCode() => Auth.Hash($"MESSAGE BLOCK - {_timeStampFormat_File} - MESSAGR BLOCK")[0];
    private static string _mGenIncCode() => Auth.Hash($"ERROR - {_timeStampFormat_File} - ERROR")[0];

    private static bool _mCheckFile() {

        if (!File.Exists(_fileLocation))
        {
            try {
                FileStream fs = File.Create(_fileLocation);
                fs.Close();
            } catch (Exception ex) {
                return false;
            }
        }
        return true;
    }

    private static void _mWriteLine(string datetime, string line)
    {
        if (_mCheckFile())
        {
            using (StreamWriter sw = File.AppendText(_fileLocation))
            {
                sw.WriteLine($"{datetime} - {line}");
            }
        }
    }
    
    public static void WriteLine(string? dateTime, string line)
    {
        string STR_dateTime = dateTime is not null ? dateTime : _mGetTimeStamp();
        _mWriteLine(STR_dateTime, line);
    }

    public static void WriteLine(DateTime? dateTime, string line)
    {
        string STR_dateTime = dateTime is not null ? dateTime.ToString() : _mGetTimeStamp();
        _mWriteLine(STR_dateTime, line);
    }

    public static void WriteLine(string line)
    {
        _mWriteLine(_mGetTimeStamp(), line);
    }

    public static void WriteErrorLine(string errorLine) => WriteErrorLine(_mGetTimeStamp(), errorLine);

    public static void WriteErrorLine(string? dateTime, string errorLine)
    {
        string ErrorIncidentCode = _mGenIncCode();
        WriteLine(dateTime, $"Incident code: {ErrorIncidentCode}, Error: " + errorLine);
    }
    
    public static void WriteErrorLine(DateTime? dateTime, string errorLine)
    {
        string ErrorIncidentCode = _mGenIncCode();
        WriteLine(dateTime, $"Incident code: {ErrorIncidentCode}, Error: " + errorLine);
    }

    public static void WriteBlock(List<string> lines)
    {
        string MessageBlockCode = _mGenMessCode();

        WriteLine(GenLine('=', 10));
        WriteLine($"START - MESSAGE BLOCK - CODE: {MessageBlockCode} - START");
        WriteLine(GenLine('=', 10));

        foreach (string line in lines)
        {
            
        }
        
        WriteLine(GenLine('=', 10));
        WriteLine($"END - MESSAGE BLOCK - CODE: {MessageBlockCode} - END");
        WriteLine(GenLine('=', 10));
    }

    public static void WriteErrorBlock(List<string> errorLines)
    {
        string ErrorIncidentCode = _mGenIncCode();

        WriteLine(GenLine('=', 10));
        WriteLine($"START - ERROR INCIDENT - CODE: {ErrorIncidentCode} - START");
        WriteLine(GenLine('=', 10));

        foreach (string errorline in errorLines)
        {

        }

        WriteLine(GenLine('=', 10));
        WriteLine($"END - ERROR MESSAGE - CODE: {ErrorIncidentCode} - END");
        WriteLine(GenLine('=', 10));
    }

    public static void UnsavedChange<T>(string modelName, int objectID, string objectField, T originalValue, T updateValue) {
        WriteLine($"UNSAVED CHANGE: {modelName} (ID: {objectID}) has its {objectField} updated from [{originalValue}] to [{updateValue}]");
    }

    private static string GenLine(char character, int amount)
    {
        string returnValue = "";
        for (int i = 0; i < amount; i++)
        {
            returnValue += character;
        }
        return returnValue;
    }
}