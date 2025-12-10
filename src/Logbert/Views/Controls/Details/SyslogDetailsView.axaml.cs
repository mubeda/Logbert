using Avalonia.Controls;

namespace Logbert.Views.Controls.Details;

/// <summary>
/// Detail view for Syslog log messages.
/// Displays Syslog-specific properties like Facility, Severity, and timestamps.
/// </summary>
public partial class SyslogDetailsView : UserControl
{
    public SyslogDetailsView()
    {
        InitializeComponent();
    }
}
