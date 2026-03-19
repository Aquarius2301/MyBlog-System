namespace BusinessObject.Enums;

public sealed record LanguageType(string Code)
{
    public static readonly LanguageType English = new("en");
    public static readonly LanguageType Vietnamese = new("vi");
    public static readonly LanguageType Japanese = new("ja");

    public static readonly List<LanguageType> All = [English, Vietnamese, Japanese];

    public static bool IsValid(string code)
    {
        return All.Any(x => x.Code == code);
    }
}
