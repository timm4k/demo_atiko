namespace atiko.Models;

public class LanguageOption
{
    public string DisplayName { get; set; }
    public string LanguageCode { get; set; }

    public LanguageOption(string displayName, string languageCode)
    {
        DisplayName = displayName;
        LanguageCode = languageCode;
    }

    public LanguageOption() => (DisplayName, LanguageCode) = (string.Empty, string.Empty);
}
