using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Infrastructure.Soap;
using static CountryInfoApp.Infrastructure.Soap.CountryInfoOperations;

namespace CountryInfoApp.Infrastructure.Services;

public sealed class SoapContinentService : IContinentService
{
    private readonly ISoapClient _client;
    private readonly CountryInfoXmlMapper _mapper;

    public SoapContinentService(ISoapClient client, CountryInfoXmlMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Continent>> GetContinentsAsync(SortOrder sortOrder, CancellationToken cancellationToken = default)
    {
        var operation = sortOrder == SortOrder.ByCode ? ListOfContinentsByCode : ListOfContinentsByName;
        var result = await _client.InvokeAsync(operation, NoParameters, cancellationToken).ConfigureAwait(false);
        return _mapper.ToContinents(result);
    }
}
