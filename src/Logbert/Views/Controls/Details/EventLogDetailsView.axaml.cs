using Avalonia.Controls;

namespace Logbert.Views.Controls.Details;

/// <summary>
/// Detail view for Windows Event Log messages.
/// Displays EventLog-specific properties like InstanceId, Category, Username, and binary data.
/// </summary>
public partial class EventLogDetailsView : UserControl
{
    public EventLogDetailsView()
    {
        InitializeComponent();
    }
}
