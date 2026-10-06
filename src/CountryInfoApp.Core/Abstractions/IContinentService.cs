using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Abstractions;

public interface IContinentService
{
    Task<IReadOnlyList<Continent>> GetContinentsAsync(SortOrder sortOrder, CancellationToken cancellationToken = default);
}
