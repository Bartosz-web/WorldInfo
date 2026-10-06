using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Infrastructure.Export;
using CountryInfoApp.Infrastructure.Services;
using CountryInfoApp.Infrastructure.Soap;
using Microsoft.Extensions.DependencyInjection;

namespace CountryInfoApp.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCountryInfoInfrastructure(this IServiceCollection services, SoapClientOptions? options = null)
    {
        var soapOptions = options ?? new SoapClientOptions();

        services.AddSingleton(soapOptions);
        services.AddSingleton(_ => new HttpClient { Timeout = soapOptions.Timeout });
        services.AddSingleton<ISoapClient>(provider =>
            new CachingSoapClient(new SoapClient(provider.GetRequiredService<HttpClient>(), soapOptions)));
        services.AddSingleton<CountryInfoXmlMapper>();

        services.AddSingleton<IContinentService, SoapContinentService>();
        services.AddSingleton<ICountryCatalogService, SoapCountryCatalogService>();
        services.AddSingleton<ICountryLookupService, SoapCountryLookupService>();
        services.AddSingleton<ICurrencyService, SoapCurrencyService>();
        services.AddSingleton<ILanguageService, SoapLanguageService>();
        services.AddSingleton<ICountryExporter, CsvCountryExporter>();

        return services;
    }
}
