namespace Reporter.Extensions;

public static class Extensions
{
    public static decimal GetHoursFromSeconds(this long timeInSeconds)
    {
        return timeInSeconds / (60 * 60);
    }
}
