public static class AppUI
{
    public static string GenProgressBar(int currentProgress, int maxProgress, int maxSize=32)
    {
        double progressDone = Convert.ToDouble(currentProgress) / Convert.ToDouble(currentProgress);
        int BlocksDone = Convert.ToInt32(Convert.ToDouble(maxSize / 100) * progressDone);

        string returnValue = "";
        for (int i = 0; i < maxSize; i++)
        {
            returnValue += "";
        }

        for (int i = returnValue.Count(); i < maxSize; i++)
        {
            returnValue += "";
        }

        return returnValue;
    }
}