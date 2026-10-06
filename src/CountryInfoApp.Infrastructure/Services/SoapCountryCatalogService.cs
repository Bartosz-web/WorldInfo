using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Infrastructure.Soap;
using static CountryInfoApp.Infrastructure.Soap.CountryInfoOperations;

namespace CountryInfoApp.Infrastructure.Services;

public sealed class SoapCountryCatalogService : ICountryCatalogService
{
    private readonly ISoapClient _client;
    private readonly CountryInfoXmlMapper _mapper;

    public SoapCountryCatalogService(ISoapClient client, CountryInfoXmlMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CountrySummary>> GetCountriesAsync(SortOrder sortOrder, CancellationToken cancellationToken = default)
    {
        var operation = sortOrder == SortOrder.ByCode ? ListOfCountryNamesByCode : ListOfCountryNamesByName;
        var result = await _client.InvokeAsync(operation, NoParameters, cancellationToken).ConfigureAwait(false);
        return _mapper.ToCountrySummaries(result);
    }

    public async Task<IReadOnlyList<ContinentCountries>> GetCountriesGroupedByContinentAsync(CancellationToken cancellationToken = default)
    {
        var result = await _client.InvokeAsync(ListOfCountryNamesGroupedByContinent, NoParameters, cancellationToken).ConfigureAwait(false);
        return _mapper.ToContinentCountries(result);
    }

    public async Task<CountryDetails> GetCountryDetailsAsync(string countryIsoCode, CancellationToken cancellationToken = default)
    {
        var result = await _client
            .InvokeAsync(FullCountryInfo, With(Parameters.CountryIsoCode, countryIsoCode.ToUpperInvariant()), cancellationToken)
            .ConfigureAwait(false);
        return _mapper.ToCountryDetails(result);
    }

    public async Task<IReadOnlyList<CountryDetails>> GetAllCountryDetailsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _client.InvokeAsync(FullCountryInfoAllCountries, NoParameters, cancellationToken).ConfigureAwait(false);
        return _mapper.ToCountryDetailsList(result);
    }
}
