using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Couchcoding.Logbert.ViewModels.Controls;
using Couchcoding.Logbert.Views.Dialogs;

namespace Couchcoding.Logbert.Views.Controls;

public partial class ColumnizerEditorControl : UserControl
{
    public ColumnizerEditorViewModel? ViewModel => DataContext as ColumnizerEditorViewModel;

    public ColumnizerEditorControl()
    {
        InitializeComponent();
        DataContext = new ColumnizerEditorViewModel();
    }

    /// <summary>
    /// Opens the Columnizer Test Dialog to test regex patterns.
    /// </summary>
    private async void OnTestPatternClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        // Build the full pattern from all column expressions
        var pattern = BuildPatternFromColumns();

        var dialog = new ColumnizerTestDialog();
        dialog.Initialize(pattern);

        var result = await dialog.ShowDialog<bool?>(topLevel as Window);

        if (result == true && dialog.PatternAccepted && !string.IsNullOrEmpty(dialog.ResultPattern))
        {
            // User accepted a pattern - optionally update the columns from the pattern
            // This is informational for now; advanced integration could parse the pattern
            // and update the columns automatically
        }
    }

    /// <summary>
    /// Builds a combined regex pattern from all column expressions.
    /// </summary>
    private string BuildPatternFromColumns()
    {
        if (ViewModel?.Columns == null || !ViewModel.Columns.Any())
        {
            // Return a sample pattern if no columns defined
            return @"^(?<timestamp>\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})\s+\[(?<level>\w+)\]\s+(?<logger>[\w\.]+)\s+-\s+(?<message>.*)$";
        }

        // Build pattern from column expressions
        // Each column should ideally have a named capturing group
        var patterns = ViewModel.Columns
            .Where(c => !string.IsNullOrWhiteSpace(c.Expression))
            .Select(c => c.Expression);

        // If columns have separate patterns, join them
        var combined = string.Join(@"\s+", patterns);

        // If the combined pattern doesn't look like a full regex, wrap it
        if (!combined.StartsWith("^"))
        {
            combined = "^" + combined;
        }
        if (!combined.EndsWith("$"))
        {
            combined = combined + "$";
        }

        return combined;
    }
}
