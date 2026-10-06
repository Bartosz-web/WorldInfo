using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Infrastructure.Soap;
using static CountryInfoApp.Infrastructure.Soap.CountryInfoOperations;

namespace CountryInfoApp.Infrastructure.Services;

public sealed class SoapCurrencyService : ICurrencyService
{
    private readonly ISoapClient _client;
    private readonly CountryInfoXmlMapper _mapper;

    public SoapCurrencyService(ISoapClient client, CountryInfoXmlMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Currency>> GetCurrenciesAsync(SortOrder sortOrder, CancellationToken cancellationToken = default)
    {
        var operation = sortOrder == SortOrder.ByCode ? ListOfCurrenciesByCode : ListOfCurrenciesByName;
        var result = await _client.InvokeAsync(operation, NoParameters, cancellationToken).ConfigureAwait(false);
        return _mapper.ToCurrencies(result);
    }

    public async Task<string> GetCurrencyNameAsync(string currencyIsoCode, CancellationToken cancellationToken = default)
    {
        var result = await _client
            .InvokeAsync(CurrencyName, With(Parameters.CurrencyIsoCode, currencyIsoCode.ToUpperInvariant()), cancellationToken)
            .ConfigureAwait(false);
        return CountryInfoXmlMapper.ToText(result);
    }

    public async Task<IReadOnlyList<CountrySummary>> GetCountriesUsingCurrencyAsync(string currencyIsoCode, CancellationToken cancellationToken = default)
    {
        var result = await _client
            .InvokeAsync(CountriesUsingCurrency, With(Parameters.IsoCurrencyCode, currencyIsoCode.ToUpperInvariant()), cancellationToken)
            .ConfigureAwait(false);
        return _mapper.ToCountrySummaries(result);
    }
}
