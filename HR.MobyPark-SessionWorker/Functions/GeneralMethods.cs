public static class GeneralMethods
{
    public static DateTime ParseDBDateTime(string dbDateTimeString) => DateTime.ParseExact(dbDateTimeString, "dd/MM/yyyy HH:mm:ss", new System.Globalization.CultureInfo("nl-NL"));
}