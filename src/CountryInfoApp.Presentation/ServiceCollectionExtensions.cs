using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Services;
using CountryInfoApp.Presentation.Abstractions;
using CountryInfoApp.Presentation.Lookups;
using CountryInfoApp.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CountryInfoApp.Presentation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCountryInfoPresentation(this IServiceCollection services)
    {
        services.AddSingleton<ICountryStatisticsService, CountryStatisticsService>();
        services.AddSingleton<IQuizService>(_ => new QuizService(Random.Shared));
        services.AddSingleton<ILookupOptionsProvider, LookupOptionsProvider>();

        services.AddTransient<CountryDetailsViewModel>();

        // Registration order defines the tab order in the main window.
        AddTab<CountryBrowserViewModel>(services);
        AddTab<LookupViewModel>(services);
        AddTab<AllCountriesViewModel>(services);
        AddTab<CurrenciesViewModel>(services);
        AddTab<LanguagesViewModel>(services);
        AddTab<StatisticsViewModel>(services);
        AddTab<QuizViewModel>(services);

        services.AddSingleton<MainViewModel>();
        return services;
    }

    private static void AddTab<TTab>(IServiceCollection services)
        where TTab : class, ITabViewModel
    {
        services.AddSingleton<TTab>();
        services.AddSingleton<ITabViewModel>(provider => provider.GetRequiredService<TTab>());
    }
}
