using CommunityToolkit.Mvvm.ComponentModel;
using CountryInfoApp.Presentation.Abstractions;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ITabViewModel? _selectedTab;

    public MainViewModel(IEnumerable<ITabViewModel> tabs)
    {
        Tabs = tabs.ToList();
        SelectedTab = Tabs.Count > 0 ? Tabs[0] : null;
    }

    public static string Title => UiText.AppTitle;

    public IReadOnlyList<ITabViewModel> Tabs { get; }

    partial void OnSelectedTabChanged(ITabViewModel? value) => _ = value?.EnsureLoadedAsync();
}
