using CommunityToolkit.Mvvm.Input;
using CountryInfoApp.Presentation.Abstractions;

namespace CountryInfoApp.Presentation.ViewModels;

public abstract partial class TabViewModelBase : ViewModelBase, ITabViewModel
{
    private bool _isLoaded;

    public abstract string Title { get; }

    public async Task EnsureLoadedAsync()
    {
        if (_isLoaded || IsBusy)
        {
            return;
        }

        _isLoaded = await RunAsync(LoadAsync);
    }

    protected abstract Task LoadAsync();

    [RelayCommand]
    private Task RetryAsync()
    {
        _isLoaded = false;
        return EnsureLoadedAsync();
    }
}
