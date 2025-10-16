public static class General
{
    public static bool IntToBool(int value)
    {
        if (value == 1) return true;
        return false;
    }

    public static int BoolToInt(bool value)
    {
        if (value) return 1;
        return 0;
    }
}