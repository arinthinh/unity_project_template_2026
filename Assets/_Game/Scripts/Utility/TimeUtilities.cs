using System;

public class TimeUtilities
{
    public static DateTime ConvertTimeString(string timeString)
    {
        return DateTime.Parse(timeString);
    }
    
    public static string ConvertToTimeString(DateTime time)
    {
        return time.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    public static string ConvertSecondsToTimeString(int seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", 
            timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}