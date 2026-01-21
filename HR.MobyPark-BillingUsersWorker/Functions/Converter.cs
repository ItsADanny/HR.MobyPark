public static class Converter
{
    public static long DayToMilisecond(double days) => (long) days * HourToMilisecond(24);
    public static long HourToMilisecond(double hours) => (long) hours * MinuteToMilisecond(60);
    public static long MinuteToMilisecond(double minutes) => (long) minutes * SecondToMilisecond(60);
    public static long SecondToMilisecond(double seconds) => (long) seconds * 1000;
}