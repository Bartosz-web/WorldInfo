using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Infrastructure.Soap;
using static CountryInfoApp.Infrastructure.Soap.CountryInfoOperations;

namespace CountryInfoApp.Infrastructure.Services;

public sealed class SoapCountryLookupService : ICountryLookupService
{
    private readonly ISoapClient _client;
    private readonly CountryInfoXmlMapper _mapper;

    public SoapCountryLookupService(ISoapClient client, CountryInfoXmlMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public Task<string> GetCountryNameAsync(string countryIsoCode, CancellationToken cancellationToken = default) =>
        GetTextAsync(CountryName, Parameters.CountryIsoCode, countryIsoCode.ToUpperInvariant(), cancellationToken);

    public Task<string> GetCountryIsoCodeAsync(string countryName, CancellationToken cancellationToken = default) =>
        GetTextAsync(CountryISOCode, Parameters.CountryName, countryName, cancellationToken);

    public Task<string> GetCapitalCityAsync(string countryIsoCode, CancellationToken cancellationToken = default) =>
        GetTextAsync(CapitalCity, Parameters.CountryIsoCode, countryIsoCode.ToUpperInvariant(), cancellationToken);

    public Task<string> GetPhoneCodeAsync(string countryIsoCode, CancellationToken cancellationToken = default) =>
        GetTextAsync(CountryIntPhoneCode, Parameters.CountryIsoCode, countryIsoCode.ToUpperInvariant(), cancellationToken);

    public async Task<Currency> GetCountryCurrencyAsync(string countryIsoCode, CancellationToken cancellationToken = default)
    {
        var result = await _client
            .InvokeAsync(CountryCurrency, With(Parameters.CountryIsoCode, countryIsoCode.ToUpperInvariant()), cancellationToken)
            .ConfigureAwait(false);
        return _mapper.ToCurrency(result);
    }

    private async Task<string> GetTextAsync(string operation, string parameter, string value, CancellationToken cancellationToken)
    {
        var result = await _client.InvokeAsync(operation, With(parameter, value), cancellationToken).ConfigureAwait(false);
        return CountryInfoXmlMapper.ToText(result);
    }
}
