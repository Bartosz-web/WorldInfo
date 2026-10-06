namespace CountryInfoApp.Presentation.Abstractions;

/// <summary>
/// A main-window tab whose data is loaded lazily, the first time the tab is shown.
/// </summary>
public interface ITabViewModel
{
    string Title { get; }

    Task EnsureLoadedAsync();
}
