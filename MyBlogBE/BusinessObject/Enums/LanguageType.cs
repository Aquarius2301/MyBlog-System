namespace BusinessObject.Enums;

public sealed record LanguageType(string Code)
{
    public static readonly LanguageType English = new("en");
    public static readonly LanguageType Vietnamese = new("vi");
    public static readonly LanguageType Japanese = new("jp");
}
