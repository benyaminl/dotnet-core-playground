namespace TodoApi.Helper;

public static class DateTimeToEpoch
{
    public static int ToEpoch(this DateTime time)
    {
        TimeSpan t = time - new DateTime(1970, 1, 1);
        return (int) t.TotalSeconds;
    }
}