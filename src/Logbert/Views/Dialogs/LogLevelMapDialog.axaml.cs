using Avalonia.Controls;
using Logbert.ViewModels.Dialogs;

namespace Logbert.Views.Dialogs;

/// <summary>
/// Dialog for configuring log level regex mappings.
/// Used by Custom receivers to map log level strings to LogLevel enum values.
/// </summary>
public partial class LogLevelMapDialog : Window
{
    public LogLevelMapDialog()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Creates a new dialog with the specified ViewModel.
    /// </summary>
    public LogLevelMapDialog(LogLevelMapDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;

        // Subscribe to DialogResult changes to close the window
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(LogLevelMapDialogViewModel.DialogResult) &&
                viewModel.DialogResult.HasValue)
            {
                Close(viewModel.DialogResult.Value);
            }
        };
    }
}
