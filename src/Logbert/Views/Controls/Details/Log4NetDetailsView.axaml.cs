using Avalonia.Controls;

namespace Logbert.Views.Controls.Details;

/// <summary>
/// Detail view for Log4Net log messages.
/// Displays Log4Net-specific properties like Thread, Location, and Custom Properties.
/// </summary>
public partial class Log4NetDetailsView : UserControl
{
    public Log4NetDetailsView()
    {
        InitializeComponent();
    }
}
