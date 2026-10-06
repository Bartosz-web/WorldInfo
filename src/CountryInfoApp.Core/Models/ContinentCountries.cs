namespace CountryInfoApp.Core.Models;

public sealed record ContinentCountries(Continent Continent, IReadOnlyList<CountrySummary> Countries);
