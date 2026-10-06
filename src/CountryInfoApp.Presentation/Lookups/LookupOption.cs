namespace CountryInfoApp.Presentation.Lookups;

/// <summary>
/// One quick lookup in the search tab: a title, an input hint and the call that answers it.
/// </summary>
public sealed record LookupOption(
    string Title,
    string InputHint,
    Func<string, CancellationToken, Task<string>> ExecuteAsync);
