using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Abstractions;

public interface ICountryExporter
{
    Task ExportAsync(IEnumerable<CountryDetails> countries, string filePath, CancellationToken cancellationToken = default);
}
