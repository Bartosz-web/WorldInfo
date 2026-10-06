using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using CountryInfoApp.Infrastructure;
using CountryInfoApp.Presentation;
using CountryInfoApp.Presentation.Abstractions;
using CountryInfoApp.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CountryInfoApp.Wpf;

public partial class App : Application
{
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Polish number/date formatting in all bindings.
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.GetCultureInfo("pl-PL").IetfLanguageTag)));

        DispatcherUnhandledException += OnDispatcherUnhandledException;

        _services = ConfigureServices();
        _services.GetRequiredService<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }

    /// <summary>
    /// Composition root: the only place that knows the concrete implementations.
    /// </summary>
    private static ServiceProvider ConfigureServices() =>
        new ServiceCollection()
            .AddCountryInfoInfrastructure()
            .AddCountryInfoPresentation()
            .AddSingleton<IFileDialogService, WpfFileDialogService>()
            .AddSingleton<MainWindow>()
            .BuildServiceProvider();

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            "Wystąpił nieoczekiwany błąd. Aplikacja spróbuje kontynuować działanie.\n\n" + e.Exception.Message,
            "Błąd",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }
}
