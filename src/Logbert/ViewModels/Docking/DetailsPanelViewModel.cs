using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Logbert.Logging;
using Logbert.ViewModels.Controls.Details;
using Logbert.Views.Controls;
using Logbert.Views.Controls.Details;

namespace Logbert.ViewModels.Docking;

/// <summary>
/// ViewModel for the dockable details panel.
/// </summary>
public partial class DetailsPanelViewModel : ViewModelBase
{
    [ObservableProperty]
    private LogMessage? _selectedMessage;

    [ObservableProperty]
    private string _selectedViewMode = "Auto";

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private ObservableCollection<string> _viewModes = new()
    {
        "Auto",
        "Enhanced",
        "Simple",
        "Log4Net",
        "Syslog",
        "EventLog"
    };

    // Cached view instances
    private readonly EnhancedLogDetailsView _enhancedView = new();
    private readonly LogDetailsView _simpleView = new();
    private readonly Log4NetDetailsView _log4NetView = new();
    private readonly SyslogDetailsView _syslogView = new();
    private readonly EventLogDetailsView _eventLogView = new();

    // ViewModels for specialized views
    private readonly Log4NetDetailsViewModel _log4NetViewModel = new();
    private readonly SyslogDetailsViewModel _syslogViewModel = new();
    private readonly EventLogDetailsViewModel _eventLogViewModel = new();

    public DetailsPanelViewModel()
    {
        _log4NetView.DataContext = _log4NetViewModel;
        _syslogView.DataContext = _syslogViewModel;
        _eventLogView.DataContext = _eventLogViewModel;

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedMessage) || e.PropertyName == nameof(SelectedViewMode))
            {
                UpdateCurrentView();
            }
        };
    }

    /// <summary>
    /// Sets the message to display.
    /// </summary>
    public void SetMessage(LogMessage? message)
    {
        SelectedMessage = message;
    }

    private void UpdateCurrentView()
    {
        if (SelectedMessage == null)
        {
            CurrentView = null;
            return;
        }

        switch (SelectedViewMode)
        {
            case "Auto":
                SelectAutoView();
                break;
            case "Enhanced":
                _enhancedView.DataContext = SelectedMessage;
                CurrentView = _enhancedView;
                break;
            case "Simple":
                _simpleView.DataContext = SelectedMessage;
                CurrentView = _simpleView;
                break;
            case "Log4Net":
                if (SelectedMessage is LogMessageLog4Net log4NetMsg)
                {
                    _log4NetViewModel.SetMessage(log4NetMsg);
                    CurrentView = _log4NetView;
                }
                else
                {
                    _enhancedView.DataContext = SelectedMessage;
                    CurrentView = _enhancedView;
                }
                break;
            case "Syslog":
                if (SelectedMessage is LogMessageSyslog syslogMsg)
                {
                    _syslogViewModel.SetMessage(syslogMsg);
                    CurrentView = _syslogView;
                }
                else
                {
                    _enhancedView.DataContext = SelectedMessage;
                    CurrentView = _enhancedView;
                }
                break;
            case "EventLog":
                if (SelectedMessage is LogMessageEventlog eventLogMsg)
                {
                    _eventLogViewModel.SetMessage(eventLogMsg);
                    CurrentView = _eventLogView;
                }
                else
                {
                    _enhancedView.DataContext = SelectedMessage;
                    CurrentView = _enhancedView;
                }
                break;
            default:
                _enhancedView.DataContext = SelectedMessage;
                CurrentView = _enhancedView;
                break;
        }
    }

    private void SelectAutoView()
    {
        // Automatically select the best view based on message type
        switch (SelectedMessage)
        {
            case LogMessageLog4Net log4NetMsg:
                _log4NetViewModel.SetMessage(log4NetMsg);
                CurrentView = _log4NetView;
                break;
            case LogMessageSyslog syslogMsg:
                _syslogViewModel.SetMessage(syslogMsg);
                CurrentView = _syslogView;
                break;
            case LogMessageEventlog eventLogMsg:
                _eventLogViewModel.SetMessage(eventLogMsg);
                CurrentView = _eventLogView;
                break;
            default:
                // Use enhanced view for generic messages
                _enhancedView.DataContext = SelectedMessage;
                CurrentView = _enhancedView;
                break;
        }
    }
}
