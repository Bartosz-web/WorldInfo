using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CountryInfoApp.Presentation.Localization;
using CountryInfoApp.Presentation.Lookups;

namespace CountryInfoApp.Presentation.ViewModels;

public sealed partial class LookupViewModel : TabViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private LookupOption? _selectedOption;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private string _input = string.Empty;

    [ObservableProperty]
    private string? _result;

    public LookupViewModel(ILookupOptionsProvider optionsProvider)
    {
        Options = optionsProvider.GetOptions();
        _selectedOption = Options.Count > 0 ? Options[0] : null;
    }

    public override string Title => UiText.LookupTab;

    public IReadOnlyList<LookupOption> Options { get; }

    protected override Task LoadAsync() => Task.CompletedTask;

    partial void OnSelectedOptionChanged(LookupOption? value) => Result = null;

    private bool CanSearch() => SelectedOption is not null && !string.IsNullOrWhiteSpace(Input);

    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task SearchAsync()
    {
        Result = null;
        var option = SelectedOption!;
        var input = Input.Trim();
        await RunAsync(async () => Result = await option.ExecuteAsync(input, CancellationToken.None));
    }
}
