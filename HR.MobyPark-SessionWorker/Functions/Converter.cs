public static class Converter
{
    public static int DayToMilisecond(double days) => (int) days * HourToMilisecond(24);
    public static int HourToMilisecond(double hours) => (int) hours * MinuteToMilisecond(60);
    public static int MinuteToMilisecond(double minutes) => (int) minutes * SecondToMilisecond(60);
    public static int SecondToMilisecond(double seconds) => (int) seconds * 1000;
}