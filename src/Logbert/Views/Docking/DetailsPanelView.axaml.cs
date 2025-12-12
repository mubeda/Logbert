using Avalonia.Controls;

namespace Logbert.Views.Docking;

/// <summary>
/// Dockable panel for displaying log message details.
/// Automatically selects the appropriate detail view based on the message type.
/// </summary>
public partial class DetailsPanelView : UserControl
{
    public DetailsPanelView()
    {
        InitializeComponent();
    }
}
