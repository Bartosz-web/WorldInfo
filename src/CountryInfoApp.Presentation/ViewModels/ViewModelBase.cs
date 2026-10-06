using CommunityToolkit.Mvvm.ComponentModel;
using CountryInfoApp.Core.Exceptions;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Runs an operation with a busy indicator and turns expected failures into a Polish message.
    /// </summary>
    /// <returns><c>true</c> when the operation completed successfully.</returns>
    protected async Task<bool> RunAsync(Func<Task> operation)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await operation();
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (EntityNotFoundException)
        {
            ErrorMessage = UiText.NotFound;
        }
        catch (CountryInfoServiceException)
        {
            ErrorMessage = UiText.ServiceUnavailable;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ErrorMessage = UiText.FileError;
        }
        finally
        {
            IsBusy = false;
        }

        return false;
    }
}
