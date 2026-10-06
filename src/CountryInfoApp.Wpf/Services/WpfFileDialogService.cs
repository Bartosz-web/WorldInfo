using CountryInfoApp.Presentation.Abstractions;
using Microsoft.Win32;

namespace CountryInfoApp.Wpf.Services;

public sealed class WpfFileDialogService : IFileDialogService
{
    public string? AskSaveFilePath(string defaultFileName, string filter)
    {
        var dialog = new SaveFileDialog
        {
            FileName = defaultFileName,
            Filter = filter,
            AddExtension = true,
            OverwritePrompt = true,
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
