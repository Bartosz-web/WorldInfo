using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Infrastructure.Soap;
using static CountryInfoApp.Infrastructure.Soap.CountryInfoOperations;

namespace CountryInfoApp.Infrastructure.Services;

public sealed class SoapLanguageService : ILanguageService
{
    private readonly ISoapClient _client;
    private readonly CountryInfoXmlMapper _mapper;

    public SoapLanguageService(ISoapClient client, CountryInfoXmlMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Language>> GetLanguagesAsync(SortOrder sortOrder, CancellationToken cancellationToken = default)
    {
        var operation = sortOrder == SortOrder.ByCode ? ListOfLanguagesByCode : ListOfLanguagesByName;
        var result = await _client.InvokeAsync(operation, NoParameters, cancellationToken).ConfigureAwait(false);
        return _mapper.ToLanguages(result);
    }

    public async Task<string> GetLanguageNameAsync(string languageIsoCode, CancellationToken cancellationToken = default)
    {
        var result = await _client
            .InvokeAsync(LanguageName, With(Parameters.IsoCode, languageIsoCode.ToLowerInvariant()), cancellationToken)
            .ConfigureAwait(false);
        return CountryInfoXmlMapper.ToText(result);
    }

    public async Task<string> GetLanguageIsoCodeAsync(string languageName, CancellationToken cancellationToken = default)
    {
        var result = await _client
            .InvokeAsync(LanguageISOCode, With(Parameters.Language, languageName), cancellationToken)
            .ConfigureAwait(false);
        return CountryInfoXmlMapper.ToText(result);
    }
}
