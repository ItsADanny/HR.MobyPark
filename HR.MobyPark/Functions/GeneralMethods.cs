public static class GeneralMethods
{
    public static bool IntToBool(int input) => input == 1;
    public static int BoolToInt(bool input) => input ? 1 : 0;
    // Parse flexible (handles US format from database), DateTime object works universally
    public static DateTime ParseDBDateTime(string dbDateTimeString) => DateTime.Parse(dbDateTimeString, System.Globalization.CultureInfo.InvariantCulture);
    public static string ToNLDateTimeString(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss", new System.Globalization.CultureInfo("nl-NL"));
}