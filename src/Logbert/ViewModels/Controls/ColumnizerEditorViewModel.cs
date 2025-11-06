using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Couchcoding.Logbert.Logging;
using Couchcoding.Logbert.Receiver.CustomReceiver;

namespace Couchcoding.Logbert.ViewModels.Controls;

public partial class ColumnizerEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _columnizerName = "New Columnizer";

    [ObservableProperty]
    private string _dateTimeFormat = Columnizer.DefaultDateTimeFormat;

    [ObservableProperty]
    private ObservableCollection<LogColumnViewModel> _columns = new();

    [ObservableProperty]
    private LogColumnViewModel? _selectedColumn;

    [ObservableProperty]
    private ObservableCollection<LogLevelMappingViewModel> _logLevelMappings = new();

    public IRelayCommand AddColumnCommand { get; }
    public IRelayCommand RemoveColumnCommand { get; }
    public IRelayCommand MoveColumnUpCommand { get; }
    public IRelayCommand MoveColumnDownCommand { get; }

    public ColumnizerEditorViewModel()
    {
        AddColumnCommand = new RelayCommand(AddColumn);
        RemoveColumnCommand = new RelayCommand(RemoveColumn, CanRemoveColumn);
        MoveColumnUpCommand = new RelayCommand(MoveColumnUp, CanMoveColumnUp);
        MoveColumnDownCommand = new RelayCommand(MoveColumnDown, CanMoveColumnDown);

        // Initialize default log level mappings
        InitializeDefaultLogLevelMappings();

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedColumn))
            {
                ((RelayCommand)RemoveColumnCommand).NotifyCanExecuteChanged();
                ((RelayCommand)MoveColumnUpCommand).NotifyCanExecuteChanged();
                ((RelayCommand)MoveColumnDownCommand).NotifyCanExecuteChanged();
            }
        };
    }

    private void InitializeDefaultLogLevelMappings()
    {
        LogLevelMappings.Add(new LogLevelMappingViewModel
        {
            Level = LogLevel.Trace,
            LevelName = "Trace",
            Pattern = @"(?i)TRACE(?-i)"
        });

        LogLevelMappings.Add(new LogLevelMappingViewModel
        {
            Level = LogLevel.Debug,
            LevelName = "Debug",
            Pattern = @"(?i)DEBUG(?-i)"
        });

        LogLevelMappings.Add(new LogLevelMappingViewModel
        {
            Level = LogLevel.Info,
            LevelName = "Info",
            Pattern = @"(?i)INFO(?-i)"
        });

        LogLevelMappings.Add(new LogLevelMappingViewModel
        {
            Level = LogLevel.Warning,
            LevelName = "Warning",
            Pattern = @"(?i)WARN|WARNING(?-i)"
        });

        LogLevelMappings.Add(new LogLevelMappingViewModel
        {
            Level = LogLevel.Error,
            LevelName = "Error",
            Pattern = @"(?i)ERROR(?-i)"
        });

        LogLevelMappings.Add(new LogLevelMappingViewModel
        {
            Level = LogLevel.Fatal,
            LevelName = "Fatal",
            Pattern = @"(?i)FATAL(?-i)"
        });
    }

    private void AddColumn()
    {
        var newColumn = new LogColumnViewModel
        {
            Name = $"Column {Columns.Count + 1}",
            Expression = ".*",
            Optional = false,
            ColumnType = LogColumnType.Unknown
        };

        Columns.Add(newColumn);
        SelectedColumn = newColumn;
    }

    private bool CanRemoveColumn() => SelectedColumn != null;

    private void RemoveColumn()
    {
        if (SelectedColumn != null)
        {
            Columns.Remove(SelectedColumn);
            SelectedColumn = null;
        }
    }

    private bool CanMoveColumnUp() => SelectedColumn != null && Columns.IndexOf(SelectedColumn) > 0;

    private void MoveColumnUp()
    {
        if (SelectedColumn == null) return;

        int index = Columns.IndexOf(SelectedColumn);
        if (index > 0)
        {
            Columns.Move(index, index - 1);
        }
    }

    private bool CanMoveColumnDown() => SelectedColumn != null && Columns.IndexOf(SelectedColumn) < Columns.Count - 1;

    private void MoveColumnDown()
    {
        if (SelectedColumn == null) return;

        int index = Columns.IndexOf(SelectedColumn);
        if (index < Columns.Count - 1)
        {
            Columns.Move(index, index + 1);
        }
    }

    public Columnizer GetColumnizer()
    {
        var columnizer = new Columnizer(ColumnizerName)
        {
            DateTimeFormat = DateTimeFormat
        };

        // Add columns
        foreach (var columnVm in Columns)
        {
            columnizer.Columns.Add(new LogColumn
            {
                Name = columnVm.Name,
                Expression = columnVm.Expression,
                Optional = columnVm.Optional,
                ColumnType = columnVm.ColumnType
            });
        }

        // Add log level mappings
        columnizer.LogLevelMapping.Clear();
        foreach (var mapping in LogLevelMappings)
        {
            columnizer.LogLevelMapping[mapping.Level] = mapping.Pattern;
        }

        return columnizer;
    }

    public void LoadColumnizer(Columnizer columnizer)
    {
        ColumnizerName = columnizer.Name;
        DateTimeFormat = columnizer.DateTimeFormat;

        Columns.Clear();
        foreach (var column in columnizer.Columns)
        {
            Columns.Add(new LogColumnViewModel
            {
                Name = column.Name,
                Expression = column.Expression,
                Optional = column.Optional,
                ColumnType = column.ColumnType
            });
        }

        // Update log level mappings
        foreach (var mapping in LogLevelMappings)
        {
            if (columnizer.LogLevelMapping.TryGetValue(mapping.Level, out string? pattern))
            {
                mapping.Pattern = pattern;
            }
        }
    }
}

public partial class LogColumnViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _expression = string.Empty;

    [ObservableProperty]
    private bool _optional;

    [ObservableProperty]
    private LogColumnType _columnType;
}

public partial class LogLevelMappingViewModel : ObservableObject
{
    public LogLevel Level { get; set; }

    [ObservableProperty]
    private string _levelName = string.Empty;

    [ObservableProperty]
    private string _pattern = string.Empty;
}
