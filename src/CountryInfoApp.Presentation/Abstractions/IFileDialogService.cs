namespace CountryInfoApp.Presentation.Abstractions;

public interface IFileDialogService
{
    /// <summary>Returns the chosen path, or <c>null</c> when the user cancelled.</summary>
    string? AskSaveFilePath(string defaultFileName, string filter);
}
