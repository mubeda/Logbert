using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels.Controls;
using Couchcoding.Logbert.Logging;

namespace Couchcoding.Logbert.Views.Dialogs;

public partial class StatisticsDialog : Window
{
    public StatisticsDialog()
    {
        InitializeComponent();
    }

    public StatisticsDialog(IEnumerable<LogMessage> messages)
    {
        InitializeComponent();

        // Create statistics view model and update with messages
        var viewModel = new StatisticsViewModel();
        viewModel.UpdateStatistics(messages);

        // Set as DataContext for both the window and the control
        DataContext = viewModel;

        // Update the control's DataContext as well
        if (this.FindControl<Controls.StatisticsControl>("StatisticsControl") is { } control)
        {
            control.DataContext = viewModel;
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
