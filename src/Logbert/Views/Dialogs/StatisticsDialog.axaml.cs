using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Logbert.ViewModels.Controls;
using Logbert.Logging;

namespace Logbert.Views.Dialogs;

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

        // Set as DataContext for the window
        DataContext = viewModel;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
