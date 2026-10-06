using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Abstractions;

public interface ILanguageService
{
    Task<IReadOnlyList<Language>> GetLanguagesAsync(SortOrder sortOrder, CancellationToken cancellationToken = default);

    Task<string> GetLanguageNameAsync(string languageIsoCode, CancellationToken cancellationToken = default);

    Task<string> GetLanguageIsoCodeAsync(string languageName, CancellationToken cancellationToken = default);
}
