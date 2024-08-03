namespace Lidas.MangaApi.Extensions;

public static class DateTimeExtensions
{
    public static bool IsUtc(this DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Utc;
    }
}
