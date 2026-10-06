namespace CountryInfoApp.Presentation;

internal static class TextFilter
{
    public static bool Matches(string? filter, params string[] values) =>
        string.IsNullOrWhiteSpace(filter)
        || values.Any(value => value.Contains(filter.Trim(), StringComparison.OrdinalIgnoreCase));
}
