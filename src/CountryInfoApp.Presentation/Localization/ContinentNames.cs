namespace CountryInfoApp.Presentation.Localization;

/// <summary>
/// The service returns English continent names; this maps the known codes to Polish.
/// </summary>
public static class ContinentNames
{
    private static readonly Dictionary<string, string> Polish = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AF"] = "Afryka",
        ["AM"] = "Ameryki",
        ["AN"] = "Antarktyda",
        ["AS"] = "Azja",
        ["EU"] = "Europa",
        ["OC"] = "Oceania",
    };

    public static string ToPolish(string code, string? fallbackName = null) =>
        Polish.TryGetValue(code, out var name) ? name : fallbackName ?? code;
}
