namespace HiSubmit.Client.SharedModels.Extensions;

public static class DateTimeExtension
{
    public static bool Between(this DateTime date, DateTime? startDate, DateTime? endDate)
    {
        if (startDate == null || endDate == null) return false;
        return date > startDate && date < endDate;
    }
    
    public static string ToLongDate(this DateTime time)
    {
        return time.ToString("MMMM dd,yyyy");
    }

    public static string ToLongTime(this DateTime time)
    {
        return time.ToString("hh:mm tt");
    }
    
    public static string ToLongDate(this DateTime? time)
    {
        return time == null ? string.Empty : time.Value.ToLongDate();
    }

    public static string ToLongTime(this DateTime? time)
    {
        return time == null ? string.Empty : time.Value.ToLongTime();
    }

    public static string ToLongDateTime(this DateTime time)
    {
        return time.ToString("MMMM dd,yyyy - hh:mm tt");
    }

    public static string ToLongDateTime(this DateTime? time)
    {
        return time == null ? string.Empty : time.Value.ToLongDateTime();
    }

    public static string LongDateFormat => "MMMM dd,yyyy";
    public static string LognTimeFormat => "hh:mm tt";

}