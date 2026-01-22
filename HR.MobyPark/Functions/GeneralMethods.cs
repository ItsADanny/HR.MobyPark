public static class GeneralMethods
{
    public static bool IntToBool(int input) => input == 1;
    public static int BoolToInt(bool input) => input ? 1 : 0;
    public static DateTime ParseDBDateTime(string dbDateTimeString)
    {
        if (DateTime.TryParse(dbDateTimeString, out DateTime result))
        {
            return result;
        }
        return DateTime.Parse(dbDateTimeString.Replace("-", "/"), new System.Globalization.CultureInfo("nl-NL"));
    }
    public static string ToNLDateTimeString(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss", new System.Globalization.CultureInfo("nl-NL"));
}